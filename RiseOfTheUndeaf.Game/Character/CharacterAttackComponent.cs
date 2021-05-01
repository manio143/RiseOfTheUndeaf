using RiseOfTheUndeaf.EntityEvents.Character;
using RiseOfTheUndeaf.Weapons;
using Stride.Core;
using Stride.Engine;
using Stride.Engine.Design;

namespace RiseOfTheUndeaf.Character
{
    [Display("Character Attack")]
    [DataContract]
    [DefaultEntityComponentProcessor(typeof(CharacterAttackProcessor), ExecutionMode = ExecutionMode.Runtime)]
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

        public void PrimaryAttack()
        {
            if (CanExecute)
                State = AttackState.Primary;
        }

        public void SecondaryAttack()
        {
            if (CanExecute)
                State = AttackState.Secondary;
        }
    }
}
