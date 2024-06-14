using Assets.PlayerScripts;
using SideFX.Events;
using SideFX.SceneManagement;
using SideFX.SceneManagement.Events;
using Unity.Mathematics;
using UnityEngine;

namespace QWOPCycle.Scoring {
    public sealed class PedalPowerTracker : MonoBehaviour {
        [field: SerializeField] public float PedalPower { get; private set; }

        [SerializeField] private float _pedalPowerIncrement = 1f;
        [SerializeField] private float _pedalPowerDecay = 0.1f;
        [SerializeField] private float _maxPedalPower = 10f;

        [SerializeField] private InputReader _input;
        private EventBinding<SceneReady> _sceneReadyBinding;
        private bool _gameIsRunning;
        private PedalState _state = PedalState.None;

        private enum PedalState {
            None, // Used when the game starts, so the player can start with either pedal
            Left,
            Right,
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

        private void FixedUpdate() {
            if (_gameIsRunning && PedalPower > 0f) {
                PedalPower -= _pedalPowerDecay * Time.fixedDeltaTime;
                PedalPower = math.clamp(PedalPower, 0f, _maxPedalPower);
            }
        }

        private void OnSceneReady(SceneReady e) {
            _gameIsRunning = e.Scene is GameplayScene;
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
