using RiseOfTheUndeaf.EntityEvents;
using RiseOfTheUndeaf.EntityEvents.Character;
using Stride.Core.Annotations;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Games;
using Stride.Navigation;
using System;
using System.Collections.Generic;

namespace RiseOfTheUndeaf.Character
{
    public class FollowerProcessor : EntityProcessor<FollowerComponent, FollowerProcessor.Data>
    {
        public class Data
        {
            public NavigationComponent Navigation { get; set; }
            public TransformComponent Source { get; set; }
            public TransformComponent Target { get; set; }
            public Vector3 LastTargetPosition { get; set; }
            public List<Vector3> PathToTarget { get; set; }
        }

        protected override Data GenerateComponentData([NotNull] Entity entity, [NotNull] FollowerComponent component)
        {
            return new Data
            {
                Navigation = entity.Get<NavigationComponent>(),
                Source = component.Entity.Get<TransformComponent>(),
                Target = component.Target.Get<TransformComponent>(),
                PathToTarget = new List<Vector3>(),
            };
        }

        public FollowerProcessor() : base(typeof(NavigationComponent), typeof(CharacterMovementComponent))
        { }

        public override void Update(GameTime time)
        {
            foreach (var kvp in ComponentDatas)
            {
                var follower = kvp.Key;
                var data = kvp.Value;

                if (!follower.Enabled)
                    continue;

                data.Source.GetWorldTransformation(out var sourcePosition, out _, out _);
                data.Target.GetWorldTransformation(out var targetPosition, out _, out _);
                //sourcePosition.Y = 0;
                //targetPosition.Y = 0;

                if (targetPosition != data.LastTargetPosition)
                {
                    data.PathToTarget.Clear();
                    data.Navigation.TryFindPath(targetPosition, data.PathToTarget);
                    for (int i = 0; i < data.PathToTarget.Count; i++)
                        data.PathToTarget[i] = new Vector3(data.PathToTarget[i].X, 0, data.PathToTarget[i].Z);
                    data.LastTargetPosition = targetPosition;
                }

                var nearestTarget = GetNearestTarget(data.PathToTarget, sourcePosition);
                var direction = nearestTarget - sourcePosition;

                if (direction.Length() > 1)
                    direction.Normalize();
                follower.Entity.BroadcastEvent<IMovementEvents>().Move(direction);
            }
        }

        private static Vector3 GetNearestTarget(List<Vector3> pathList, Vector3 sourcePosition)
        {
            Vector3 nextTarget = sourcePosition;
            while (pathList.Count > 0)
            {
                nextTarget = pathList[0];

                if (pathList.Count > 1)
                {
                    if ((nextTarget - sourcePosition).Length() > 0.01f)
                        return nextTarget;
                }
                else
                {
                    if((nextTarget - sourcePosition).Length() > 0.82f)
                        return nextTarget;
                }

                pathList.RemoveAt(0);
            }

            return sourcePosition;
        }
    }
}
