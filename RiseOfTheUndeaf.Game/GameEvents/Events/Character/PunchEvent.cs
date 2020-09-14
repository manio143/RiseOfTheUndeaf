using Stride.Engine;

namespace RiseOfTheUndeaf.GameEvents.Events.Character
{
    public class PunchEvent : GameEvent<Entity>
    {
        public PunchEvent(Entity puncher) => Context = puncher;

        public override string ToLogString() => $"{{Puncher: {Context.Name}}}";
    }
}
