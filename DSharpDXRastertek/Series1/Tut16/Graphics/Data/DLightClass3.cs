using SharpDX;

namespace DSharpDXRastertek.Tut16.Graphics.Data
{
    public class DLight                 // 16 lines
    {
        // Properties
        public Vector3 Direction { get; private set; }

        // Methods
        public void SetDirection(float x, float y, float z)
        {
            Direction = new Vector3(x, y, z);
        }
    }
}