using SharpDX;

namespace DSharpDXRastertek.TutTerr05.Graphics.Data
{
    public class DLight                 // 22 lines
    {
        // Properties
        public Vector4 AmbientColor { get; private set; }
        public Vector4 DiffuseColour { get; private set; }
        public Vector3 Direction { get; set; }

        // Methods
        public void SetAmbientColor(float red, float green, float blue, float alpha)
        {
            AmbientColor = new Vector4(red, green, blue, alpha);
        }
        public void SetDiffuseColor(float red, float green, float blue, float alpha)
        {
            DiffuseColour = new Vector4(red, green, blue, alpha);
        }
    }
}