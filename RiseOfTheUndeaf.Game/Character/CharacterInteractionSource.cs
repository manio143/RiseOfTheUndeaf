using Stride.Core;
using Stride.Engine;
using System.Threading.Tasks;

namespace RiseOfTheUndeaf.Character
{
    [Display("Character Interaction Source")]
    public class CharacterInteractionSource : AsyncScript
    {
        public CharacterInteractionComponent InteractionParent { get; set; }

        public override async Task Execute()
        {
            var physics = Entity.Get<PhysicsComponent>();

            while(true)
            {
                var collision = await physics.NewCollision();

                var collider = collision.ColliderA == physics ? collision.ColliderB : collision.ColliderA;
                InteractionParent.InterractWith(collider.Entity);
            }
        }
    }
}
