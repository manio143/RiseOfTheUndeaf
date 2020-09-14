namespace RiseOfTheUndeaf.EntityEvents.Character
{
    public interface IDamageEvents : IEntityEvent
    {
        /// <summary>
        /// Decrease character's life by <paramref name="power"/>.
        /// </summary>
        /// <remarks>The decrease may be subject to other effects.</remarks>
        void DealDamage(int power);

        /// <summary>
        /// Increase character's life by <paramref name="amount"/>.
        /// </summary>
        /// <remarks>The increase may be subject to other effects.</remarks>
        void Heal(int amount);
    }
}
