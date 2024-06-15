using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(Rigidbody))]
public class BalanceComponent : MonoBehaviour
{
    #region Component References
    private Rigidbody _rigidBody;
    #endregion
    #region Vars
    [Header("Balance Physics")]
    public float minBalanceForce = 100f;
    public float maxBalanceForce = 1000f;
    public Vector3 balancePivotPoint = new Vector3(0.0f, 0.0f, 1.0f);
    public Vector3 balancePivotOffset;
    public AnimationCurve balanceCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float fallAngleThreshold = 45.0f;
    public float steeringForce = 5.0f;
    public float maxSpeedWhenTilting = 0.5f;
    [FormerlySerializedAs("_canMove")]
    [Header("Settings")]
    private bool _canMove = true;
    public bool enableDebug = false;
    #endregion
    #region Getter Setters
    public bool CanMove
    {
        get => _canMove;
        set => _canMove = value;
    }
    #endregion
    #region Delegates
    public event Action FellOverEvent = delegate { };
    #endregion
    #region Intialisation
    private void Awake()
    {
        SetupBalanceComponent();
    }
    private void SetupBalanceComponent()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }
    #endregion
    #region Balance Logic
    public void BalanceRight()
    {
        if (HasFallenOver() && _canMove)
        {
            HandleFellOver();
        }

        if (!_canMove) return;
        ApplyTorque(-CalcPowerLevel);
        if (!enableDebug) return;
        Debug.Log("QWOPCharacter : Trying to balance right!");
    }
    public void BalanceLeft()
    {
        if (HasFallenOver() && _canMove)
        {
            HandleFellOver();
        }

        if (!_canMove) return;
        ApplyTorque(CalcPowerLevel);
        if (!enableDebug) return;
        Debug.Log("Balance Component : Trying to balance left!");
    }

    private void HandleFellOver()
    {
        FellOverEvent?.Invoke();
        _canMove = false;
        _rigidBody.ResetInertiaTensor();
    }

    private float CalcPowerLevel
    {
        get
        {
            var normalisedTiltAngle = Mathf.InverseLerp(0, fallAngleThreshold, AbsoluteTiltAngle);
            var curveValue = balanceCurve.Evaluate(normalisedTiltAngle);
            var balanceForceOut = math.lerp(minBalanceForce, maxBalanceForce, curveValue);
            if (enableDebug)
            {
                Debug.Log($"Balance Component : Balance force is {balanceForceOut}");
            }
            return balanceForceOut;
        }
    }

    private void ApplyTorque(float power)
    {
        var torque = balancePivotPoint * power;
        var torquePosition = transform.position + balancePivotOffset;
        var finalTorque = torque + torquePosition;
        _rigidBody.AddTorque(finalTorque, ForceMode.Impulse);
        if (!enableDebug) return;
        Debug.Log($"Balance Component : Applied {finalTorque} units of torque.");
    }

    private bool HasFallenOver()
    {
        return AbsoluteTiltAngle > fallAngleThreshold;
    }
    private float AbsoluteTiltAngle => Math.Abs(Vector3.Angle(Vector3.up, transform.up)); //provides absolute angle only.
    public float TiltAngle => Vector3.Angle(Vector3.up, transform.up); //Provides negative and positive angles.

    #endregion
    #region Fixed Updates
    private void FixedUpdate()
    {
        if (_canMove)
        {
            ApplySteeringForce();
        }
    }

    private void ApplySteeringForce()
    {
        //Determine the direction and magnitude of the steering force
        float steeringDirection = Vector3.Dot(transform.right, Vector3.up) > 0 ? -1 : 1;
        float appliedSteeringForce = steeringForce * AbsoluteTiltAngle * steeringDirection * maxSpeedWhenTilting;
        
        //Apply the steering force | locked on the X axis only.
        _rigidBody.AddForce(Vector3.right * appliedSteeringForce, ForceMode.Acceleration);
    }

    #endregion
}
