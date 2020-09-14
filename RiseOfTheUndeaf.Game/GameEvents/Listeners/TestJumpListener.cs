using RiseOfTheUndeaf.GameEvents.Events.Character;
using Stride.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiseOfTheUndeaf.GameEvents.Listeners
{
    public class TestJumpListener : GameEventListener
    {
        private int consecutiveJumps = 1;
        private JumpEvent lastEvent;

        private static ILogger logger = GlobalLogger.GetLogger(nameof(TestJumpListener));

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
                logger.Info($"Jump combo: {consecutiveJumps} jumps.");

            lastEvent = jump;
        }
    }
}
