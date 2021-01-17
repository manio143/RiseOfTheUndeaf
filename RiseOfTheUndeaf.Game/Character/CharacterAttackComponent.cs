using RiseOfTheUndeaf.EntityEvents.Character;
using RiseOfTheUndeaf.Weapons;
using Stride.Core;
using Stride.Engine;
using Stride.Engine.Design;

namespace RiseOfTheUndeaf.Character
{
    [Display("Character Attack")]
    [DataContract]
    [DefaultEntityComponentProcessor(typeof(CharacterAttackProcessor))]
    public class CharacterAttackComponent : EntityComponent, IAttackEvents
    {
        internal enum AttackState
        {
            None, Primary, Secondary
        }

        internal AttackState State { get; set; }

        public bool CanExecute { get; set; } = true;

        public IAttackHandler PrimaryHandler { get; set; }
        public IAttackHandler SecondaryHandler { get; set; }

        public void PrimaryAttack() => State = AttackState.Primary;
        public void SecondaryAttack() => State = AttackState.Secondary;
    }
}
