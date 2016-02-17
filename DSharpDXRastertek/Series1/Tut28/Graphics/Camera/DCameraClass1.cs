using SharpDX;
using System;

namespace DSharpDXRastertek.Tut28.Graphics.Camera
{
    public class DCamera                    // 40 lines
    {
        // Properties.
        private float PositionX { get; set; }
        private float PositionY { get; set; }
        private float PositionZ { get; set; }
        private float RotationY { get; set; }
        public Matrix ViewMatrix { get; private set; }

        // Constructor
        public DCamera() { }

        // Methods.
        public void SetPosition(float x, float y, float z)
        {
            PositionX = x;
            PositionY = y;
            PositionZ = z;
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
    }
}