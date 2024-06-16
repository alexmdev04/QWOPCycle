using Assets.PlayerScripts;
using SideFX.Anchors;
using SideFX.Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace QWOPCycle.Gameplay {
    [RequireComponent(typeof(BalanceComponent))]
    [RequireComponent(typeof(SteerComponent))]
    [RequireComponent(typeof(CharacterSFXComponent))]
    public class QWOPCharacter : MonoBehaviour {
#region System

        [Header("Controller / Input Reader")] public InputReader inputReader;
        [Header("Game Manager")] public GameManagerAnchor gameManagerAnchor;
        public Rigidbody RigidBody { get; private set; }
        public BalanceComponent BalanceComponent { get; private set; }
        public SteerComponent SteerComponent { get; private set; }
        public CharacterSFXComponent CharacterSfxComponent { get; private set; }

        private EventBinding<GameReset> _gameStartBinding;

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
            CharacterSfxComponent = GetComponent<CharacterSFXComponent>();
        }

        private void Start() {
            SteerComponent.SetCharacter(this);
            BalanceComponent.SetCharacter(this);
        }

        private void Update() {
            if (Keyboard.current.fKey.wasPressedThisFrame) {
                SteerComponent.DebugFreeze();
                RigidBody.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        private void OnGameReset(GameReset e) {
            transform.position = Vector3.up * 0.5f;
            transform.rotation = Quaternion.identity;
            BalanceComponent.CanMove = true;
            CharacterSfxComponent.CanPlaySfx = true;
        }

#endregion

#region Bind/Unbind

        private void OnEnable() {
            BindController();
            _gameStartBinding = new EventBinding<GameReset>(OnGameReset);
            EventBus<GameReset>.Register(_gameStartBinding);
        }

        private void OnDisable() {
            UnbindController();
            EventBus<GameReset>.Deregister(_gameStartBinding);
        }

        private void BindController() {
            inputReader.BalanceLeftEvent += ControllerBalanceLeft;
            inputReader.BalanceRightEvent += ControllerBalanceRight;
            inputReader.PedalLeftEvent += ControllerPedalLeft;
            inputReader.PedalRightEvent += ControllerPedalRight;
        }

        private void UnbindController() {
            inputReader.BalanceLeftEvent -= ControllerBalanceLeft;
            inputReader.BalanceRightEvent -= ControllerBalanceRight;
            inputReader.PedalLeftEvent -= ControllerPedalLeft;
            inputReader.PedalRightEvent -= ControllerPedalRight;
        }
        private void ControllerBalanceLeft() {
            BalanceComponent.BalanceLeft();
            CharacterSfxComponent.PlayBalanceSfx();
        }
        private void ControllerBalanceRight() {
            BalanceComponent.BalanceRight();
            CharacterSfxComponent.PlayBalanceSfx();
        }
        private void ControllerPedalLeft() {
            CharacterSfxComponent.PlayPedalSfx();
        }
        private void ControllerPedalRight() {
            CharacterSfxComponent.PlayPedalSfx();
        }
#endregion
    }
}
