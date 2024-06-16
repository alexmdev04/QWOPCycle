using SideFX.Events;

namespace QWOPCycle.Player {
    // Notify all game systems that the level has increased
    // Usage: EventBus<PlayerFellOver>.Raise(default);
    public readonly struct LevelIncrease : IEvent { }
}
