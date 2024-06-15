using QWOPCycle.Player;
using SideFX.Events;
using SideFX.SceneManagement.Events;
using UnityEngine;
using UnityEngine.Serialization;

namespace QWOPCycle.Gameplay {
    public class SteerComponent : MonoBehaviour {
        public float steeringMultiplier = 0.001f;
        public bool canMove = true;
        private QWOPCharacter _character;
        private EventBinding<PlayerFellOver> _playerFellOverBinding;

        private EventBinding<SceneReady> _sceneReadyBinding;

        private float PlayerXDelta => _character.BalanceComponent.TiltAngle * Time.deltaTime * steeringMultiplier;

        private void Update() {
            if (!canMove) return;
            transform.position = transform.position.With(transform.position.x - PlayerXDelta);
        }

        private void OnEnable() {
            _sceneReadyBinding = new EventBinding<SceneReady>(OnSceneReady);
            _playerFellOverBinding = new EventBinding<PlayerFellOver>(OnFellOver);
            EventBus<SceneReady>.Register(_sceneReadyBinding);
        }

        private void OnDisable() {
            EventBus<SceneReady>.Deregister(_sceneReadyBinding);
            EventBus<PlayerFellOver>.Deregister(_playerFellOverBinding);
        }

        public void SetCharacter(QWOPCharacter character) {
            _character = character;
        }

        private void OnSceneReady(SceneReady e) {
            canMove = true;
            _character.RigidBody.constraints = (RigidbodyConstraints)48 + 10; // freeze rot x and y, freeze pos x and z
        }

        private void OnFellOver() {
            canMove = false;
            _character.RigidBody.constraints = (RigidbodyConstraints)48 + 8; // freeze rot x and y, freeze pos z
            //_character.RigidBody.velocity = _character.RigidBody.velocity.With(x: -PlayerXDelta);
        }
    }
}
