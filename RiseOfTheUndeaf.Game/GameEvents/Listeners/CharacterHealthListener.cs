using RiseOfTheUndeaf.Core.Logging;
using RiseOfTheUndeaf.GameEvents.Events.Character;

namespace RiseOfTheUndeaf.GameEvents.Listeners
{
    public class CharacterHealthListener : GameEventListener
    {
        public override void ProcessEvent(GameEvent gameEvent)
        {
            var healthChange = gameEvent as HealthChangeEvent;
            if (healthChange == null)
                return;

            if (healthChange.Context.CurrentHealth == 0)
            {
                if (healthChange.Context.Character.Name == "PlayerCharacter")
                {
                    this.LogWarning("Game over!"); // TODO: handle game over
                }
                else
                {
                    // TODO: make this more customizable
                    healthChange.Context.Character.Scene.Entities.Remove(healthChange.Context.Character);
                }
            }
        }
    }
}
