using System;
using Unity.Logging;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.PlayerScripts {
    [CreateAssetMenu(fileName = "Input Reader", menuName = "InputReader/Inputs")]
    public class InputReader : ScriptableObject, Controls.IPlayActions {
#region InputReader Setup

        private Controls _controls;

        private void OnEnable() {
            InitController();
            _controls.Play.Enable();
        }

        private void OnDisable() {
            _controls.Play.Disable();
        }

        private void InitController() {
            _controls = _controls is null ? new Controls() : _controls;
            _controls.Play.SetCallbacks(this);
        }

#endregion

#region IPlayerControllerActions

        public event Action BalanceRightEvent = delegate { };
        public event Action BalanceLeftEvent = delegate { };
        public event Action PedalLeftEvent = delegate { };
        public event Action PedalRightEvent = delegate { };

        public void OnBalanceRight(InputAction.CallbackContext context) {
            if (context.performed) {
                Log.Verbose("Input Reader : Balance Right performed");
                BalanceRightEvent?.Invoke();
            }
        }

        public void OnBalanceLeft(InputAction.CallbackContext context) {
            if (context.performed) {
                Log.Verbose("Input Reader : Balance Left performed");
                BalanceLeftEvent?.Invoke();
            }
        }

        public void OnPedalLeft(InputAction.CallbackContext context) {
            if (context.performed) {
                Log.Verbose("Input Reader : Pedal Left performed");
                PedalLeftEvent?.Invoke();
            }
        }

        public void OnPedalRight(InputAction.CallbackContext context) {
            if (context.performed) {
                Log.Verbose("Input Reader : Pedal Right performed");
                PedalRightEvent?.Invoke();
            }
        }

#endregion
    }
}
