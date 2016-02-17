using SharpDX;

namespace DSharpDXRastertek.Tut30.Graphics.Data
{
    public class DLight                 // 21 lines
    {
        // Properties
        public Vector4 Position { get; set; }
        public Vector4 DiffuseColour { get; private set; }
   
        // Methods
        public void SetPosition(float x, float y, float z)
        {
            Position = new Vector4(x, y, z, 1.0f);
        }
        public void SetDiffuseColor(float red, float green, float blue, float alpha)
        {
            DiffuseColour = new Vector4(red, green, blue, alpha);
        }
    }
}