using System;
using QWOPCycle.Player;
using QWOPCycle.Scoring;
using SideFX.Events;
using Unity.Logging;
using Unity.Mathematics;
using UnityEngine;

namespace QWOPCycle.Gameplay {
    [RequireComponent(typeof(Rigidbody))]
    public class BalanceComponent : MonoBehaviour {
#region Component References

        //public Rigidbody _rigidBody;
        private QWOPCharacter _character;
        [SerializeField] private PedalTracker pedalTracker;

#endregion

#region Getter Setters

        [field: Header("Settings")] public bool CanMove { get; set; } = true;

#endregion

#region Intialisation

        public void SetCharacter(QWOPCharacter character) {
            _character = character;
        }

#endregion

#region Vars

        [Header("Balance Physics")] public float minBalanceForce = 100f;
        public float maxBalanceForce = 1000f;
        public Vector3 balancePivotPoint = new(0.0f, 0.0f, 1.0f);
        public Vector3 balancePivotOffset;
        public AnimationCurve balanceCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public float fallAngleThreshold = 45.0f;
        public float steeringForce = 5.0f;
        public float maxSpeedWhenTilting = 0.5f;
        public bool enableDebug;

#endregion

#region Delegates

        //public event Action FellOverEvent;

#endregion

#region Balance Logic

        public void BalanceRight() {
            if (!CanMove) {
                if (enableDebug) Log.Verbose("Balance Component : Can't move, ignoring BalanceRight action!");
                return;
            }

            ApplyTorque(-CalcPowerLevel());
            if (enableDebug) Log.Verbose("Balance Component : Trying to balance right!");
        }

        public void BalanceLeft() {
            if (!CanMove) {
                if (enableDebug) Log.Verbose("Balance Component : Can't move, ignoring BalanceLeft action!");
                return;
            }

            ApplyTorque(CalcPowerLevel());
            if (enableDebug) Log.Verbose("Balance Component : Trying to balance left!");
        }

        private void HandleFellOver() {
            Log.Info("Balance Component : Fell over!");
            EventBus<PlayerFellOver>.Raise(default);
            //FellOverEvent?.Invoke();
            CanMove = false;
            _character.RigidBody.ResetInertiaTensor();
        }

        private float CalcPowerLevel() {
            float normalisedTiltAngle = Mathf.InverseLerp(0, fallAngleThreshold, AbsoluteTiltAngle);
            float curveValue = balanceCurve.Evaluate(normalisedTiltAngle);
            float balanceForceOut = math.lerp(minBalanceForce, maxBalanceForce, curveValue);
            if (enableDebug) Debug.Log($"Balance Component : Balance force is {balanceForceOut}");

            return balanceForceOut;
        }

        private void ApplyTorque(float power) {
            Vector3 torque = balancePivotPoint * power;
            Vector3 torquePosition = transform.position + balancePivotOffset;
            Vector3 finalTorque = torque + torquePosition;
            _character.RigidBody.AddTorque(finalTorque, ForceMode.Impulse);
            if (!enableDebug) return;
            Debug.Log($"Balance Component : Applied {finalTorque} units of torque.");
        }

        private bool HasFallenOver() =>
            AbsoluteTiltAngle > fallAngleThreshold
            || _character.RigidBody.transform.position.x
            <= -_character.gameManagerAnchor.Value.BlockWidth / 2 - _character.bikeWidth
            || _character.RigidBody.transform.position.x
            >= _character.gameManagerAnchor.Value.BlockWidth / 2 + _character.bikeWidth;

        private float AbsoluteTiltAngle =>
            Math.Abs(Vector3.Angle(Vector3.up, transform.up)); //provides absolute angle only.

        public float TiltAngle =>
            transform.eulerAngles.z >= 180f
                ? transform.eulerAngles.z - 360f
                : transform.eulerAngles.z; //Provides negative and positive angles.

#endregion

#region Fixed Updates

        private void FixedUpdate() {
            if (!CanRun()) return;
            if (HasFallenOver() && CanMove) HandleFellOver();
            if (CanMove) ApplySteeringForce();
        }

        private bool CanRun() => _character.gameManagerAnchor.IsSet;

        private void ApplySteeringForce() {
            //Determine the direction and magnitude of the steering force
            float steeringDirection = Vector3.Dot(transform.right, Vector3.up) > 0 ? -1 : 1;
            float appliedSteeringForce = steeringForce * AbsoluteTiltAngle * steeringDirection * maxSpeedWhenTilting;

            //Apply the steering force | locked on the X axis only.
            _character.RigidBody.AddForce(
                Vector3.right
                * (appliedSteeringForce * Math.Max(pedalTracker.MaxPedalPower - pedalTracker.PedalPower, 1f)),
                ForceMode.Acceleration
            );
        }

#endregion
    }
}
