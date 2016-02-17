using SharpDX;
using System;

namespace DSharpDXRastertek.Tut29.Graphics.Camera
{
    public class DCamera                    // 67 lines
    {
        // Properties.
        private float PositionX { get; set; }
        private float PositionY { get; set; }
        private float PositionZ { get; set; }
        private float RotationX { get; set; }
        private float RotationY { get; set; }
        private float RotationZ { get; set; }
        public Matrix ViewMatrix { get; private set; }
        public Matrix ReflectionViewMatrix { get; private set; }

        // Constructor
        public DCamera() { }

        // Methods.
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
        public Vector3 GetPosition()
        {
            return new Vector3(PositionX, PositionY, PositionZ);
        }
        public void Render()
        {
            // Setup the position of the camera in the world.
            var position = new Vector3(PositionX, PositionY, PositionZ);

            // Calculate the rotation in radians.
            var yaw = RotationY * 0.0174532925f;

            // Setup where the camera is looking.
            var lookAt = new Vector3((float)Math.Sin(yaw) + position.X, position.Y, (float)Math.Cos(yaw) + position.Z);

            // Create the view matrix from the three vectors.
            ViewMatrix = Matrix.LookAtLH(position, lookAt, Vector3.UnitY);
        }
        public void RenderReflection(float height)
        {
            // Setup the position of the camera in the world.
            Vector3 position = new Vector3(PositionX, -PositionY + (height * 2), PositionZ);

            // Set the yaw (Y axis), pitch (X axis), and roll (Z axis) rotations in radians.
            float yaw = RotationY * 0.0174532925f;

            // Setup where the camera is looking by default.
            var lookAt = new Vector3((float)Math.Sin(yaw) + position.X, position.Y, (float)Math.Cos(yaw) + position.Z);

            // Finally create the reflection view matrix from the three updated vectors.
			ReflectionViewMatrix = Matrix.LookAtLH(position, lookAt, Vector3.UnitY);
        }
    }
}