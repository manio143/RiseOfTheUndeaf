using RiseOfTheUndeaf.Input.Buttons;
using Stride.Core;
using Stride.Input;
using System.Collections.Generic;

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
        /// Invert mouse movement on the X axis.
        /// </summary>
        public bool InvertMouseX { get; set; }

        /// <summary>
        /// Invert mouse movement on the Y axis.
        /// </summary>
        public bool InvertMouseY { get; set; }

        /// <summary>
        /// Mapping of <see cref="GameButton"/> to physical input methods.
        /// </summary>
        public Dictionary<GameButton, List<IPhysicalButton>> ButtonBindings { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="VirtualButtonConfig"/> from the <see cref="ButtonBindings"/>.
        /// </summary>
        public VirtualButtonConfig CreateVirtualButtonConfig()
        {
            var config = new VirtualButtonConfig();

            foreach(var kvp in ButtonBindings)
            {
                var virtualButtonName = kvp.Key;
                foreach (var physicalButton in kvp.Value)
                    config.Add(new VirtualButtonBinding(virtualButtonName, physicalButton.ToVirtual(this)));
            }

            return config;
        }
    }
}
