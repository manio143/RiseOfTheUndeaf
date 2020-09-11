using RiseOfTheUndeaf.EntityEvents;
using RiseOfTheUndeaf.EntityEvents.Character;
using Stride.Core.Annotations;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Games;
using Stride.Physics;
using System;
using System.Linq;

namespace RiseOfTheUndeaf.Character
{
    public class CharacterMovementProcessor : EntityProcessor<CharacterMovementComponent, CharacterMovementProcessor.Data>
    {
        public class Data
        {
            public CharacterComponent Character { get; set; }
            public TransformComponent ModelEntityTransform { get; set; }
            public float YawOrientation { get; set; }
        }

        public CharacterMovementProcessor() : base(typeof(CharacterComponent))
        { }

        protected override Data GenerateComponentData([NotNull] Entity entity, [NotNull] CharacterMovementComponent component)
        {
            return new Data
            {
                Character = entity.Get<CharacterComponent>(),
                ModelEntityTransform = entity.GetChildren()
                    .First(e => e.Name == "CharacterModel").Get<TransformComponent>(),
            };
        }

        public override void Update(GameTime time)
        {
            foreach (var kvp in ComponentDatas)
            {
                var movementComponent = kvp.Key;
                var data = kvp.Value;

                if (movementComponent.MoveDirection.HasValue)
                {
                    Move(movementComponent, data, movementComponent.MoveDirection.Value);
                    movementComponent.MoveDirection = null;
                }
                if (data.Character.IsGrounded)
                {
                    data.ModelEntityTransform.Entity.BroadcastEvent<IAnimationEvents>().SetGrounded(true);
                }
                if (movementComponent.ShouldJump)
                {
                    Jump(movementComponent, data);
                    movementComponent.ShouldJump = false;
                }
            }
        }

        private void Jump(CharacterMovementComponent movementComponent, Data data)
        {
            if (data.Character.IsGrounded)
            {
                data.Character.Jump();
                data.ModelEntityTransform.Entity.BroadcastEvent<IAnimationEvents>().SetGrounded(false);
            }
        }

        private void Move(CharacterMovementComponent component, Data data, Vector3 moveDirection)
        {
            // Allow very simple inertia to the character to make animation transitions more fluid
            component.LastMoveDirection = component.LastMoveDirection * 0.85f + moveDirection * 0.15f;

            data.Character.SetVelocity(component.LastMoveDirection * component.MovementSpeed);

            data.ModelEntityTransform.Entity.BroadcastEvent<IAnimationEvents>().SetRunSpeed(component.LastMoveDirection.Length());

            // Character orientation
            if (moveDirection.Length() > 0.001)
            {
                data.YawOrientation = (float)Math.Atan2(-moveDirection.Z, moveDirection.X) + MathUtil.PiOverTwo;
            }
            data.ModelEntityTransform.Rotation = Quaternion.RotationYawPitchRoll(data.YawOrientation, 0, 0);
        }
    }
}
