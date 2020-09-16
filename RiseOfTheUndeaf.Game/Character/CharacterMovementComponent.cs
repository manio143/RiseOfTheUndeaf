using RiseOfTheUndeaf.EntityEvents.Character;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Design;
using System.ComponentModel;

namespace RiseOfTheUndeaf.Character
{
    /// <summary>
    /// Character component responsible for character movement.
    /// </summary>
    [DataContract("CharacterMovement")]
    [Display("Character Movement")]
    [DefaultEntityComponentProcessor(typeof(CharacterMovementProcessor))]
    public class CharacterMovementComponent : EntityComponent, IMovementEvents
    {
        /// <summary>
        /// How fast should the character move (top speed).
        /// </summary>
        public float MovementSpeed { get; set; } = 10f;

        /// <summary>
        /// The direction this character was last moving in (see <see cref="CharacterMovementProcessor"/>).
        /// </summary>
        internal Vector3 LastMoveDirection { get; set; } = Vector3.Zero;

        /// <summary>
        /// The direction this character should move in, in the current frame (see <see cref="CharacterMovementProcessor"/>).
        /// </summary>
        internal Vector3? MoveDirection { get; set; }
        /// <inheritdoc/>
        public void Move(Vector3 moveDirection) => MoveDirection = moveDirection;

        /// <summary>
        /// If true the character tries to jump (see <see cref="CharacterMovementProcessor"/>).
        /// </summary>
        internal bool ShouldJump { get; set; }
        /// <inheritdoc/>
        public void Jump() => ShouldJump = true;
    }
}
