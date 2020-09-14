using System;
using Stride.Games;

namespace RiseOfTheUndeaf.GameEvents
{
    /// <summary>
    /// A logical event that occured in the game.
    /// </summary>
    public abstract class GameEvent
    {
        /// <summary>
        /// When the event occured (based on <see cref="GameTime.Total"/>).
        /// </summary>
        public TimeSpan TimeStamp { get; internal set; }

        /// <summary>
        /// String representation of the event's data for loggging purposes.
        /// </summary>
        public virtual string ToLogString() => ToString();
    }

    public abstract class GameEvent<T> : GameEvent
    {
        /// <summary>
        /// Additional event context.
        /// </summary>
        public T Context { get; set; }
    }
}
