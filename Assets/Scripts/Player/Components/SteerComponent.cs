using System;
using System.Collections;
using System.Collections.Generic;
using SideFX.Anchors;
using SideFX.Events;
using SideFX.SceneManagement.Events;
using Unity.IO.LowLevel.Unsafe;
using Unity.Logging;
using UnityEngine;

namespace QWOPCycle.Gameplay {
    public class SteerComponent : MonoBehaviour {
        private QWOPCharacter _character;
        public float steeringMultiplier = 0.001f;
        public bool _canMove = true;
        [SerializeField] private GameManagerAnchor _gameManagerAnchor;

        private void Awake() {
            _character = GetComponent<QWOPCharacter>();
        }

        private EventBinding<SceneReady> _sceneReadyBinding;

        private void OnEnable() {
            _sceneReadyBinding = new EventBinding<SceneReady>(OnSceneReady);
            EventBus<SceneReady>.Register(_sceneReadyBinding);
        }

        private void OnDisable() {
            EventBus<SceneReady>.Deregister(_sceneReadyBinding);
        }

        private void OnSceneReady(SceneReady e) {
            _canMove = true;
            _character.RigidBody.constraints = (RigidbodyConstraints)48 + 10; // freeze rot x and y, freeze pos x and z
            _character.BalanceComponent.FellOverEvent += DisableMove;
        }

        private void DisableMove() {
            _canMove = false;
            _character.RigidBody.constraints = (RigidbodyConstraints) 48 + 8; // freeze rot x and y, freeze pos z
            //_character.RigidBody.velocity = _character.RigidBody.velocity.With(x: -PlayerXDelta);
        }

        private void Update() {
            if (!_canMove) { return; }
            transform.position = transform.position.With(
                x: Mathf.Clamp(
                    transform.position.x - PlayerXDelta,
                    (-_gameManagerAnchor.Value.blockWidth / 2) + _character.bikeWidth,
                    (_gameManagerAnchor.Value.blockWidth / 2) - _character.bikeWidth
                )
            );
        }

        private float PlayerXDelta => _character.BalanceComponent.TiltAngle * Time.deltaTime * steeringMultiplier;
    }
}
