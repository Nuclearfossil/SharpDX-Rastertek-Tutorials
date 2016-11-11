using DSharpDXRastertek.Series2.TutTerr06.System;
using SharpDX.Direct3D11;

namespace DSharpDXRastertek.Series2.TutTerr06.Graphics.Models
{
    public class DTextureManager
    {
        public int TextureCount { get; set; }
        public DTexture[] TextureArray { get; set; }

        public bool Initialize(int count)
        {
            TextureCount = count;

            // Create the color texture object.
            TextureArray = new DTexture[TextureCount];

            return true;
        }
        public void ShutDown()
        {
            foreach (DTexture tex in TextureArray)
                tex?.ShutDown();

            TextureArray = null;
        }
        public bool LoadTexture(Device device, DeviceContext deviceContext, string filename, int location)
        {
            // Initialize the color texture object
            TextureArray[location] = new DTexture();
            if (!TextureArray[location].Initialize(device, DSystemConfiguration.TextureFilePath + filename))
                return false;

            return true;
        }
    }
}
