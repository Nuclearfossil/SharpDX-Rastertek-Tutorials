using SharpDX;

namespace DSharpDXRastertek.Tut21.Graphics.Data
{
    public class DLight                 // 31 lines
    {
        // Properties
        public Vector4 DiffuseColour { get; private set; }
        public Vector3 Direction { get; private set; }
        public Vector4 SpecularColor { get; private set; }
        public float SpecularPower { get; private set; }

        // Methods
        public void SetDiffuseColor(float red, float green, float blue, float alpha)
        {
            DiffuseColour = new Vector4(red, green, blue, alpha);
        }
        public void SetDirection(float x, float y, float z)
        {
            Direction = new Vector3(x, y, z);
        }
        public void SetSpecularColor(float red, float green, float blue, float alpha)
        {
            SpecularColor = new Vector4(red, green, blue, alpha);
        }
        public void SetSpecularPower(float power)
        {
            SpecularPower = power;
        }
    }
}