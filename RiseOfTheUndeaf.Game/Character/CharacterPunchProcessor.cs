using RiseOfTheUndeaf.EntityEvents;
using RiseOfTheUndeaf.EntityEvents.Character;
using RiseOfTheUndeaf.GameEvents;
using RiseOfTheUndeaf.GameEvents.Events.Character;
using Stride.Core.Annotations;
using Stride.Engine;
using Stride.Games;
using System;

namespace RiseOfTheUndeaf.Character
{
    public class CharacterPunchProcessor : EntityProcessor<CharacterPunchComponent, CharacterPunchProcessor.Data>
    {
        public class Data
        {
            public TimeSpan? WaitTime { get; set; }
        }

        public CharacterPunchProcessor() : base(typeof(PhysicsComponent))
        { }

        protected override Data GenerateComponentData([NotNull] Entity entity, [NotNull] CharacterPunchComponent component)
        {
            return new Data
            {
            };
        }

        private GameEventSystem gameEventSystem;

        public override void Update(GameTime time)
        {
            if (gameEventSystem == null)
            {
                gameEventSystem = GameEventSystem.GetFromServices(Services);
            }

            foreach (var kvp in ComponentDatas)
            {
                var punchComponent = kvp.Key;
                var data = kvp.Value;

                if (punchComponent.ShouldPunch)
                {
                    // animate punch
                    punchComponent.Entity.BroadcastEvent<IAnimationEvents>().Punch();
                    data.WaitTime = TimeSpan.FromSeconds(0.2);
                }

                if (data.WaitTime.HasValue)
                {
                    if (data.WaitTime.Value > new TimeSpan())
                    {
                        data.WaitTime -= time.Elapsed;
                    }
                    else
                    {
                        gameEventSystem.Log(new PunchEvent(punchComponent.Entity));
                        foreach (var collision in punchComponent.InteractionSource.Collisions)
                        {
                            var otherEntity = collision.ColliderA.Entity == punchComponent.Entity
                                ? collision.ColliderB.Entity
                                : collision.ColliderA.Entity;
                            if (otherEntity.Name == "PlayerCharacter")
                                otherEntity.BroadcastEvent<IDamageEvents>().DealDamage(1);
                        }
                        data.WaitTime = null;
                    }
                }

                // reset state
                punchComponent.ShouldPunch = false;
            }
        }
    }
}
