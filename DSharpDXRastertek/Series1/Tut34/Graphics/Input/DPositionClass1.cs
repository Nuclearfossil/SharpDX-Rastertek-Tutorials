using System;

namespace DSharpDXRastertek.Tut34.Input
{
    public class DPosition                  // 77 lines
    {
        // Variables
        private float leftTurnSpeed, rightTurnSpeed;

        // Properties
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float FrameTime { get; set; }
        public float RotationY { get; private set; }
        public float MovementY { get; private set; }
        public float MovementX { get; private set; }

        // Public Methods
        public void SetPosition(float x, float y, float z)
        {
            PositionX = x;
            PositionY = y;
            PositionZ = z;
        }
        internal void MoveLeft(bool keydown)
        {
            // Update the forward speed movement based on the frame time and whether the user is holding the key down or not.
            if (keydown)
            {
                leftTurnSpeed += FrameTime * 0.1f;

                if(leftTurnSpeed > (FrameTime * 0.03f))
                    leftTurnSpeed = FrameTime * 0.03f;
            }
            else
            {
                leftTurnSpeed -= FrameTime * 0.07f;

                if (leftTurnSpeed < 0.0f)
                    leftTurnSpeed = 0.0f;
            }

            // Convert degrees to radians.
            float radians = RotationY * 0.0174532925f;

            // Update the position.
            PositionX -= ((float)Math.Cos(radians)) * leftTurnSpeed;
            PositionZ -= ((float)Math.Sin(radians)) * leftTurnSpeed;
        }
        internal void MoveRight(bool keydown)
        {
            // Update the backward speed movement based on the frame time and whether the user is holding the key down or not.
            if (keydown)
            {
                rightTurnSpeed += FrameTime * 0.1f;

                if (rightTurnSpeed > (FrameTime * 0.03f))
                    rightTurnSpeed = FrameTime * 0.03f;
            }
            else
            {
                rightTurnSpeed -= FrameTime * 0.07f;

                if (rightTurnSpeed < 0.0f)
                    rightTurnSpeed = 0.0f;
            }

            // Convert degrees to radians.
            float radians = RotationY * 0.0174532925f;

            // Update the position.
            PositionX += ((float)Math.Cos(radians) * rightTurnSpeed);
            PositionZ += ((float)Math.Sin(radians) * rightTurnSpeed);
        }
    }
}