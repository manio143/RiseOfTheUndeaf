namespace RiseOfTheUndeaf.GameEvents
{
    public abstract class GameEventListener
    {
        public abstract void ProcessEvent(GameEvent gameEvent);
    }
}