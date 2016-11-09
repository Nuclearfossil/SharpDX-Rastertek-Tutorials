using SharpDX;

namespace DSharpDXRastertek.Series2.TutTerr05.Graphics.Data
{
    public class DLight                 // 32 lines
    {
        // Properties
        public Vector4 DiffuseColour { get; private set; }
        public Vector3 Direction { get; set; }
        public Vector3 Position { get; internal set; }

        // Methods
        public void SetDiffuseColor(float red, float green, float blue, float alpha)
        {
            DiffuseColour = new Vector4(red, green, blue, alpha);
        }
    }
}