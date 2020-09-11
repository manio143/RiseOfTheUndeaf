using Stride.Core.Mathematics;

namespace RiseOfTheUndeaf.EntityEvents.Camera
{
    public interface ICameraEvents : IEntityEvent
    {
        /// <summary>
        /// Rotates the camera by an offset of <paramref name="direction"/>.
        /// </summary>
        void MoveBy(Vector2 direction);
    }
}
