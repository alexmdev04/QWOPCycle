using System;
using System.Collections;
using System.Collections.Generic;
using Assets.PlayerScripts;
using UnityEngine;

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
    [Range(-1.0f, 0.0f)]
    public float minBalanceForceThreshold = -1.0f;
    [Range(0.0f, 1.0f)]
    public float maxBalanceForceThreshold = 1.0f;
    public float fallAngleThreshold = 45.0f;
    public float steeringForce = 5.0f;
    public float maxSpeedWhenTilting = 0.5f;
    [Header("Settings")] 
    public bool canMove = true;
    public Vector3 _pivotOffset;
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
        if (_rigidBody is null)
        {
            _rigidBody = GetComponent<Rigidbody>();
        }
        _pivotOffset = new Vector3(0, -_rigidBody.transform.localScale.y / 2, 0);
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
        var normalisedTiltAngle = GetNormalisedTilt();
        var rightTilt = transform.right;
        var normalisedMinBalanceForce = Mathf.Clamp(normalisedTiltAngle, minBalanceForceThreshold, maxBalanceForceThreshold);
        ApplyTorqueWithPivot(rightTilt * balanceForce * normalisedMinBalanceForce);
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
        var normalisedTiltAngle = GetNormalisedTilt();
        var leftTilt = transform.right * -1;
        var normalisedMinBalanceForce = Mathf.Clamp(normalisedTiltAngle, minBalanceForceThreshold, maxBalanceForceThreshold);
        ApplyTorqueWithPivot(leftTilt * balanceForce * normalisedMinBalanceForce);
        Debug.Log("QWOPCharacter : Trying to balance left!");
    }
    
    private void ApplyTorqueWithPivot(Vector3 force)
    {
        Vector3 pivotWorldPosition = _rigidBody.position + _pivotOffset;
        _rigidBody.AddForceAtPosition(force, pivotWorldPosition);
    }

    private bool HasFallenOver()
    {
        return GetTiltAngle() > fallAngleThreshold;
    }

    public float GetTiltAngle()
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
        float appliedSteeringForce = steeringForce * GetNormalisedTilt() * maxSpeedWhenTilting;
        
        //Apply the steering force | locked on the X axis only.
        _rigidBody.AddForce(Vector3.right * appliedSteeringForce, ForceMode.Acceleration);
    }

    #endregion
}
