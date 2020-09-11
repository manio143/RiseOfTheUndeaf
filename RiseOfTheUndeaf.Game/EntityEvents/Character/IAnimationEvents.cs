namespace RiseOfTheUndeaf.EntityEvents.Character
{
    public interface IAnimationEvents : IEntityEvent
    {
        void SetGrounded(bool grounded);
        void SetRunSpeed(float speed);
    }
}
