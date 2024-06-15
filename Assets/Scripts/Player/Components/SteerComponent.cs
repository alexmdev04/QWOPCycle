using System;
using System.Collections;
using System.Collections.Generic;
using SideFX.Events;
using SideFX.SceneManagement.Events;
using Unity.Logging;
using UnityEngine;

namespace QWOPCycle.Gameplay {
    public class SteerComponent : MonoBehaviour {
        private QWOPCharacter _character;
        public float steeringMultiplier = 0.001f;
        public bool _canMove = true;

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
            _character.BalanceComponent.FellOverEvent += DisableMove;
        }

        private void DisableMove() {
            _canMove = false;
        }

        private void Update() {
            if (!_canMove) { return; }
            transform.position = transform.position.With(
                x: Mathf.Clamp(
                    transform.position.x - (_character.BalanceComponent.TiltAngle * steeringMultiplier),
                    -2.45f,
                    2.45f
                )
            );
        }
    }
}
