using SideFX.Events;
using SideFX.SceneManagement;
using SideFX.SceneManagement.Events;
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
        private EventBinding<SceneReady> _sceneReadyBinding;

        private void OnEnable() {
            _scoreBinding = new EventBinding<ScoreEvent>(OnScore);
            _sceneReadyBinding = new EventBinding<SceneReady>(OnSceneReady);
            EventBus<ScoreEvent>.Register(_scoreBinding);
        }

        private void OnDisable() {
            EventBus<ScoreEvent>.Deregister(_scoreBinding);
            EventBus<SceneReady>.Deregister(_sceneReadyBinding);
        }

        public void AddDistance(float distance) => DistanceTravelled += distance;

#region EventHandlers

        private void OnScore(ScoreEvent e) {
            Score += e.Value;
            Log.Verbose("ScoreTracker : Score increased by {0}, new score: {1}", e.Value, Score);
        }

        /// <summary>
        /// Reset score to 0 when gameplay starts
        /// </summary>
        private void OnSceneReady(SceneReady e) {
            if (e.Scene is not GameplayScene) return;

            Log.Verbose("ScoreTracker : Resetting");
            Score = 0;
            DistanceTravelled = 0f;
        }

#endregion
    }
}
