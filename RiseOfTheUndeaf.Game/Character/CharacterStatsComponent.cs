using RiseOfTheUndeaf.EntityEvents.Character;
using Stride.Core;
using Stride.Engine;

namespace RiseOfTheUndeaf.Character
{
    [DataContract("CharacterStats")]
    [Display("Character Stats")]
    public class CharacterStatsComponent : EntityComponent, IDamageEvents
    {
        /// <summary>
        /// Current level of health.
        /// </summary>
        public int Health { get; set; }

        /// <summary>
        /// Maximal level of health.
        /// </summary>
        public int MaxHealth { get; set; }

        /// <summary>
        /// Damage dealt to this character in the current frame (see <see cref="CharacterStatsProcessor"/>).
        /// </summary>
        internal int DamageDealt { get; set; }
        /// <summary>
        /// Amount healed of this character in the current frame (see <see cref="CharacterStatsProcessor"/>).
        /// </summary>
        internal int AmountHealed { get; set; }

        /// <inheritdoc/>
        public void DealDamage(int power) => DamageDealt += power;

        /// <inheritdoc/>
        public void Heal(int amount) => AmountHealed += amount;
    }
}
