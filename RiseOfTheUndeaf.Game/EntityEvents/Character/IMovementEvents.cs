using Stride.Core.Mathematics;

namespace RiseOfTheUndeaf.EntityEvents.Character
{
    public interface IMovementEvents: IEntityEvent
    {
        /// <summary>
        /// Move by the <paramref name="moveDirection"/>.
        /// </summary>
        void Move(Vector3 moveDirection);

        /// <summary>
        /// Make the character jump.
        /// </summary>
        void Jump();
    }
}
