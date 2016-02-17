using SharpDX.Direct3D11;

namespace DSharpDXRastertek.TutTerr02.Graphics.Models
{
    public class DTexture                   // 31 lines
    {
        // Propertues
        public ShaderResourceView TextureResource { get; private set; }

        // Methods.
        public bool Initialize(Device device, string fileName)
        {
            try
            {
                // Load the texture file.
                TextureResource = ShaderResourceView.FromFile(device, fileName);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void ShutDown()
        {
            // Release the texture resource.
            TextureResource?.Dispose();
            TextureResource = null;
        }
    }
}