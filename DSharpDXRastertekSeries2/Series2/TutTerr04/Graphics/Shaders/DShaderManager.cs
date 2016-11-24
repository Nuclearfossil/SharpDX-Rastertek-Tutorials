using SharpDX;
using SharpDX.Direct3D11;
using System;

namespace DSharpDXRastertek.Series2.TutTerr04.Graphics.Shaders
{
    public class DShaderManager
    {
        // Properties
        public DLightShader LightShader { get; set; }
        public DFontShader FontShader { get; set; }

        // Methods
        public bool Initilize(DDX11 D3DDevice, IntPtr windowsHandle)
        {
            // Create the light shader object.
            LightShader = new DLightShader();
            // Initialize the light shader object.
            if (!LightShader.Initialize(D3DDevice.Device, windowsHandle))
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
            // Release the light shader object.
            LightShader?.ShutDown();
            LightShader = null;
        }
        public bool RenderFontShader(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix orthoMatrix, ShaderResourceView texture, Vector4 fontColour)
        {
            // Render the FontShader.
            if (!FontShader.Render(deviceContext, indexCount, worldMatrix, viewMatrix, orthoMatrix, texture, fontColour))
                return false;

            return true;
        }
        public bool RenderLightShader(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, ShaderResourceView texture, Vector3 lightDirection, Vector4 diffuse)
        {
            // Render the model using the light shader.
            if (!LightShader.Render(deviceContext, indexCount, worldMatrix, viewMatrix, projectionMatrix, texture, lightDirection, diffuse))
                return false;

            return true;
        }
    }
}