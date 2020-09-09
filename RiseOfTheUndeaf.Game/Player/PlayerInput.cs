// Copyright (c) Stride contributors (https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using System.Collections.Generic;
using System.Linq;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Events;
using Stride.Input;
using RiseOfTheUndeaf.Core;
using RiseOfTheUndeaf.Input;

namespace RiseOfTheUndeaf.Player
{
    public class PlayerInput : SyncScript
    {
        /// <summary>
        /// Raised every frame with the intended direction of movement from the player.
        /// </summary>
        // TODO Should not be static, but allow binding between player and controller
        public static readonly EventKey<Vector3> MoveDirectionEventKey = new EventKey<Vector3>();

        public static readonly EventKey<Vector2> CameraDirectionEventKey = new EventKey<Vector2>();

        public static readonly EventKey<bool> JumpEventKey = new EventKey<bool>();
        private bool jumpButtonDown = false;


        public CameraComponent Camera { get; set; }

        public InputSettings InputSettings { get; set; }

        public override void Start()
        {
            //TODO: [Stride] bindings serialization
            if (InputSettings.ButtonBindings == null)
                InputSettings.ButtonBindings = InputSettings.DefaultButtonBindings();

            Input.VirtualButtonConfigSet = Input.VirtualButtonConfigSet ?? new VirtualButtonConfigSet();
            Input.VirtualButtonConfigSet.Add(InputSettings.ButtonBindings);
        }

        public override void Update()
        {
            // Character movement: should be camera-aware
            {
                var moveDirection = new Vector2
                {
                    X = Input.GetVirtualButton(0, GameButton.MovementHorizontal),
                    Y = Input.GetVirtualButton(0, GameButton.MovementVertical),
                };

                // Broadcast the movement vector as a world-space Vector3 to allow characters to be controlled
                var worldSpeed = (Camera != null)
                    ? Utils.LogicDirectionToWorldDirection(moveDirection, Camera, Vector3.UnitY)
                    : new Vector3(moveDirection.X, 0, moveDirection.Y);

                // Adjust vector's magnitute - worldSpeed has been normalized
                var moveLength = moveDirection.Length();
                var isDeadZoneLeft = moveLength < InputSettings.JoystickDeadZone;
                if (isDeadZoneLeft)
                {
                    worldSpeed = Vector3.Zero;
                }
                else
                {
                    if (moveLength > 1)
                    {
                        moveLength = 1;
                    }
                    else
                    {
                        moveLength = (moveLength - InputSettings.JoystickDeadZone) / (1f - InputSettings.JoystickDeadZone);
                    }

                    worldSpeed *= moveLength;
                }

                MoveDirectionEventKey.Broadcast(worldSpeed);
            }

            // Camera rotation: left-right rotates the camera horizontally while up-down controls its altitude
            {
                //TODO: use virtual buttons data

                // Right stick: camera rotation
                var cameraDirection = Input.GetRightThumbAny(InputSettings.JoystickDeadZone);
                var isDeadZoneRight = cameraDirection.Length() < InputSettings.JoystickDeadZone;
                if (isDeadZoneRight)
                    cameraDirection = Vector2.Zero;
                else
                    cameraDirection.Normalize();

                // Mouse-based camera rotation. Only enabled after you click the screen to lock your cursor, pressing escape cancels this
                if (Input.IsMouseButtonDown(MouseButton.Left))
                {
                    Input.LockMousePosition(true);
                    Game.IsMouseVisible = false;
                }
                if (Input.IsKeyPressed(Keys.Escape))
                {
                    Input.UnlockMousePosition();
                    Game.IsMouseVisible = true;
                }
                if (Input.IsMousePositionLocked)
                {
                    cameraDirection += new Vector2(Input.MouseDelta.X, -Input.MouseDelta.Y) * InputSettings.MouseSensitivity;
                }

                // Broadcast the camera direction directly, as a screen-space Vector2
                CameraDirectionEventKey.Broadcast(cameraDirection);
            }

            // Jumping: don't bother with jump restrictions here, just pass the button states
            {
                var isJumpDown = Input.GetVirtualButton(0, GameButton.Jump) > 0;
                var didJump = (!jumpButtonDown && isJumpDown); //don't signal another jump when button wasn't released
                jumpButtonDown = isJumpDown;

                JumpEventKey.Broadcast(didJump);
            }
        }
    }
}
