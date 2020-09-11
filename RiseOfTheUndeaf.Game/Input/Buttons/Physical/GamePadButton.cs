using Stride.Core;
using Stride.Input;
using System;
using GPButton = Stride.Input.GamePadButton;

namespace RiseOfTheUndeaf.Input.Buttons
{
    /// <summary>
    /// A description of a key from the gamepad.
    /// </summary>
    [DataContract]
    public class GamePadButton : IPhysicalButton
    {
        /// <summary>
        /// Represented gamepad button.
        /// </summary>
        public GPButton Button { get; set; }

        /// <inheritdoc/>
        public IVirtualButton ToVirtual(InputSettings settings)
        {
            switch(Button)
            {
                case GPButton.A: return VirtualButton.GamePad.A;
                case GPButton.B: return VirtualButton.GamePad.B;
                case GPButton.X: return VirtualButton.GamePad.X;
                case GPButton.Y: return VirtualButton.GamePad.Y;
                case GPButton.Start: return VirtualButton.GamePad.Start;
                case GPButton.Back: return VirtualButton.GamePad.Back;
                case GPButton.LeftShoulder: return VirtualButton.GamePad.LeftShoulder;
                case GPButton.RightShoulder: return VirtualButton.GamePad.RightShoulder;
                case GPButton.LeftThumb: return VirtualButton.GamePad.LeftThumb;
                case GPButton.RightThumb: return VirtualButton.GamePad.RightThumb;
                case GPButton.PadUp: return VirtualButton.GamePad.PadUp;
                case GPButton.PadDown: return VirtualButton.GamePad.PadDown;
                case GPButton.PadLeft: return VirtualButton.GamePad.PadLeft;
                case GPButton.PadRight: return VirtualButton.GamePad.PadRight;
                default:
                    throw new InvalidOperationException($"Provided value ({Button}) has no virtual button representation.");
            }
        }
    }
}
