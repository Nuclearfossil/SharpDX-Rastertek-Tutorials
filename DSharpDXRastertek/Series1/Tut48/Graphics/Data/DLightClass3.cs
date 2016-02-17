using SharpDX;

namespace DSharpDXRastertek.Tut48.Graphics.Data
{
    public class DLight                 // 43 lines
    {
        // Properties
        public Vector4 AmbientColor { get; private set; }
        public Vector4 DiffuseColour { get; private set; }
        public Vector3 Direction { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 LookAt { get; set; }
        public Matrix ViewMatrix { get; set; }
        public Matrix OrthoMatrix { get; set; }

        // Methods
        public void SetAmbientColor(float red, float green, float blue, float alpha)
        {
            AmbientColor = new Vector4(red, green, blue, alpha);
        }
        public void SetDiffuseColor(float red, float green, float blue, float alpha)
        {
            DiffuseColour = new Vector4(red, green, blue, alpha);
        }
        public void GenerateOrthoMatrix(float width, float depthPlane, float nearPlane)
        {
            // Create the orthographic matrix for the light that represents the Sun with Square shadowns not trapazoidal.
            OrthoMatrix = Matrix.OrthoLH(width, width, nearPlane, depthPlane);
        }
        public void GenerateViewMatrix()
        {
            // Setup the vector that points upwards.
            Vector3 upVector = Vector3.Up;

            // Create the view matrix from the three vectors.
            ViewMatrix = Matrix.LookAtLH(Position, LookAt, upVector);
        }
        public void SetLookAt(float x, float y, float z)
        {
            LookAt = new Vector3(x, y, z);
        }
    }
}