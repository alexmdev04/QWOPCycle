using Assets.PlayerScripts;
using QWOPCycle.Gameplay;
using QWOPCycle.Player;
using SideFX.Events;
using SideFX.SceneManagement;
using SideFX.SceneManagement.Events;
using Unity.Logging;
using Unity.Mathematics;
using UnityEngine;

namespace QWOPCycle.Scoring {
    [CreateAssetMenu(fileName = "PedalTracker", menuName = "QWOPCycle/PedalTracker")]
    public sealed class PedalTracker : ScriptableObject {
        [field: Range(0f, 1f)] public float debugPedalPowerRatio;
        public float PedalPower { get; private set; }

        [Header("Starting (OnSceneReady) Values")] [SerializeField]
        private float pedalPowerIncrement = 1f;

        [SerializeField] private float pedalPowerDecay = 0.1f;
        [SerializeField] private float maxPedalPower = 10f;
        [SerializeField] private float _levelIncreaseDecay = 0.2f;
        [SerializeField] private float _levelIncreaseMaxPower = 1f;
        [SerializeField] private InputReader _input;

        private bool _gameIsRunning;
        private EventBinding<GameStart> _gameStartBinding;
        private EventBinding<LevelIncreaseEvent> _levelIncreaseBinding;
        private PedalState _state = PedalState.None;
        private float _pedalPowerIncrement = 1f;
        private float _pedalPowerDecay = 0.1f;
        private float _maxPedalPower = 10f;
        public float MaxPedalPower => _maxPedalPower;

        /// <summary>
        /// Value from 0.0 - 1.0 representing how much power is being generated
        /// </summary>
        public float PedalPowerRatio => PedalPower / _maxPedalPower;

        private enum PedalState {
            None, // Used when the game starts, so the player can start with either pedal
            Left,
            Right,
        }

        private void Awake() {
            Log.Debug("[PedalTracker] Awake");
            _input = CreateInstance<InputReader>();
        }


        private void OnEnable() {
            _input.PedalLeftEvent += OnPedalLeft;
            _input.PedalRightEvent += OnPedalRight;

            _gameStartBinding = new EventBinding<GameStart>(OnGameStart);
            EventBus<GameStart>.Register(_gameStartBinding);

            _levelIncreaseBinding = new EventBinding<LevelIncreaseEvent>(OnLevelIncrease);
            EventBus<LevelIncreaseEvent>.Register(_levelIncreaseBinding);
        }

        private void OnDisable() {
            _input.PedalLeftEvent -= OnPedalLeft;
            _input.PedalRightEvent -= OnPedalRight;
            EventBus<GameStart>.Deregister(_gameStartBinding);
            EventBus<LevelIncreaseEvent>.Deregister(_levelIncreaseBinding);
        }

        public void Tick(float deltaTime) {
            debugPedalPowerRatio = PedalPowerRatio;
            PedalPower -= _pedalPowerDecay * deltaTime;
            PedalPower = math.clamp(PedalPower, 0f, _maxPedalPower);
        }

        private void OnGameStart(GameStart e) {
            _gameIsRunning = true;
            PedalPower = _maxPedalPower;
            _pedalPowerDecay = pedalPowerDecay;
            _pedalPowerIncrement = pedalPowerIncrement;
            _maxPedalPower = maxPedalPower;
            Log.Debug("[PedalTracker] Starting");
        }

        private void OnPedalLeft() {
            if (_gameIsRunning && _state is not PedalState.Left) {
                _state = PedalState.Left;
                PedalPower += _pedalPowerIncrement;
            }
        }

        private void OnPedalRight() {
            if (_gameIsRunning && _state is not PedalState.Right) {
                _state = PedalState.Right;
                PedalPower += _pedalPowerIncrement;
            }
        }

        private void OnLevelIncrease() {
            _pedalPowerDecay += _levelIncreaseDecay; // require more pedalling
            _maxPedalPower += _levelIncreaseMaxPower; // increase overall speed
            Log.Debug(
                "[PedalTracker.OnLevelIncrease] Level Increased; decay: "
                + _pedalPowerDecay
                + " maxPedalPower: "
                + _maxPedalPower
            );
        }
    }
}
