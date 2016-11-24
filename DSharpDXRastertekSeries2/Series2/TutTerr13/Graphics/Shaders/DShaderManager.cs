using SharpDX;
using SharpDX.Direct3D11;
using System;

namespace DSharpDXRastertek.Series2.TutTerr13.Graphics.Shaders
{
    public class DShaderManager
    {
        // Properties
        public DColorShader ColorShader { get; set; }
        public DFontShader FontShader { get; set; }
        public DTerrainShader TerrainShader { get; set; }
        public DSkyDomeShader SkyDomeShader { get; set; }

        // Methods
        public bool Initilize(DDX11 D3DDevice, IntPtr windowsHandle)
        {
            // Create the texture shader object.
            ColorShader = new DColorShader();
            // Initialize the texture shader object.
            if (!ColorShader.Initialize(D3DDevice.Device, windowsHandle))
                return false;

            // Create the font shader object.
            FontShader = new DFontShader();
            // Initialize the font shader object.
            if (!FontShader.Initialize(D3DDevice.Device, windowsHandle))
                return false;

            // Create the sky dome shader object.
            SkyDomeShader = new DSkyDomeShader();
            // Initialize the sky dome shader object.
            if (!SkyDomeShader.Initialize(D3DDevice.Device, windowsHandle))
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
            // Release the sky dome shader object.
            SkyDomeShader.ShutDown();
            SkyDomeShader = null;
            // Release the Terrain Shader ibject.
            TerrainShader?.ShutDown();
            TerrainShader = null;
            // Release the font shader object.
            FontShader?.Shuddown();
            FontShader = null;
            // Release the texture shader object.
            ColorShader?.ShutDown();
            ColorShader = null;
        }
        public bool RenderColorShader(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            // Render the ColoreShader.
            if (!ColorShader.Render(deviceContext, indexCount, worldMatrix, viewMatrix, projectionMatrix))
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
        public bool RenderTerrainShader(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, ShaderResourceView texture, ShaderResourceView normal, ShaderResourceView normal1, Vector3 lightDirection, Vector4 diffuse)
        {
            if (!TerrainShader.Render(deviceContext, indexCount, worldMatrix, viewMatrix, projectionMatrix, texture, normal, normal1, lightDirection, diffuse))
                return false;

            return true;
        }
        public bool RenderSkyDomeShader(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix,  Vector4 apwxColor, Vector4 centerColor)
        {
            if (!SkyDomeShader.Render(deviceContext, indexCount, worldMatrix, viewMatrix, projectionMatrix, apwxColor, centerColor))
                return false;

            return true;
        }
    }
}