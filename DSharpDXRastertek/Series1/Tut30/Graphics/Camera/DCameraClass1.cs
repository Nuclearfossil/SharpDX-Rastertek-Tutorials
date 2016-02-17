using SharpDX;

namespace DSharpDXRastertek.Tut30.Graphics.Camera
{
    public class DCamera                    // 55 lines
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
            //// Setup where the camera is looking  forwardby default.
            Vector3 lookAt = new Vector3(0, 0, 1.0f);

            // Setup the position of the camera in the world.
            var position = new Vector3(PositionX, PositionY, PositionZ);

            // Create the view matrix from the three vectors.
            ViewMatrix = Matrix.LookAtLH(position, lookAt, Vector3.UnitY);
        }
    }
}