using Stride.Input;

namespace RiseOfTheUndeaf.Input.Buttons
{
    /// <summary>
    /// Represents a physical button of either the keyboard or a gamepad.
    /// </summary>
    public interface IPhysicalButton
    {
        /// <summary>
        /// Returns a virtual button corresponding to this physical button.
        /// </summary>
        IVirtualButton ToVirtual(InputSettings settings);
    }
}
