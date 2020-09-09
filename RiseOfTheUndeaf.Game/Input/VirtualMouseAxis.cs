using Stride.Input;

namespace RiseOfTheUndeaf.Input
{
    /// <summary>
    /// Applies sensitivity correction and negation to <see cref="VirtualButton.Mouse.DeltaX"/> or <see cref="VirtualButton.Mouse.DeltaY"/>.
    /// </summary>
    public class VirtualMouseAxis : IVirtualButton
    {
        private float mouseSensitivity;
        private bool negative;
        private IVirtualButton mouseAxis;

        public VirtualMouseAxis(IVirtualButton mouseAxis, bool negative = false, float mouseSensitivity = 100f)
        {
            this.mouseAxis = mouseAxis;
            this.mouseSensitivity = mouseSensitivity;
            this.negative = negative;
        }

        public float GetValue(InputManager manager)
        {
            var value = mouseAxis.GetValue(manager) * mouseSensitivity;
            return negative ? -value : value;
        }

        public bool IsDown(InputManager manager) => false;
        public bool IsPressed(InputManager manager) => false;
        public bool IsReleased(InputManager manager) => false;
    }
}
