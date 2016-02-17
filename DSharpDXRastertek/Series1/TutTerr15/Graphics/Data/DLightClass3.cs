using SharpDX;

namespace DSharpDXRastertek.TutTerr15.Graphics.Data
{
    public class DLight                 // 17 lines
    {
        // Properties
        public Vector4 DiffuseColour { get; private set; }
        public Vector3 Direction { get; set; }

        // Methods
        public void SetDiffuseColor(float red, float green, float blue, float alpha)
        {
            DiffuseColour = new Vector4(red, green, blue, alpha);
        }
    }
}