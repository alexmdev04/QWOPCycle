using Assets.PlayerScripts;
using SideFX.Events;
using SideFX.SceneManagement;
using SideFX.SceneManagement.Events;
using Unity.Logging;
using Unity.Mathematics;
using UnityEngine;

namespace QWOPCycle.Scoring {
    [CreateAssetMenu(fileName = "PedalTracker", menuName = "QWOPCycle/PedalTracker")]
    public sealed class PedalTracker : ScriptableObject {
        [field: SerializeField] public float PedalPower { get; private set; }
        public float MaxPedalPower => _maxPedalPower;

        /// <summary>
        /// Value from 0.0 - 1.0 representing how much power is being generated
        /// </summary>
        public float PedalPowerRatio => PedalPower / _maxPedalPower;

        [SerializeField] private float _pedalPowerIncrement = 1f;
        [SerializeField] private float _pedalPowerDecay = 0.1f;
        [SerializeField] private float _maxPedalPower = 10f;

        private bool _gameIsRunning;
        private EventBinding<SceneReady> _sceneReadyBinding;
        private InputReader _input;
        private PedalState _state = PedalState.None;

        private enum PedalState {
            None, // Used when the game starts, so the player can start with either pedal
            Left,
            Right,
        }

        private void Awake() {
            _input = CreateInstance<InputReader>();
        }


        private void OnEnable() {
            _input.PedalLeftEvent += OnPedalLeft;
            _input.PedalRightEvent += OnPedalRight;

            _sceneReadyBinding = new EventBinding<SceneReady>(OnSceneReady);
            EventBus<SceneReady>.Register(_sceneReadyBinding);
        }

        private void OnDisable() {
            _input.PedalLeftEvent -= OnPedalLeft;
            _input.PedalRightEvent -= OnPedalRight;
            EventBus<SceneReady>.Deregister(_sceneReadyBinding);
        }

        public void Tick(float deltaTime) {
            PedalPower -= _pedalPowerDecay * Time.fixedDeltaTime;
            PedalPower = math.clamp(PedalPower, 0f, _maxPedalPower);
        }

        private void OnSceneReady(SceneReady e) {
            if (e.Scene is GameplayScene) {
                _gameIsRunning = true;
                PedalPower = 0f;
                Log.Debug("[PedalPowerTracker] Starting");
            }
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
    }
}
