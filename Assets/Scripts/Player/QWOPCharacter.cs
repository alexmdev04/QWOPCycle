using System;
using System.Collections;
using System.Collections.Generic;
using Assets.PlayerScripts;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class QWOPCharacter : MonoBehaviour
{
    #region System
    [Header("Controller / Input Reader")]
    public InputReader inputReader;
    private Rigidbody _rigidBody;
    #endregion
    #region Vars
    [Header("Physics")]
    public float balanceForce = 100f;
    public float maxBalanceForce = 1000f;
    public float fallAngleThreshold = 45.0f;
    public float steeringForce = 5.0f;
    public float maxSpeedWhenTilting = 0.5f;
    [Header("Settings")] 
    public bool canMove = true;
    #endregion
    #region Delegates
    public event Action FellOverEvent = delegate { };
    #endregion
    #region Initialisation
    void Awake()
    {
        SetupPlayer();
    }

    private void SetupPlayer()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }
    #endregion
    #region Bind/Unbind

    private void OnEnable()
    {
        EnableControllerHelper();
    }

    private void OnDisable()
    {
        DisableControllerHelper();
    }

    private void EnableControllerHelper()
    {
        inputReader.BalanceLeftEvent += BalanceLeft;
        inputReader.BalanceRightEvent += BalanceRight;
    }

    private void DisableControllerHelper()
    {
        inputReader.BalanceLeftEvent -= BalanceLeft;
        inputReader.BalanceRightEvent -= BalanceRight;
    }

    #endregion
    #region Balance Logic
    private void BalanceRight()
    {
        if (HasFallenOver() && canMove)
        {
            FellOverEvent?.Invoke();
            canMove = false;
        }

        if (!canMove) return;
        ApplyTorque(CalcPowerLevel());
        Debug.Log("QWOPCharacter : Trying to balance right!");
    }
    private void BalanceLeft()
    {
        if (HasFallenOver() && canMove)
        {
            FellOverEvent?.Invoke();
            canMove = false;
        }

        if (!canMove) return;
        ApplyTorque(-CalcPowerLevel());
        Debug.Log("QWOPCharacter : Trying to balance left!");
    }

    private float CalcPowerLevel()
    {
        float a = (math.abs(transform.rotation.z));
        float powerLevel = Mathf.InverseLerp(0, fallAngleThreshold, a);
        Debug.Log($"QWOPCharacter : Power level is {powerLevel}");
        return math.lerp(balanceForce, maxBalanceForce, powerLevel);
    }

    private void ApplyTorque(float power)
    {
        var newPower = power * transform.forward;
        _rigidBody.AddRelativeTorque(newPower, ForceMode.Force);
    }

    private bool HasFallenOver()
    {
        return GetTiltAngle() > fallAngleThreshold;
    }

    private float GetTiltAngle()
    {
        return Vector3.Angle(Vector3.up, transform.up);
    }

    private float GetNormalisedTilt()
    {
        return Mathf.Clamp(GetTiltAngle() / fallAngleThreshold, 0, 1);
    }
    #endregion
    #region Fixed Updates
    private void FixedUpdate()
    {
        if (canMove)
        {
            ApplySteeringForce();
        }
    }

    private void ApplySteeringForce()
    {
        //Determine the direction and magnitude of the steering force
        float steeringDirection = Vector3.Dot(transform.right, Vector3.up) > 0 ? -1 : 1;
        float appliedSteeringForce = steeringForce * GetNormalisedTilt() * steeringDirection * maxSpeedWhenTilting;
        
        //Apply the steering force | locked on the X axis only.
        _rigidBody.AddForce(Vector3.right * appliedSteeringForce, ForceMode.Acceleration);
    }

    #endregion
}
