using RiseOfTheUndeaf.GameEvents;
using Stride.Engine;
using Stride.Games;
using Stride.Core.Mathematics;
using RiseOfTheUndeaf.GameEvents.Events.Character;

namespace RiseOfTheUndeaf.Character
{
    public class CharacterStatsProcessor : EntityProcessor<CharacterStatsComponent>
    {
        private GameEventSystem gameEventSystem;

        public override void Update(GameTime time)
        {
            if (gameEventSystem == null)
                gameEventSystem = GameEventSystem.GetFromServices(Services);

            foreach (var kvp in ComponentDatas)
            {
                var stats = kvp.Key;

                var oldHealth = stats.Health;
                var newHealth = MathUtil.Clamp(oldHealth - stats.DamageDealt + stats.AmountHealed, 0, stats.MaxHealth);

                if (oldHealth != newHealth)
                {
                    stats.Health = newHealth;
                    gameEventSystem.Log(new HealthChangeEvent(stats.Entity, oldHealth, newHealth));
                }

                ResetStatsDiff(stats);
            }
        }

        private static void ResetStatsDiff(CharacterStatsComponent stats)
        {
            stats.AmountHealed = 0;
            stats.DamageDealt = 0;
        }
    }
}
