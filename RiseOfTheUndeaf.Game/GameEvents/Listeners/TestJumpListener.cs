using System;
using RiseOfTheUndeaf.Core.Logging;
using RiseOfTheUndeaf.GameEvents.Events.Character;

namespace RiseOfTheUndeaf.GameEvents.Listeners
{
    public class TestJumpListener : GameEventListener
    {
        private int consecutiveJumps = 1;
        private JumpEvent lastEvent;

        public override void ProcessEvent(GameEvent gameEvent)
        {
            var jump = gameEvent as JumpEvent;
            if (jump == null)
                return;

            if(lastEvent != null)
            {
                if (jump.TimeStamp - lastEvent.TimeStamp < TimeSpan.FromSeconds(1))
                    consecutiveJumps++;
                else
                    consecutiveJumps = 1;

            }

            if (consecutiveJumps >= 5)
                this.LogInfo("Jump combo: {consecutiveJumps} jumps.", consecutiveJumps);

            lastEvent = jump;
        }
    }
}
