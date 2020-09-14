using RiseOfTheUndeaf.EntityEvents.Character;
using Stride.Core;
using Stride.Engine;
using Stride.Engine.Design;

namespace RiseOfTheUndeaf.Character
{
    [DataContract("CharacterPunch")]
    [Display("Character Punch")]
    [DefaultEntityComponentProcessor(typeof(CharacterPunchProcessor))]
    public class CharacterPunchComponent : EntityComponent, IMeleeAttackEvents
    {
        public PhysicsComponent InteractionSource { get; set; }

        internal bool ShouldPunch { get; set; }
        public void MeleeAttack() => ShouldPunch = true;
    }
}
