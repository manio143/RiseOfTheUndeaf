using Stride.Core;
using Stride.Core.Annotations;
using Stride.Core.Collections;
using Stride.Core.Diagnostics;
using Stride.Games;
using System.Collections.Generic;
using System.Linq;

namespace RiseOfTheUndeaf.GameEvents
{
    public sealed class GameEventSystem : GameSystemBase
    {
        private FastCollection<GameEvent> eventLog = new FastCollection<GameEvent>();
        private int eventsProcessed = 0;
        private HashSet<GameEventListener> listeners = new HashSet<GameEventListener>();

        private static ILogger logger = GlobalLogger.GetLogger(nameof(GameEventSystem));

        static GameEventSystem()
        {
            (logger as Logger).ActivateLog(LogMessageType.Debug);
        }

        public GameEventSystem([NotNull] IServiceRegistry registry) : base(registry)
        {
            Enabled = true; // required for Update to be called by GameSystemsCollection
            logger.Debug("GameEventSystem has been created.");
        }

        /// <inheritdoc/>
        public override void Update(GameTime gameTime)
        {
            while (eventsProcessed < eventLog.Count)
            {
                var gameEvent = eventLog[eventsProcessed++];

                logger.Info($"Processing event {gameEvent.GetType().Name} at {gameEvent.TimeStamp} => {gameEvent.ToLogString()}.");

                foreach (var listener in listeners)
                {
                    listener.ProcessEvent(gameEvent);
                }
            }
        }

        /// <summary>
        /// Append <paramref name="gameEvent"/> to the log.
        /// </summary>
        /// <param name="gameEvent">New event</param>
        public void Log(GameEvent gameEvent)
        {
            lock (eventLog)
            {
                gameEvent.TimeStamp = Game.UpdateTime.Total;
                eventLog.Add(gameEvent);
                logger.Debug($"Logged new event: {gameEvent.GetType().Name}.");
            }
        }

        /// <summary>
        /// Register <paramref name="listener"/> in the list of game event listeners.
        /// </summary>
        /// <param name="listener"></param>
        public void RegisterListener(GameEventListener listener)
        {
            lock (listeners)
            {
                // remove existing listener of the same type
                var removed = listeners.Remove(listener);
                if (removed)
                    logger.Debug($"Removed existing listener: {listener.GetType().Name}.");

                listeners.Add(listener);
                logger.Debug($"New listener registered: {listener.GetType().Name}.");
            }
        }

        /// <summary>
        /// Get <see cref="GameEventSystem"/> from the service registry or adds one if it doesn't exist.
        /// </summary>
        public static GameEventSystem GetFromServices(IServiceRegistry services)
        {
            var ges = services.GetService<GameEventSystem>();
            if (ges == null)
            {
                ges = new GameEventSystem(services);
                services.AddService<GameEventSystem>(ges);
                services.GetService<IGame>().GameSystems.Add(ges);
            }
            return ges;
        }
    }
}
