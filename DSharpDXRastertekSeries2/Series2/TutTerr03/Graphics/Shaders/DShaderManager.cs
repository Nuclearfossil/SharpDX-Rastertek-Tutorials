using SharpDX;
using SharpDX.Direct3D11;
using System;

namespace DSharpDXRastertek.Series2.TutTerr03.Graphics.Shaders
{
    public class DShaderManager
    {
        // Properties
        public DTextureShader TextureShader { get; set; }
        public DFontShader FontShader { get; set; }

        // Methods
        public bool Initilize(DDX11 D3DDevice, IntPtr windowsHandle)
        {
            // Create the texture shader object.
            TextureShader = new DTextureShader();
            // Initialize the texture shader object.
            if (!TextureShader.Initialize(D3DDevice.Device, windowsHandle))
                return false;

            // Create the font shader object.
            FontShader = new DFontShader();
            // Initialize the font shader object.
            if (!FontShader.Initialize(D3DDevice.Device, windowsHandle))
                return false;

            return true;
        }
        public void ShutDown()
        {
            // Release the font shader object.
            FontShader?.Shuddown();
            FontShader = null;
            // Release the texture shader object.
            TextureShader?.ShutDown();
            TextureShader = null;
        }
        public bool RenderTextureShader(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, ShaderResourceView texture)
        {
            // Render the TextureShader.
            if (!TextureShader.Render(deviceContext, indexCount, worldMatrix, viewMatrix, projectionMatrix, texture))
                return false;

            return true;
        }
        public bool RenderFontShader(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix orthoMatrix, ShaderResourceView texture, Vector4 fontColour)
        {
            // Render the FontShader.
            if (!FontShader.Render(deviceContext, indexCount, worldMatrix, viewMatrix, orthoMatrix, texture, fontColour))
                return false;

            return true;
        }
    }
}
