using Stride.Engine;

namespace RiseOfTheUndeaf.GameEvents.Events.Character
{
    /// <summary>
    /// Signals a character has had a change in health.
    /// </summary>
    public class HealthChangeEvent : GameEvent<HealthChangeEvent.HealthChangeContext>
    {
        public struct HealthChangeContext
        {
            public Entity Character;
            public int PreviousHealth;
            public int CurrentHealth;
        }

        public HealthChangeEvent(Entity character, int oldHealth, int newHealth)
        {
            Context = new HealthChangeContext
            {
                Character = character,
                PreviousHealth = oldHealth,
                CurrentHealth = newHealth,
            };
        }

        public override string ToLogString()
            => $"{{Entity: {Context.Character.Name}, Health: {Context.PreviousHealth} -> {Context.CurrentHealth}}}";
    }
}
