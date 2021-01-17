namespace RiseOfTheUndeaf.EntityEvents.Character
{
    public interface IAttackEvents : IEntityEvent
    {
        void PrimaryAttack();

        void SecondaryAttack();
    }
}
