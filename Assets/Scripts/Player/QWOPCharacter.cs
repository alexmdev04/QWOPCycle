using Assets.PlayerScripts;
using SideFX.Anchors;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QWOPCycle.Gameplay {
    [RequireComponent(typeof(BalanceComponent))]
    [RequireComponent(typeof(SteerComponent))]
    public class QWOPCharacter : MonoBehaviour {
#region System

        [Header("Controller / Input Reader")] public InputReader inputReader;
        [Header("Game Manager")] public GameManagerAnchor gameManagerAnchor;
        public Rigidbody RigidBody { get; private set; }
        public BalanceComponent BalanceComponent { get; private set; }
        public SteerComponent SteerComponent { get; private set; }

#endregion

#region Vars

        [Header("Settings")] public bool canMove = true;

        public float bikeWidth = 0.05f;

#endregion

#region Initialisation

        private void Awake() {
            RigidBody = GetComponent<Rigidbody>();
            BalanceComponent = GetComponent<BalanceComponent>();
            SteerComponent = GetComponent<SteerComponent>();
        }

        private void Start() {
            SteerComponent.SetCharacter(this);
            BalanceComponent.SetCharacter(this);
            BalanceComponent.CanMove = canMove;
        }

        private void Update() {
            if (Keyboard.current.fKey.wasPressedThisFrame) {
                SteerComponent.DebugFreeze();
                RigidBody.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

#endregion

#region Bind/Unbind

        private void OnEnable() {
            BindController();
        }

        private void OnDisable() {
            UnbindController();
        }

        private void BindController() {
            inputReader.BalanceLeftEvent += BalanceComponent.BalanceLeft;
            inputReader.BalanceRightEvent += BalanceComponent.BalanceRight;
        }

        private void UnbindController() {
            inputReader.BalanceLeftEvent -= BalanceComponent.BalanceLeft;
            inputReader.BalanceRightEvent -= BalanceComponent.BalanceRight;
        }

#endregion
    }
}
