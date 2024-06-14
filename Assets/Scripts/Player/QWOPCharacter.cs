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
    public float balanceForce = 10f;
    public float fallAngleThreshold = 45.0f;
    public float steeringForce = 5.0f;
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
        if (_rigidBody is null)
        {
            _rigidBody = GetComponent<Rigidbody>();
        }
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
        if (HasFallenOver())
        {
            FellOverEvent?.Invoke();
            canMove = false;
        }

        if (canMove)
        {
            Vector3 rightTilt = transform.forward;
            _rigidBody.AddTorque(rightTilt * balanceForce);
            _rigidBody.AddForce(Vector3.left * steeringForce, ForceMode.Acceleration);
            Debug.Log("QWOPCharacter : Trying to balance right!");
        }
    }

    private void BalanceLeft()
    {
        if (HasFallenOver())
        {
            FellOverEvent?.Invoke();
            canMove = false;
        }

        if (canMove)
        {
            Vector3 leftTilt = transform.forward * -1;
            _rigidBody.AddTorque(leftTilt * balanceForce);
            _rigidBody.AddForce(Vector3.right * steeringForce, ForceMode.Acceleration);
            Debug.Log("QWOPCharacter : Trying to balance left!");
        }
    }

    private bool HasFallenOver()
    {
        float tiltAngle = Vector3.Angle(Vector3.up, transform.up);
        return tiltAngle > fallAngleThreshold;
    }
    #endregion
}
