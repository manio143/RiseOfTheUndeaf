using Stride.Core;
using Stride.Input;

namespace RiseOfTheUndeaf.Input
{
    [DataContract]
    public class InputSettings
    {
        /// <summary>
        /// Minimum distance the joystick has to be moved for it to be registered.
        /// </summary>
        public float JoystickDeadZone { get; set; } = 0.25f;

        /// <summary>
        /// Multiplies mouse movement by this amount to apply aim rotations.
        /// </summary>
        public float MouseSensitivity { get; set; } = 100.0f;

        /// <summary>
        /// Bindigs of <see cref="GameButton"/>s to keyboard and gamepad.
        /// </summary>
        public VirtualButtonConfig ButtonBindings { get; set; }

        /// <summary>
        /// A set of default bindings for testing.
        /// </summary>
        public static VirtualButtonConfig DefaultButtonBindings()
        {
            return new VirtualButtonConfig
            {
                // Horizontal movement X axis
                new VirtualButtonBinding(GameButton.MovementHorizontal,
                    new VirtualButtonTwoWay(VirtualButton.Keyboard.A, VirtualButton.Keyboard.D)),
                new VirtualButtonBinding(GameButton.MovementHorizontal,
                    new VirtualButtonTwoWay(VirtualButton.Keyboard.Left, VirtualButton.Keyboard.Right)),
                new VirtualButtonBinding(GameButton.MovementHorizontal, VirtualButton.GamePad.LeftThumbAxisX),
                
                // Vertical movement Y axis
                new VirtualButtonBinding(GameButton.MovementVertical,
                    new VirtualButtonTwoWay(VirtualButton.Keyboard.S, VirtualButton.Keyboard.W)),
                new VirtualButtonBinding(GameButton.MovementVertical,
                    new VirtualButtonTwoWay(VirtualButton.Keyboard.Down, VirtualButton.Keyboard.Up)),
                new VirtualButtonBinding(GameButton.MovementVertical, VirtualButton.GamePad.LeftThumbAxisY),

                // Jumping
                new VirtualButtonBinding(GameButton.Jump, VirtualButton.Keyboard.Space),
                new VirtualButtonBinding(GameButton.Jump, VirtualButton.GamePad.A),

                // Horizontal camera X axis
                new VirtualButtonBinding(GameButton.CameraHorizontal, VirtualButton.Mouse.DeltaX),
                new VirtualButtonBinding(GameButton.CameraHorizontal, VirtualButton.GamePad.RightThumbAxisX),

                // Vertical camera Y axis
                new VirtualButtonBinding(GameButton.CameraVertical, VirtualButton.Mouse.DeltaY),
                new VirtualButtonBinding(GameButton.CameraHorizontal, VirtualButton.GamePad.RightThumbAxisY),
            };
        }
    }

    /// <summary>
    /// Virtual button definitions for the game.
    /// </summary>
    public enum GameButton
    {
        Undefined = 0,

        MovementHorizontal,
        MovementVertical,

        Jump,

        CameraHorizontal,
        CameraVertical,
    }
}
