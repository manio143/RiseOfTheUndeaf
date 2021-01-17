using System;
using System.Threading.Tasks;
using RiseOfTheUndeaf.Core;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Processors;
using Stride.Physics;
using Stride.Rendering;

namespace RiseOfTheUndeaf.Weapons
{
    [DataContract]
    public class BulletShot : IAttackHandler
    {
        public float InitialForce { get; set; }

        /// <summary>
        /// Time to reload in seconds.
        /// </summary>
        public float ReloadTime { get; set; }

        public float Size { get; set; }

        public Model Model { get; set; }

        public CameraComponent Camera { get; set; }


        public async Task ExecuteAttack(Entity executor, Action<bool> canExecute, IServiceRegistry services)
        {
            canExecute(false);
            var script = services.GetService<ScriptSystem>();
            var reloadTime = DateTime.Now.AddSeconds(ReloadTime);

            CreateProjectile(executor);

            while (DateTime.Now < reloadTime)
                await script.NextFrame();

            canExecute(true);
        }

        private void CreateProjectile(Entity executor)
        {
            var direction = Utils.LogicDirectionToWorldDirection(Vector2.UnitY, Camera, Vector3.UnitY);
            direction.Normalize();

            var projectile = new Entity();

            var transform = projectile.Get<TransformComponent>();
            transform.Scale = new Vector3(Size);
            transform.Position = executor.Transform.Position + Vector3.UnitY + direction * 0.2f;
            
            var rigidBody = projectile.GetOrCreate<RigidbodyComponent>();
            rigidBody.ColliderShapes.Add(new BoxColliderShapeDesc
            {
                Size = new Vector3(Size),
            });
            rigidBody.IsKinematic = false;
            rigidBody.Gravity = Vector3.Zero;
            rigidBody.OverrideGravity = true;

            var model = projectile.GetOrCreate<ModelComponent>();
            model.Model = Model;

            projectile.Add(new BulletHitScript());

            executor.Scene.Entities.Add(projectile);

            rigidBody.ApplyForce(direction * InitialForce);
        }
    }
}
