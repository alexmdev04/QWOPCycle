using System;
using QWOPCycle.Player;
using QWOPCycle.Scoring;
using SideFX.Anchors;
using SideFX.Events;
using SideFX.SceneManagement;
using SideFX.SceneManagement.Events;
using Unity.Logging;
using UnityEngine;

namespace QWOPCycle.Gameplay {
    public readonly struct GameStart : IEvent { }

    public readonly struct GameReset : IEvent { }

    public readonly struct GameOver : IEvent {
        public double Score { get; init; }
        public float Distance { get; init; }
        public TimeSpan RunTime { get; init; }
    }

    public readonly struct LevelIncreaseEvent : IEvent {
        public uint Level { get; init; }
    }

    public sealed class GameManager : MonoBehaviour {
        [SerializeField] private GameManagerAnchor anchor;

        [Header("Track Blocks")]
        [SerializeField] private Block blockPrefab;

        [SerializeField] [Tooltip("This value is only used in Start()")]
        private int blocksNumToCreate = 7;

        public int blockLanes = 4;

        [SerializeField] [Tooltip("In meters per second")]
        private float minTrackSpeed = 0f;

        [SerializeField] [Tooltip("In meters per second")]
        private float maxSpeedBonus = 5f;

        [SerializeField] [Tooltip("In seconds")]
        private double levelLength = 20d;

        public float laneObstacleSpawnChance = 0.1f;
        private int _blockMovedIndex;
        private int _blocksNumCreated;
        private float _blockMoveToFrontThreshold;
        private float _blockMoveDistanceOld;
        private Block[] _blocks;
        private bool _blocksReady;
        private EventBinding<PlayerFellOver> _playerFellOverBinding;
        private EventBinding<SceneReady> _sceneReadyBinding;
        private EventBinding<GameReset> _gameResetBinding;
        private EventBinding<LevelIncreaseEvent> _levelIncreaseBinding;

        [SerializeField] private ScoreTracker _scoreTracker;
        [SerializeField] private PedalTracker _pedalTracker;

        public float BlockLength { get; private set; }
        public float BlockWidth { get; private set; }
        public float BlockLaneWidth { get; private set; }
        public uint LevelCurrent { get; private set; }

        private GameState _gameState = GameState.Building;

        private enum GameState {
            Building, Running, GameOver,
        }

        private void Start() {
            _blocks = new Block[blocksNumToCreate];
            BlockLength = blockPrefab.transform.localScale.z;
            BlockWidth = blockPrefab.transform.localScale.x;
            BlockLaneWidth = BlockWidth / blockLanes;
            _blockMovedIndex = blocksNumToCreate - 1;
        }

        private void Update() {
            switch (_gameState) {
                case GameState.Building:
                    if (_blocksReady) _gameState = GameState.Running;
                    return;
                case GameState.Running:
                    BlocksMoveToFrontCheck();
                    BlocksMove();
                    TrackDistanceTravelled();
                    _pedalTracker.Tick(Time.deltaTime);
                    _scoreTracker.Tick(Time.deltaTime);
                    LevelUpdate();
                    return;
                case GameState.GameOver:
                    break;
            }
        }

        private void OnEnable() {
            _sceneReadyBinding = new EventBinding<SceneReady>(OnSceneReady);
            _playerFellOverBinding = new EventBinding<PlayerFellOver>(OnPlayerFellOver);
            _gameResetBinding = new EventBinding<GameReset>(OnGameReset);
            _levelIncreaseBinding = new EventBinding<LevelIncreaseEvent>(OnLevelIncrease);
            EventBus<SceneReady>.Register(_sceneReadyBinding);
            EventBus<PlayerFellOver>.Register(_playerFellOverBinding);
            EventBus<GameReset>.Register(_gameResetBinding);
            EventBus<LevelIncreaseEvent>.Register(_levelIncreaseBinding);
            anchor.Provide(this);
        }

        private void OnDisable() {
            EventBus<SceneReady>.Deregister(_sceneReadyBinding);
            EventBus<PlayerFellOver>.Deregister(_playerFellOverBinding);
            EventBus<GameReset>.Deregister(_gameResetBinding);
            EventBus<LevelIncreaseEvent>.Deregister(_levelIncreaseBinding);
        }

        private void OnSceneReady(SceneReady e) {
            if (e.Scene is not GameplayScene) return;
            BlocksInitialize();
            foreach (Block block in _blocks) block.obstacles = block.CreateRandomObstacles();

            EventBus<GameStart>.Raise(default);
        }

        private void OnGameReset() {
            BlocksResetPositions();
            foreach (Block b in _blocks) b.DestroyObstacles();
        }

        private void OnPlayerFellOver() {
            EventBus<GameOver>.Raise(
                new GameOver {
                    Score = _scoreTracker.Score,
                    Distance = _scoreTracker.DistanceTravelled,
                    RunTime = _scoreTracker.RunTime,
                }
            );
        }

        private void OnLevelIncrease() {
            laneObstacleSpawnChance += 0.025f;
        }

        /// <summary>
        /// Instantiates all blocks and sets values
        /// </summary>
        private void BlocksInitialize() {
            for (var i = 0; i < blocksNumToCreate; i++) {
                Block blockNew = Instantiate(blockPrefab);
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
            foreach (Block block in _blocks) {
                block.transform.position = block.transform.position.With(
                    z: block.transform.position.z - GetTrackSpeed() * Time.deltaTime
                );
            }
        }

        /// <summary>
        /// If the block's Z is beyond or equal to the threshold, move it in front of the most recently moved block
        /// </summary>
        private void BlocksMoveToFrontCheck() {
            foreach (Block block in _blocks) {
                if (block.transform.position.z > _blockMoveToFrontThreshold) continue;
                block.transform.position = block.transform.position.With(
                    z: _blocks[_blockMovedIndex].transform.position.z + BlockLength
                );

                _blockMovedIndex++;
                if (_blockMovedIndex >= _blocksNumCreated) _blockMovedIndex = 0;

                block.DestroyObstacles();
                block.obstacles = block.CreateRandomObstacles();
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
            _scoreTracker.AddDistance(GetTrackSpeed() * Time.deltaTime);
        }

        private float GetTrackSpeed() => minTrackSpeed + _pedalTracker.PedalPowerRatio * maxSpeedBonus;

        private void LevelUpdate() {
            if (Math.Floor(_scoreTracker.RunTime.TotalSeconds / levelLength) > LevelCurrent) {
                LevelCurrent++;
                Log.Debug("[GameManager] Level Increased to: " + LevelCurrent);
                EventBus<LevelIncreaseEvent>.Raise(
                    new LevelIncreaseEvent {
                        Level = LevelCurrent,
                    }
                );
            }
        }
    }
}
