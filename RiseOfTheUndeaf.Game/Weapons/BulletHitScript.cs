using System.Threading.Tasks;
using RiseOfTheUndeaf.EntityEvents;
using RiseOfTheUndeaf.EntityEvents.Character;
using Stride.Core;
using Stride.Engine;

namespace RiseOfTheUndeaf.Weapons
{
    [Display("Bullet Hit")]
    public class BulletHitScript : AsyncScript
    {
        public override async Task Execute()
        {
            var physics = Entity.Get<PhysicsComponent>();

            var collision = await physics.NewCollision();
            var colliderEntity = collision.ColliderA == physics
                ? collision.ColliderB.Entity
                : collision.ColliderA.Entity;

            colliderEntity.BroadcastEvent<IDamageEvents>().DealDamage(1);

            Entity.Scene.Entities.Remove(Entity);
        }
    }
}
