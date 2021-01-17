// Copyright (c) Stride contributors (https://stride3d.net) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;
using RiseOfTheUndeaf.Core;
using RiseOfTheUndeaf.Input;
using RiseOfTheUndeaf.EntityEvents;
using RiseOfTheUndeaf.EntityEvents.Character;
using RiseOfTheUndeaf.EntityEvents.Camera;

namespace RiseOfTheUndeaf.Player
{
    public class PlayerInput : SyncScript
    {
        private bool jumpButtonDown = false;
        private bool cameraEnabled = false;

        public CameraComponent Camera { get; set; }

        public InputSettings InputSettings { get; set; }

        public override void Start()
        {
            Input.VirtualButtonConfigSet = Input.VirtualButtonConfigSet ?? new VirtualButtonConfigSet();
            Input.VirtualButtonConfigSet.Add(InputSettings.CreateVirtualButtonConfig());
        }

        public override void Update()
        {
            UpdateCharacterMovement();
            UpdateCameraRotation();
            UpdateJumpAction();
            UpdateAttack();
        }

        private void UpdateJumpAction()
        {
            // Jumping: don't bother with jump restrictions here, just pass the button states
            var isJumpDown = Input.GetVirtualButton(0, GameButton.Jump) > 0;
            var didJump = (!jumpButtonDown && isJumpDown); //don't signal another jump when button wasn't released
            jumpButtonDown = isJumpDown;

            if (didJump)
                Entity.BroadcastEvent<IMovementEvents>().Jump();
        }

        private void UpdateCameraRotation()
        {
            // Camera rotation: left-right rotates the camera horizontally while up-down controls its altitude
            var cameraDirection = new Vector2
            {
                X = Input.GetVirtualButton(0, GameButton.CameraHorizontal),
                Y = Input.GetVirtualButton(0, GameButton.CameraVertical),
            };
            cameraDirection.Normalize();

            // Mouse-based camera rotation. Only enabled after you click the screen to lock your cursor, pressing escape cancels this
            // TODO: modify the two cases below in some sensible way
            if (Input.IsMouseButtonDown(MouseButton.Left))
            {
                EnableCameraRotation();
            }
            if (Input.IsKeyPressed(Keys.Escape))
            {
                DisableCameraRotation();
            }

            // Broadcast the camera direction directly, as a screen-space Vector2
            if (cameraEnabled)
                Entity.BroadcastEvent<ICameraEvents>().MoveBy(cameraDirection);
        }

        private void DisableCameraRotation()
        {
            Input.UnlockMousePosition();
            Game.IsMouseVisible = true;

            cameraEnabled = false;
        }

        private void EnableCameraRotation()
        {
            Input.LockMousePosition(true);
            Game.IsMouseVisible = false;
            
            cameraEnabled = true;
        }

        private void UpdateCharacterMovement()
        {
            // Character movement: should be camera-aware
            var moveDirection = new Vector2
            {
                X = Input.GetVirtualButton(0, GameButton.MovementHorizontal),
                Y = Input.GetVirtualButton(0, GameButton.MovementVertical),
            };

            // Broadcast the movement vector as a world-space Vector3 to allow characters to be controlled
            // (rotates the vector so that character moves in the direction of where camera is looking)
            var worldSpeed = (Camera != null)
                ? Utils.LogicDirectionToWorldDirection(moveDirection, Camera, Vector3.UnitY)
                : new Vector3(moveDirection.X, 0, moveDirection.Y);

            // Adjust vector's magnitute - worldSpeed has been normalized
            var moveLength = moveDirection.Length();

            if (moveLength > 1)
                moveLength = 1;
            else
                // expand input range of [dead..1) to the [0..1)
                moveLength = (moveLength - InputSettings.JoystickDeadZone) / (1f - InputSettings.JoystickDeadZone);

            worldSpeed *= moveLength;

            Entity.BroadcastEvent<IMovementEvents>().Move(worldSpeed);
        }

        private void UpdateAttack()
        {
            if (Input.GetVirtualButton(0, GameButton.PrimaryAttack) > 0)
            {
                Entity.BroadcastEvent<IAttackEvents>().PrimaryAttack();
            }

            if (Input.GetVirtualButton(0, GameButton.SecondaryAttack) > 0)
            {
                Entity.BroadcastEvent<IAttackEvents>().SecondaryAttack();
            }
        }
    }
}
