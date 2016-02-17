using SharpDX;

namespace DSharpDXRastertek.Tut20.Graphics.Data
{
    public class DLight                 // 21 lines
    {
        // Properties
        public Vector4 DiffuseColour { get; private set; }
        public Vector3 Direction { get; private set; }

        // Methods
        public void SetDiffuseColor(float red, float green, float blue, float alpha)
        {
            DiffuseColour = new Vector4(red, green, blue, alpha);
        }
        public void SetDirection(float x, float y, float z)
        {
            Direction = new Vector3(x, y, z);
        }
    }
}