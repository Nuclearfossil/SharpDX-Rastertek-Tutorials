using SharpDX;

namespace DSharpDXRastertek.Tut43.Graphics.Data
{
    public class DViewPoint                 // 43 lines
    {
        // Variables
        private float m_fieldOfView, m_aspectRatio, m_nearPlane, m_farPlane;

        // Properties
        public Vector3 Position { get; set; }
        public Vector3 LookAt { get; set; }
        public Matrix ViewMatrix { get; set; }
        public Matrix ProjectionMatrix { get; set; }

        // Methods.
        public void SetPosition(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
        }
        public void SetLookAt(float x, float y, float z)
        {
            LookAt = new Vector3(x, y, z);
        }
        public void SetProjectionParameters(float fieldOfView, float aspectRatio, float nearPlane, float farPlane)
        {
            m_fieldOfView = fieldOfView;
            m_aspectRatio = aspectRatio;
            m_nearPlane = nearPlane;
            m_farPlane = farPlane;
        }
        public void GenerateViewMatrix()
        {
            // Create the view matrix from the three vectors.
            ViewMatrix = Matrix.LookAtLH(Position, LookAt, Vector3.Up);
        }
        public void GenerateProjectionMatrix()
        {
            // Create the projection matrix for the view point.
            ProjectionMatrix = Matrix.PerspectiveFovLH(m_fieldOfView, m_aspectRatio, m_nearPlane, m_farPlane);
        }
    }
}