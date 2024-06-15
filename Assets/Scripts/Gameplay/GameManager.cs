using SideFX.Events;
using SideFX.SceneManagement.Events;
using Unity.Logging;
using UnityEngine;

namespace QWOPCycle.Gameplay {
    public readonly struct GameOverEvent : IEvent {
        public readonly uint Score;
        public readonly float Distance;
    }

    public sealed class GameManager : MonoBehaviour {
        [field: Header("Track Blocks")]
        [field: SerializeField]
        private GameObject blockPrefab;

        [field: SerializeField]
        [field: Tooltip("This value is only used in Start()")]
        private int blocksNumToCreate = 7;

        [field: SerializeField]
        [field: Tooltip("In meters per second")]
        private float trackSpeed = 1f;

        private GameObject[] blocks;

        private bool blocksReady;

        private int
            blockMovedIndex,
            blocksNumCreated;

        private float
            blockMoveToFrontThreshhold,
            blockMoveDistanceOld,
            blockLength;

        private EventBinding<SceneReady> _sceneReadyBinding;

        [SerializeField] private ScoreTracker _scoreTracker;

        private void OnEnable() {
            _sceneReadyBinding = new EventBinding<SceneReady>(OnSceneReady);
            EventBus<SceneReady>.Register(_sceneReadyBinding);
        }

        private void OnDisable() {
            EventBus<SceneReady>.Deregister(_sceneReadyBinding);
        }

        private void OnSceneReady(SceneReady e) {
            BlocksInitialize();
        }

        private void Start() {
            blocks = new GameObject[blocksNumToCreate];
            blockLength = blockPrefab.transform.localScale.z;
            blockMovedIndex = blocksNumToCreate - 1;
        }

        private void Update() {
            if (!blocksReady) { return; }
            BlocksMoveToFrontCheck();
            BlocksMove();
            TrackDistanceTravelled();
        }

        /// <summary>
        /// Instantiates all blocks and sets values
        /// </summary>
        private void BlocksInitialize() {
            for (int i = 0; i < blocksNumToCreate; i++) {
                GameObject blockNew = Instantiate(blockPrefab);
                blockNew.name = "block" + (i + 1);
                blockNew.transform.position = blockNew.transform.position.With(z: blockLength * i);
                blocks[i] = blockNew;
            }

            blocksNumCreated = blocksNumToCreate;
            blockMoveToFrontThreshhold = 0f - blockLength;
            blocksReady = true;
        }

        /// <summary>
        /// Moves all blocks towards the player by ScrollBlockDistance(). if the value alters during runtime, reset all block positions
        /// </summary>
        private void BlocksMove() {
            foreach (GameObject scrollingBlock in blocks) {
                scrollingBlock.transform.position = scrollingBlock.transform.position.With(
                    z: scrollingBlock.transform.position.z - (trackSpeed * Time.deltaTime));
            }
        }

        /// <summary>
        /// If the block's Z is beyond or equal to the threshold, move it in front of the most recently moved block
        /// </summary>
        private void BlocksMoveToFrontCheck() {
            foreach (GameObject scrollingBlock in blocks)
            {
                if (scrollingBlock.transform.position.z > blockMoveToFrontThreshhold) { continue; }
                scrollingBlock.transform.position = scrollingBlock.transform.position.With(
                    z: blocks[blockMovedIndex].transform.position.z + blockLength);

                blockMovedIndex++;
                if (blockMovedIndex >= blocksNumCreated) { blockMovedIndex = 0; }
            }
        }

        /// <summary>
        /// Resets all blocks to starting positions
        /// </summary>
        private void BlocksResetPositions() {
            blockMovedIndex = blocksNumCreated - 1;
            for (int i = 0; i < blocks.Length; i++) {
                blocks[i].transform.position = blocks[i].transform.position.With(z: blockLength * i);
            }
        }

        private void TrackDistanceTravelled() {
            _scoreTracker.AddDistance(trackSpeed * Time.deltaTime);
        }
    }
}
