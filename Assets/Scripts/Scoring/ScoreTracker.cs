using System;
using QWOPCycle.Gameplay;
using SideFX.Events;
using Unity.Logging;
using UnityEngine;

namespace QWOPCycle.Scoring {
    public readonly struct ScoreEvent : IEvent { // Usage: EventBus<ScoreEvent>.Raise(new ScoreEvent(5));
        public readonly uint Value;
        public ScoreEvent(uint value) => Value = value;
    }

    /// <summary>
    /// A Project level asset that keeps track of score.
    /// Responds to <see cref="ScoreEvent" />s sent via the event bus
    /// </summary>
    [CreateAssetMenu(fileName = "ScoreTracker", menuName = "QWOPCycle/ScoreTracker")]
    public sealed class ScoreTracker : ScriptableObject {
        public uint Score { get; private set; }
        public float DistanceTravelled { get; private set; }

        private EventBinding<ScoreEvent> _scoreBinding;
        private EventBinding<StartGameEvent> _startGameBinding;
        private EventBinding<GameOverEvent> _gameOverBinding;
        private DateTime _startTime;
        private DateTime _endTime;
        private bool _isGameRunning;


        private void OnEnable() {
            _scoreBinding = new EventBinding<ScoreEvent>(OnScore);
            _startGameBinding = new EventBinding<StartGameEvent>(OnStartGame);
            _gameOverBinding = new EventBinding<GameOverEvent>(OnGameOver);
            EventBus<ScoreEvent>.Register(_scoreBinding);
            EventBus<StartGameEvent>.Register(_startGameBinding);
            EventBus<GameOverEvent>.Register(_gameOverBinding);
        }

        private void OnDisable() {
            EventBus<ScoreEvent>.Deregister(_scoreBinding);
            EventBus<StartGameEvent>.Deregister(_startGameBinding);
            EventBus<GameOverEvent>.Deregister(_gameOverBinding);
        }

        public void AddDistance(float distance) {
            if (!_shouldTrack) return;
            DistanceTravelled += distance;
        }

#region EventHandlers

        private void OnScore(ScoreEvent e) {
            if (!_shouldTrack) return;
            Score += e.Value;
            Log.Verbose("ScoreTracker : Score increased by {0}, new score: {1}", e.Value, Score);
        }

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
