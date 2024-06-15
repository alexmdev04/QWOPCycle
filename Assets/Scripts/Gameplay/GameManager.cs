using QWOPCycle.Player;
using QWOPCycle.Scoring;
using SideFX.Anchors;
using SideFX.Events;
using SideFX.SceneManagement;
using SideFX.SceneManagement.Events;
using UnityEngine;

namespace QWOPCycle.Gameplay {
    public readonly struct GameOverEvent : IEvent {
        public uint Score { get; init; }
        public float Distance { get; init; }
    }

    public sealed class GameManager : MonoBehaviour {
        [SerializeField] private GameManagerAnchor anchor;

        [field: Header("Track Blocks")] [field: SerializeField]
        private GameObject blockPrefab;

        [field: SerializeField] [field: Tooltip("This value is only used in Start()")]
        private int blocksNumToCreate = 7;

        [field: SerializeField] [field: Tooltip("In meters per second")]
        private float trackSpeed = 1f;

        private int
            _blockMovedIndex,
            _blocksNumCreated;

        private float
            _blockMoveToFrontThreshold,
            _blockMoveDistanceOld;

        private GameObject[] _blocks;

        private bool _blocksReady;
        private EventBinding<PlayerFellOver> _playerFellOverBinding;
        private EventBinding<SceneReady> _sceneReadyBinding;
        private ScoreTracker _scoreTracker;
        private PedalTracker _pedalTracker;

        public float BlockLength { get; private set; }
        public float BlockWidth { get; private set; }

        private void Start() {
            _blocks = new GameObject[blocksNumToCreate];
            BlockWidth = blockPrefab.transform.localScale.x;
            BlockLength = blockPrefab.transform.localScale.z;
            _blockMovedIndex = blocksNumToCreate - 1;
        }

        private void Update() {
            if (!_blocksReady) return;
            BlocksMoveToFrontCheck();
            BlocksMove();
            TrackDistanceTravelled();
        }

        private void Awake() {
            _scoreTracker = ScriptableObject.CreateInstance<ScoreTracker>();
            _pedalTracker = ScriptableObject.CreateInstance<PedalTracker>();
        }

        private void OnEnable() {
            _sceneReadyBinding = new EventBinding<SceneReady>(OnSceneReady);
            _playerFellOverBinding = new EventBinding<PlayerFellOver>(OnPlayerFellOver);
            EventBus<SceneReady>.Register(_sceneReadyBinding);
            EventBus<PlayerFellOver>.Register(_playerFellOverBinding);
            anchor.Provide(this);
        }

        private void OnDisable() {
            EventBus<SceneReady>.Deregister(_sceneReadyBinding);
            EventBus<PlayerFellOver>.Deregister(_playerFellOverBinding);
        }

        private void OnSceneReady(SceneReady e) {
            if (e.Scene is not GameplayScene) return;
            BlocksInitialize();
        }

        private void OnPlayerFellOver() {
            EventBus<GameOverEvent>.Raise(
                new GameOverEvent {
                    Score = _scoreTracker.Score,
                    Distance = _scoreTracker.DistanceTravelled,
                }
            );
        }

        /// <summary>
        /// Instantiates all blocks and sets values
        /// </summary>
        private void BlocksInitialize() {
            for (var i = 0; i < blocksNumToCreate; i++) {
                GameObject blockNew = Instantiate(blockPrefab);
                blockNew.name = "block" + (i + 1);
                blockNew.transform.position = blockNew.transform.position.With(z: BlockLength * i);
                _blocks[i] = blockNew;
            }

            _blocksNumCreated = blocksNumToCreate;
            _blockMoveToFrontThreshold = 0f - BlockLength;
            _blocksReady = true;
        }

        /// <summary>
        /// Moves all blocks towards the player by ScrollBlockDistance(). if the value alters during runtime, reset all block
        /// positions
        /// </summary>
        private void BlocksMove() {
            foreach (GameObject scrollingBlock in _blocks) {
                scrollingBlock.transform.position = scrollingBlock.transform.position.With(
                    z: scrollingBlock.transform.position.z - trackSpeed * Time.deltaTime
                );
            }
        }

        /// <summary>
        /// If the block's Z is beyond or equal to the threshold, move it in front of the most recently moved block
        /// </summary>
        private void BlocksMoveToFrontCheck() {
            foreach (GameObject scrollingBlock in _blocks) {
                if (scrollingBlock.transform.position.z > _blockMoveToFrontThreshold) continue;
                scrollingBlock.transform.position = scrollingBlock.transform.position.With(
                    z: _blocks[_blockMovedIndex].transform.position.z + BlockLength
                );

                _blockMovedIndex++;
                if (_blockMovedIndex >= _blocksNumCreated) _blockMovedIndex = 0;
            }
        }

        /// <summary>
        /// Resets all blocks to starting positions
        /// </summary>
        private void BlocksResetPositions() {
            _blockMovedIndex = _blocksNumCreated - 1;
            for (var i = 0; i < _blocks.Length; i++)
                _blocks[i].transform.position = _blocks[i].transform.position.With(z: BlockLength * i);
        }

        private void TrackDistanceTravelled() {
            _scoreTracker.AddDistance(trackSpeed * Time.deltaTime);
        }
    }
}
