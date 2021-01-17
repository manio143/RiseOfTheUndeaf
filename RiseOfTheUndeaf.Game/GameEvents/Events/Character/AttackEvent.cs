using Stride.Engine;

namespace RiseOfTheUndeaf.GameEvents.Events.Character
{
    public class AttackEvent : GameEvent<Entity>
    {
        protected AttackEvent(Entity executor) => Context = executor;
    }

    public class PrimaryAttackEvent : AttackEvent
    {
        public PrimaryAttackEvent(Entity executor) : base(executor) { }
        public override string ToLogString() => $"Primary Attack by {{{Context}}}";
    }

    public class SecondaryAttackEvent : AttackEvent
    {
        public SecondaryAttackEvent(Entity executor) : base(executor) { }
        public override string ToLogString() => $"Secondary Attack by {{{Context}}}";
    }
}
