using System;

namespace DSharpDXRastertek.Tut49.Input
{
    public class DPosition                  // 212 lines
    {
        // Variables
        private float leftTurnSpeed, rightTurnSpeed;
        private float lookUpSpeed, downLookSpeed;
        private float forwardsMoveSpeed, reverseMoceSpeed, downwardSpeed, upwardSpeed;

        // Properties
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float FrameTime { get; set; }
        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }

        // Public Methods
        public void SetPosition(float x, float y, float z)
        {
            PositionX = x;
            PositionY = y;
            PositionZ = z;
        }
        public void SetRotation(float x, float y, float z)
        {
            RotationX = x;
            RotationY = y;
            RotationZ = z;
        }
        public void TurnLeft(bool keydown)
        {
            // If the key is pressed increase the speed at which the camera turns left. If not slow down the turn speed.
            if (keydown)
            {
                leftTurnSpeed += FrameTime * 0.01f;
                if (leftTurnSpeed > FrameTime * 0.15)
                    leftTurnSpeed = FrameTime * 0.15f;
            }
            else
            {
                leftTurnSpeed -= FrameTime * 0.005f;
                if (leftTurnSpeed < 0)
                    leftTurnSpeed = 0;
            }

            // Update the rotation using the turning speed.
            RotationY -= leftTurnSpeed;

            // Keep the rotation in the 0 to 360 range.
            if (RotationY < 0)
                RotationY += 360;
        }
        public void TurnRight(bool keydown)
        {
            // If the key is pressed increase the speed at which the camera turns right. If not slow down the turn speed.
            if (keydown)
            {
                rightTurnSpeed += FrameTime * 0.01f;
                if (rightTurnSpeed > FrameTime * 0.15)
                    rightTurnSpeed = FrameTime * 0.15f;
            }
            else
            {
                rightTurnSpeed -= FrameTime * 0.005f;
                if (rightTurnSpeed < 0)
                    rightTurnSpeed = 0;
            }

            // Update the rotation using the turning speed.
            RotationY += rightTurnSpeed;

            // Keep the rotation in the 0 to 360 range which is looking stright Up.
            if (RotationY > 360)
                RotationY -= 360;
        }
        public void LookDown(bool keydown)
        {
            // If the key is pressed increase the speed at which the camera turns down. If not slow down the turn speed.
            if (keydown)
            {
                downLookSpeed += FrameTime * 0.01f;
                if (downLookSpeed > FrameTime * 0.15)
                    downLookSpeed = FrameTime * 0.15f;
            }
            else
            {
                downLookSpeed -= FrameTime * 0.005f;
                if (downLookSpeed < 0)
                    downLookSpeed = 0;
            }

            // Update the rotation using the turning speed.
            RotationX += downLookSpeed;

            // Keep the rotation maximum 90 degrees which is looking straight down.
            if (RotationX < -90)
                RotationX = -90;
        }
        internal void LookUp(bool keydown)
        {
            if (keydown)
            {
                // Update the upward rotation speed movement based on the frame time and whether the user is holding the key down or not.
                lookUpSpeed += FrameTime * 0.01f;

                if (lookUpSpeed > FrameTime * 0.35f)
                    lookUpSpeed = FrameTime * 0.15f;
            }
            else
            {
                lookUpSpeed -= FrameTime * 0.005f;
                if (lookUpSpeed < 0.0f)
                    lookUpSpeed = 0.0f;
            }

            // Update the rotation.
            RotationX -= lookUpSpeed;

            // Keep the rotation maximum 90 degrees.
            if (RotationX > 90.0f)
                RotationX = 90.0f;
        }
        internal void MoveForward(bool keydown)
        {
            // If the key is pressed increase the speed at which the camera moves forward in the Y axis. If not slow down the turn speed.
            if (keydown)
            {
                forwardsMoveSpeed += FrameTime * 0.001f;
                if (forwardsMoveSpeed > FrameTime * 0.03)
                    forwardsMoveSpeed = FrameTime * 0.03f;
            }
            else
            {
                forwardsMoveSpeed -= FrameTime * 0.007f;
                if (forwardsMoveSpeed < 0)
                    forwardsMoveSpeed = 0;
            }

            // Convert degrees to radians.
            float radians = RotationY * 0.0174532925f;
            float radians2 = RotationX * 0.0174532925f;
             
            // Update the position.
            PositionX += (float)Math.Sin(radians) * forwardsMoveSpeed;
            PositionZ += (float)Math.Cos(radians) * forwardsMoveSpeed;
        }
        internal void MoveBackward(bool keydown)
        {
            // If the key is pressed increase the speed at which the camera moves backward in the Y axis. If not slow down the turn speed.
            if (keydown)
            {
                reverseMoceSpeed += FrameTime * 0.001f;
                if (reverseMoceSpeed > FrameTime * 0.03f)
                    reverseMoceSpeed = FrameTime * 0.03f;
            }
            else
            {
                reverseMoceSpeed -= FrameTime * 0.007f;
                if (reverseMoceSpeed < 0)
                    reverseMoceSpeed = 0;
            }

            // Convert degrees to radians.
            float radians = RotationY * 0.0174532925f;
            float radians2 = RotationX * 0.0174532925f;

            // Update the position.
            PositionX -= (float)Math.Sin(radians) * reverseMoceSpeed;
            PositionZ -= (float)Math.Cos(radians) * reverseMoceSpeed;
        }
        public void MoveUpward(bool keydown)
        {
            if (keydown)
            {
                upwardSpeed += FrameTime * 0.003f;
                if (upwardSpeed > (FrameTime * 0.03f))
                    upwardSpeed = FrameTime * 0.03f;
            }
            else
            {
                upwardSpeed -= FrameTime * 0.002f;
                if (upwardSpeed < 0.0f)
                    upwardSpeed = 0.0f;
            }

            // Update the height position.
            PositionY += upwardSpeed;
        }
        public void MoveDownward(bool keydown)
        {
            if (keydown)
            {
                downwardSpeed += FrameTime * 0.003f;
                if (downwardSpeed > (FrameTime * 0.03f))
                    downwardSpeed = FrameTime * 0.03f;
            }
            else
            {
                downwardSpeed -= FrameTime * 0.002f;
                if (downwardSpeed < 0.0f)
                    downwardSpeed = 0.0f;
            }

            // Update the height position.
            PositionY -= downwardSpeed;
        }
    }
}