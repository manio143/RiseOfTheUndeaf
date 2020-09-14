using Stride.Engine;

namespace RiseOfTheUndeaf.GameEvents.Events.Character
{
    /// <summary>
    /// Signals a character has jumped.
    /// </summary>
    public class JumpEvent : GameEvent<Entity>
    {
        public JumpEvent(Entity character)
        {
            Context = character;
        }

        public override string ToLogString() => $"{{Entity: {Context.Name}}}";
    }
}
