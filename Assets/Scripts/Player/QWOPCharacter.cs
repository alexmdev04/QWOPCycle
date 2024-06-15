using System;
using System.Collections;
using System.Collections.Generic;
using Assets.PlayerScripts;
using QWOPCycle;
using SideFX.Anchors;
using Unity.Logging;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace QWOPCycle.Gameplay {
    [RequireComponent(typeof(BalanceComponent))]
    [RequireComponent(typeof(SteerComponent))]
    public class QWOPCharacter : MonoBehaviour {
        #region System

        [Header("Controller / Input Reader")] public InputReader inputReader;
        [Header("Game Manager")] public GameManagerAnchor gameManagerAnchor;
        public Rigidbody RigidBody { get; private set; }
        public BalanceComponent BalanceComponent { get; private set; }
        public SteerComponent SteerComponent { get; private set; }

#endregion
        #region Vars

        [Header("Settings")]
        public bool canMove = true;
        public float bikeWidth = 0.05f;

#endregion
        #region Initialisation

        void Awake() {
            SetupPlayer();
        }

        private void SetupPlayer() {
            RigidBody = GetComponent<Rigidbody>();
            BalanceComponent = GetComponent<BalanceComponent>();
            BalanceComponent.SetCharacter(this);
            BalanceComponent.CanMove = canMove;
            SteerComponent = GetComponent<SteerComponent>();
            SteerComponent.SetCharacter(this);
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
