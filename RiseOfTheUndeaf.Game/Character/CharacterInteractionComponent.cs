using RiseOfTheUndeaf.EntityEvents;
using RiseOfTheUndeaf.EntityEvents.Character;
using Stride.Core;
using Stride.Engine;

namespace RiseOfTheUndeaf.Character
{
    [DataContract("CharacterInteraction")]
    [Display("Character Interaction")]
    public class CharacterInteractionComponent : EntityComponent
    {
        public void InterractWith(Entity other)
        {
            if (other.Name == "PlayerCharacter")
                Entity.BroadcastEvent<IMeleeAttackEvents>().MeleeAttack();
        }
    }
}
