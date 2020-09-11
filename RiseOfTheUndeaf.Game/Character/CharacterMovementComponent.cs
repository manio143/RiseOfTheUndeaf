using RiseOfTheUndeaf.EntityEvents.Character;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Design;
using System.ComponentModel;

namespace RiseOfTheUndeaf.Character
{
    [DataContract]
    [DefaultEntityComponentProcessor(typeof(CharacterMovementProcessor))]
    public class CharacterMovementComponent : EntityComponent, IMovementEvents
    {
        [DefaultValue(10f)]
        public float MovementSpeed { get; set; }

        [DataMemberIgnore]
        public Vector3 LastMoveDirection { get; set; } = Vector3.Zero;

        internal Vector3? MoveDirection { get; set; }
        /// <inheritdoc/>
        public void Move(Vector3 moveDirection) => MoveDirection = moveDirection;

        /// <summary>
        /// If true the character tries to jump.
        /// </summary>
        internal bool ShouldJump { get; set; }
        /// <inheritdoc/>
        public void Jump() => ShouldJump = true;
    }
}
