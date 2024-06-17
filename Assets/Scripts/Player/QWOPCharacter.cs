using Assets.PlayerScripts;
using QWOPCycle.Scoring;
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
        [Header("Animator")] public Animator playerAnimator;
        [Header("Pedal Tracker")] public PedalTracker pedalTracker;
        public Rigidbody RigidBody { get; private set; }
        public BalanceComponent BalanceComponent { get; private set; }
        public SteerComponent SteerComponent { get; private set; }
        public CharacterSFXComponent CharacterSfxComponent { get; private set; }

        private EventBinding<GameStart> _gameStartBinding;
        private EventBinding<GameReset> _gameResetBinding;

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
            playerAnimator.speed = pedalTracker.PedalPower / 2f;
#if UNITY_EDITOR
            if (Keyboard.current.fKey.wasPressedThisFrame) {
                SteerComponent.DebugFreeze();
                RigidBody.constraints = RigidbodyConstraints.FreezeAll;
            }
#endif
        }

        private void OnGameStart(GameStart e) {
            BalanceComponent.CanMove = true;
            CharacterSfxComponent.CanPlaySfx = true;
        }

        private void OnGameReset(GameReset e) {
            transform.position = Vector3.up * 0.5f;
            transform.rotation = Quaternion.identity;
        }

#endregion

#region Bind/Unbind

        private void OnEnable() {
            BindController();
            _gameStartBinding = new EventBinding<GameStart>(OnGameStart);
            _gameResetBinding = new EventBinding<GameReset>(OnGameReset);
            EventBus<GameStart>.Register(_gameStartBinding);
            EventBus<GameReset>.Register(_gameResetBinding);
        }

        private void OnDisable() {
            UnbindController();
            EventBus<GameStart>.Deregister(_gameStartBinding);
            EventBus<GameReset>.Deregister(_gameResetBinding);
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
