using System;
using System.Collections;
using System.Collections.Generic;
using Assets.PlayerScripts;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(BalanceComponent))]
public class QWOPCharacter : MonoBehaviour
{
    #region System
    [Header("Controller / Input Reader")]
    public InputReader inputReader;
    private Rigidbody _rigidBody;
    private BalanceComponent _balanceComponent;
    #endregion
    #region Vars
    [Header("Settings")] 
    public bool canMove = true;
    #endregion
    #region Initialisation
    void Awake()
    {
        SetupPlayer();
    }

    private void SetupPlayer()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _balanceComponent = GetComponent<BalanceComponent>();
        _balanceComponent.CanMove = canMove;
    }
    #endregion
    #region Bind/Unbind

    private void OnEnable()
    {
        BindController();
    }

    private void OnDisable()
    {
        UnbindController();
    }

    private void BindController()
    {
        inputReader.BalanceLeftEvent += _balanceComponent.BalanceLeft;
        inputReader.BalanceRightEvent += _balanceComponent.BalanceRight;
    }

    private void UnbindController()
    {
        inputReader.BalanceLeftEvent -= _balanceComponent.BalanceLeft;
        inputReader.BalanceRightEvent -= _balanceComponent.BalanceRight;
    }

    #endregion
    
}
