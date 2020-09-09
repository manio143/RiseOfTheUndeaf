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
        // TODO: figure out how to serialize bindings and present them to the user

        /// <summary>
        /// A set of default bindings for testing.
        /// </summary>
        public VirtualButtonConfig DefaultButtonBindings()
        {
            return new VirtualButtonConfig
            {
                // Horizontal movement X axis
                new VirtualButtonBinding(GameButton.MovementHorizontal,
                    new VirtualButtonTwoWay(VirtualButton.Keyboard.A, VirtualButton.Keyboard.D)),
                new VirtualButtonBinding(GameButton.MovementHorizontal,
                    new VirtualButtonTwoWay(VirtualButton.Keyboard.Left, VirtualButton.Keyboard.Right)),
                new VirtualButtonBinding(GameButton.MovementHorizontal,
                    new VirtualJoystick(VirtualButton.GamePad.LeftThumbAxisX, JoystickDeadZone)),
                
                // Vertical movement Y axis
                new VirtualButtonBinding(GameButton.MovementVertical,
                    new VirtualButtonTwoWay(VirtualButton.Keyboard.S, VirtualButton.Keyboard.W)),
                new VirtualButtonBinding(GameButton.MovementVertical,
                    new VirtualButtonTwoWay(VirtualButton.Keyboard.Down, VirtualButton.Keyboard.Up)),
                new VirtualButtonBinding(GameButton.MovementVertical,
                    new VirtualJoystick(VirtualButton.GamePad.LeftThumbAxisY, JoystickDeadZone)),

                // Jumping
                new VirtualButtonBinding(GameButton.Jump, VirtualButton.Keyboard.Space),
                new VirtualButtonBinding(GameButton.Jump, VirtualButton.GamePad.A),

                // Horizontal camera X axis
                new VirtualButtonBinding(GameButton.CameraHorizontal,
                    new VirtualMouseAxis(VirtualButton.Mouse.DeltaX, mouseSensitivity: MouseSensitivity)),
                new VirtualButtonBinding(GameButton.CameraHorizontal,
                    new VirtualJoystick(VirtualButton.GamePad.RightThumbAxisX, JoystickDeadZone)),

                // Vertical camera Y axis
                new VirtualButtonBinding(GameButton.CameraVertical,
                    new VirtualMouseAxis(VirtualButton.Mouse.DeltaY, negative: true, mouseSensitivity: MouseSensitivity)),
                new VirtualButtonBinding(GameButton.CameraVertical,
                    new VirtualJoystick(VirtualButton.GamePad.RightThumbAxisY, JoystickDeadZone)),
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
