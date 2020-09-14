using RiseOfTheUndeaf.GameEvents.Events.Character;
using Stride.Core.Diagnostics;

namespace RiseOfTheUndeaf.GameEvents.Listeners
{
    public class CharacterHealthListener : GameEventListener
    {
        private static ILogger logger = GlobalLogger.GetLogger(nameof(CharacterHealthListener));
        public override void ProcessEvent(GameEvent gameEvent)
        {
            var healthChange = gameEvent as HealthChangeEvent;
            if (healthChange == null)
                return;

            if (healthChange.Context.CurrentHealth == 0)
            {
                if (healthChange.Context.Character.Name == "PlayerCharacter")
                {
                    logger.Warning("Game over!"); // TODO: handle game over
                }
                else
                {
                    // TODO: handle zombie death
                }
            }
        }
    }
}
