using System;

namespace RiseOfTheUndeaf.GameEvents
{
    public abstract class GameEventListener : IEquatable<GameEventListener>
    {
        // Two listeners are considered equal if they are of the same type
        // for reference equality cast onto System.Object
        public bool Equals(GameEventListener other) => GetType() == other.GetType();
        public override int GetHashCode() => GetType().GetHashCode();

        public abstract void ProcessEvent(GameEvent gameEvent);
    }
}