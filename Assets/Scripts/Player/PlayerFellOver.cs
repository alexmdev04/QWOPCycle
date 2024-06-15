using SideFX.Events;

namespace QWOPCycle.Player {
    // Notify all game systems that the player fell over
    // Usage: EventBus<PlayerFellOver>.Raise(default);
    public readonly struct PlayerFellOver : IEvent { }
}
