using SharpDX;

namespace DSharpDXRastertek.Tut32.Graphics
{
    public class DCamera                    // 38 lines
    {
        // Properties.
        private float PositionX { get; set; }
        private float PositionY { get; set; }
        private float PositionZ { get; set; }
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
            Vector3 position = new Vector3(PositionX, PositionY, PositionZ);

            // Setup where the camera is looking by default.
            Vector3 lookAt = new Vector3(0, 0, 1);

            // Transform the lookAt and up vector by the rotation matrix so the view is correctly rotated at the origin.
            Vector3 up = Vector3.UnitY;

            // Finally create the view matrix from the three updated vectors.
            ViewMatrix = Matrix.LookAtLH(position, lookAt, up);
        }
    }
}