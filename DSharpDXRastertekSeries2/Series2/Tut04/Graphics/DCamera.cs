using SharpDX;
using SharpDX.Mathematics.Interop;

namespace DSharpDXRastertek.Series2.Tut04.Graphics
{
    public class DCamera
    {
        private float PositionX { get; set; }
        private float PositionY { get; set; }
        private float PositionZ { get; set; }
        private float RotationX { get; set; }
        private float RotationY { get; set; }
        private float RotationZ { get; set; }
        public RawMatrix ViewMatrix { get; private set; }

        public DCamera() { }

        public void SetPosition(float x, float y, float z)
        {
            PositionX = x;
            PositionY = y;
            PositionZ = z;
        }
        public void Render()
        {
            RawVector3 position = new RawVector3(PositionX, PositionY, PositionZ);
            RawVector3 lookAt = new RawVector3(0, 0, 1);

            float pitch = RotationX * 0.0174532925f;
            float yaw = RotationY * 0.0174532925f; ;
            float roll = RotationZ * 0.0174532925f; ;
            RawMatrix rotationMatrix = Matrix.RotationYawPitchRoll(yaw, pitch, roll);

            lookAt = Vector3.TransformCoordinate(lookAt, rotationMatrix);
            RawVector3 up = Vector3.TransformCoordinate(Vector3.UnitY, rotationMatrix);
            lookAt = Vector3.Add(position, lookAt);

            ViewMatrix = Matrix.LookAtLH(position, lookAt, up);
        }
    }
}
