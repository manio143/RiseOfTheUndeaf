using System;
using System.Collections.Generic;
using System.Text;

namespace RiseOfTheUndeaf.EntityEvents.Character
{
    public interface IMeleeAttackEvents : IEntityEvent
    {
        public void MeleeAttack();
    }
}
