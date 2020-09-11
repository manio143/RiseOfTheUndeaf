using Stride.Core;
using Stride.Input;
using System;

namespace RiseOfTheUndeaf.Input.Buttons
{
    /// <summary>
    /// A description of a mouse action.
    /// </summary>
    [DataContract]
    public class Mouse : IPhysicalButton
    {
        public enum MouseAction { Left, Right, Middle, DeltaX, DeltaY }

        /// <summary>
        /// Represented mouse action.
        /// </summary>
        public MouseAction Action { get; set; }

        /// <inheritdoc/>
        public IVirtualButton ToVirtual(InputSettings settings)
        {
            switch (Action)
            {
                case MouseAction.Left:
                    return VirtualButton.Mouse.Left;
                case MouseAction.Right:
                    return VirtualButton.Mouse.Right;
                case MouseAction.Middle:
                    return VirtualButton.Mouse.Middle;
                case MouseAction.DeltaX:
                    return new VirtualMouseAxis(VirtualButton.Mouse.DeltaX, negative: settings.InvertMouseX, mouseSensitivity: settings.MouseSensitivity);
                case MouseAction.DeltaY:
                    return new VirtualMouseAxis(VirtualButton.Mouse.DeltaY, negative: !settings.InvertMouseY, mouseSensitivity: settings.MouseSensitivity);
                default:
                    throw new InvalidOperationException($"Provided value ({Action}) has no virtual button representation.");
            }
        }
    }
}
