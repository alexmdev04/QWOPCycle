using System;
using QWOPCycle.Gameplay;
using SideFX.Events;
using Unity.Logging;
using Unity.Mathematics;
using UnityEngine;

namespace QWOPCycle.Scoring {
    /// <summary>
    /// A Project level asset that keeps track of score.
    /// Is ticked from <see cref="GameManager"/>
    /// </summary>
    [CreateAssetMenu(fileName = "ScoreTracker", menuName = "QWOPCycle/ScoreTracker")]
    public sealed class ScoreTracker : ScriptableObject {
        public double Score { get; private set; }
        public float DistanceTravelled { get; private set; }

        public TimeSpan RunTime
            => _isGameRunning
                   ? DateTime.Now - _startTime
                   : _endTime - _startTime;

        private EventBinding<StartGameEvent> _startGameBinding;
        private EventBinding<GameOverEvent> _gameOverBinding;
        private DateTime _startTime;
        private DateTime _endTime;
        private bool _isGameRunning;


        private void OnEnable() {
            _startGameBinding = new EventBinding<StartGameEvent>(OnStartGame);
            _gameOverBinding = new EventBinding<GameOverEvent>(OnGameOver);
            EventBus<StartGameEvent>.Register(_startGameBinding);
            EventBus<GameOverEvent>.Register(_gameOverBinding);
        }

        private void OnDisable() {
            EventBus<StartGameEvent>.Deregister(_startGameBinding);
            EventBus<GameOverEvent>.Deregister(_gameOverBinding);
        }

        public void Tick(float deltaTime) {
            Score += deltaTime * (DistanceTravelled / (float)RunTime.TotalSeconds);
        }

        public void AddDistance(float distance) {
            if (!_isGameRunning) return;
            DistanceTravelled += distance;
        }

#region EventHandlers

        /// <summary>
        /// Reset score to 0 when gameplay starts
        /// </summary>
        private void OnStartGame() {
            Log.Verbose("ScoreTracker : Resetting");
            Score = 0;
            DistanceTravelled = 0f;
            _startTime = DateTime.Now;
            _isGameRunning = true;
        }

        private void OnGameOver(GameOverEvent e) {
            _endTime = DateTime.Now;
            _isGameRunning = false;
        }

#endregion
    }
}
