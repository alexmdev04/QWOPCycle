using System;
using System.Collections;
using System.Collections.Generic;
using Assets.PlayerScripts;
using QWOPCycle;
using Unity.Logging;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace QWOPCycle.Gameplay {
    [RequireComponent(typeof(BalanceComponent))]
    public class QWOPCharacter : MonoBehaviour {
        #region System

        [Header("Controller / Input Reader")] public InputReader inputReader;
        public Rigidbody RigidBody { get; private set; }
        public BalanceComponent BalanceComponent { get; private set; }
        public SteerComponent SteerComponent { get; private set; }

#endregion
        #region Vars

        [Header("Settings")] public bool canMove = true;

#endregion
        #region Initialisation

        void Awake() {
            SetupPlayer();
        }

        private void SetupPlayer() {
            RigidBody = GetComponent<Rigidbody>();
            BalanceComponent = GetComponent<BalanceComponent>();
            BalanceComponent.CanMove = canMove;
            SteerComponent = GetComponent<SteerComponent>();
        }

#endregion
        #region Bind/Unbind

        private void OnEnable() {
            BindController();
        }

        private void OnDisable() {
            UnbindController();
        }

        private void BindController() {
            inputReader.BalanceLeftEvent += BalanceComponent.BalanceLeft;
            inputReader.BalanceRightEvent += BalanceComponent.BalanceRight;
        }

        private void UnbindController() {
            inputReader.BalanceLeftEvent -= BalanceComponent.BalanceLeft;
            inputReader.BalanceRightEvent -= BalanceComponent.BalanceRight;
        }

#endregion
    }
}
