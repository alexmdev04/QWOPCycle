using SideFX.Events;
using UnityEngine;

namespace QWOPCycle.Scoring {
    public readonly struct ScoreEvent : IEvent {
        public readonly uint Value;
        public ScoreEvent(uint value) => Value = value;
    }

    [CreateAssetMenu(fileName = "ScoreTracker", menuName = "QWOPCycle/ScoreTracker")]
    public sealed class ScoreTracker : ScriptableObject {
        public uint Score { get; private set; }
        private EventBinding<ScoreEvent> _scoreBinding;

        private void OnEnable() {
            _scoreBinding = new EventBinding<ScoreEvent>(OnScore);
            EventBus<ScoreEvent>.Register(_scoreBinding);
        }

        private void OnDisable() => EventBus<ScoreEvent>.Deregister(_scoreBinding);

        private void OnScore(ScoreEvent e) => Score += e.Value;
    }
}
