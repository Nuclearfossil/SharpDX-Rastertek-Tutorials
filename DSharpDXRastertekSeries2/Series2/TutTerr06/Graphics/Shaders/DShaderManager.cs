using SharpDX;
using SharpDX.Direct3D11;
using System;

namespace DSharpDXRastertek.Series2.TutTerr06.Graphics.Shaders
{
    public class DShaderManager
    {
        // Properties
        public DFontShader FontShader { get; set; }
        public DTerrainShader TerrainShader { get; set; }

        // Methods
        public bool Initilize(DDX11 D3DDevice, IntPtr windowsHandle)
        {
            // Create the font shader object.
            FontShader = new DFontShader();
            // Initialize the font shader object.
            if (!FontShader.Initialize(D3DDevice.Device, windowsHandle))
                return false;

            // Create the terrain shader object.
            TerrainShader = new DTerrainShader();
            // Initialize the terrain shader object.
            if (!TerrainShader.Initialize(D3DDevice.Device, windowsHandle))
                return false;

            return true;
        }
        public void ShutDown()
        {
            // Release the Terrain Shader ibject.
            TerrainShader?.ShutDown();
            TerrainShader = null;
            // Release the font shader object.
            FontShader?.Shuddown();
            FontShader = null;
        }
        public bool RenderFontShader(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix orthoMatrix, ShaderResourceView texture, Vector4 fontColour)
        {
            // Render the FontShader.
            if (!FontShader.Render(deviceContext, indexCount, worldMatrix, viewMatrix, orthoMatrix, texture, fontColour))
                return false;

            return true;
        }
        public bool RenderTerrainShader(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, ShaderResourceView texture, ShaderResourceView normal, Vector3 lightDirection, Vector4 diffuse)
        {
            if (!TerrainShader.Render(deviceContext, indexCount, worldMatrix, viewMatrix, projectionMatrix, texture, normal, lightDirection, diffuse))
                return false;

            return true;
        }
    }
}