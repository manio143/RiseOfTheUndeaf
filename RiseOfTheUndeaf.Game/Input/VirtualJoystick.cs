using Stride.Input;
using System;

namespace RiseOfTheUndeaf.Input
{
    /// <summary>
    /// Applies a deadzone to joystick input (if in deadzone, returns 0).
    /// </summary>
    public class VirtualJoystick : IVirtualButton
    {
        private float deadZone;
        private IVirtualButton joystick;

        public VirtualJoystick(IVirtualButton joystick, float deadZone = 0f)
        {
            this.joystick = joystick;
            this.deadZone = deadZone;
        }

        public float GetValue(InputManager manager)
        {
            var value = joystick.GetValue(manager);
            if (Math.Abs(value) < deadZone)
                return 0f;
            return value;
        }

        public bool IsDown(InputManager manager) => false;
        public bool IsPressed(InputManager manager) => false;
        public bool IsReleased(InputManager manager) => false;
    }
}
