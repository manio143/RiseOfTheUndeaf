using Stride.Core;
using Stride.Input;
using System;

namespace RiseOfTheUndeaf.Input.Buttons
{
    /// <summary>
    /// A description of an axis from the gamepad.
    /// </summary>
    [DataContract]
    public class GamePadAxis : IPhysicalButton
    {
        public enum PadAxis { LeftX, LeftY, RightX, RightY, LeftTrigger, RightTrigger }

        /// <summary>
        /// Represented gamepad axis.
        /// </summary>
        public PadAxis Axis { get; set; }

        /// <inheritdoc/>
        public IVirtualButton ToVirtual(InputSettings settings)
        {
            switch (Axis)
            {
                case PadAxis.LeftX:
                    return new VirtualJoystick(VirtualButton.GamePad.LeftThumbAxisX, settings.JoystickDeadZone);
                case PadAxis.LeftY:
                    return new VirtualJoystick(VirtualButton.GamePad.LeftThumbAxisY, settings.JoystickDeadZone);
                case PadAxis.RightX:
                    return new VirtualJoystick(VirtualButton.GamePad.RightThumbAxisX, settings.JoystickDeadZone);
                case PadAxis.RightY:
                    return new VirtualJoystick(VirtualButton.GamePad.RightThumbAxisY, settings.JoystickDeadZone);
                case PadAxis.LeftTrigger:
                    return VirtualButton.GamePad.LeftTrigger;
                case PadAxis.RightTrigger:
                    return VirtualButton.GamePad.RightTrigger;
                default:
                    throw new InvalidOperationException($"Provided value ({Axis}) has no virtual button representation.");
            }
        }
    }
}
