namespace RiseOfTheUndeaf.EntityEvents.Character
{
    public interface IAnimationEvents : IEntityEvent
    {
        void Punch();
        void SetGrounded(bool grounded);
        void SetRunSpeed(float speed);
    }
}
