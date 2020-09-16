using RiseOfTheUndeaf.Character;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using System;
using System.Threading.Tasks;

namespace RiseOfTheUndeaf.EntityControllers
{
    [Display("Zombie Spawner")]
    public class ZombieSpawner : AsyncScript
    {
        private static int zombieCounter = 0;
        private static object counterLock = new object();

        /// <summary>
        /// Number of zombies spawned per minute.
        /// </summary>
        public float SpawnRate { get; set; }

        /// <summary>
        /// Prefab of the zombie to be spawned.
        /// </summary>
        public Prefab ZombiePrefab { get; set; }

        /// <summary>
        /// Target for zombies to follow.
        /// </summary>
        public Entity PlayerCharacter { get; set; }

        public override async Task Execute()
        {
            while (true)
            {
                if (SpawnRate == 0)
                {
                    await Script.NextFrame();
                    continue;
                }

                var spawnNext = TimeSpan.FromSeconds(60 / SpawnRate);
                await Task.Delay(spawnNext);

                var zombie = ZombiePrefab.Instantiate()[0];
                zombie.Add(new FollowerComponent { Target = PlayerCharacter });
                zombie.Get<TransformComponent>().Position = Entity.Transform.Position + Vector3.UnitY;

                lock (counterLock)
                {
                    zombie.Name = String.Format("{0}_{1:000}", zombie.Name, zombieCounter);
                    zombieCounter++;
                }

                Entity.Scene.Entities.Add(zombie);
            }
        }
    }
}
