using System;
using System.Threading.Tasks;
using Stride.Engine;

namespace RiseOfTheUndeaf.Weapons
{
    public interface IAttackHandler
    {
        /// <summary>
        /// Performs actions necessary to execute an attack.
        /// This may involve creating a projectile, changing animations, etc.
        /// </summary>
        /// <param name="executor">Entity which executes the attack.</param>
        /// <param name="canExecute">Callback to block attack execution on the calling component.</param>
        Task ExecuteAttack(Entity executor, Action<bool> canExecute);
    }
}
