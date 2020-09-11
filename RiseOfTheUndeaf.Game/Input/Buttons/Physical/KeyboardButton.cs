using Stride.Core;
using Stride.Input;

namespace RiseOfTheUndeaf.Input.Buttons
{
    /// <summary>
    /// A description of a key from the keyboard.
    /// </summary>
    [DataContract]
    public class KeyboardButton : IPhysicalButton, IVirtualButton
    {
        /// <summary>
        /// Represented keyboard button.
        /// </summary>
        public Keys Button { get; set; }

        public float GetValue(InputManager manager) => IsDown(manager) ? 1.0f : 0.0f;
        public bool IsDown(InputManager manager) => manager.IsKeyDown(Button);
        public bool IsPressed(InputManager manager) => manager.IsKeyPressed(Button);
        public bool IsReleased(InputManager manager) => manager.IsKeyReleased(Button);

        /// <inheritdoc/>
        public IVirtualButton ToVirtual(InputSettings settings) => this;
    }
}
