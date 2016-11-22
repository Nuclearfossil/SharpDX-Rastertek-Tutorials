using DSharpDXRastertek.Series2.TutTerr13.System;
using DSharpDXRastertek.Series2.TutTerr13.Graphics.Shaders;
using SharpDX;

namespace DSharpDXRastertek.Series2.TutTerr13.Graphics.Models
{
    public class DMiniMap
    {
        // Variables
        int m_mapLocationX, m_mapLocationY, m_pointLocationX, m_pointLocationY;
        float m_mapSizeX, m_mapSizeY, m_terrainWidth, m_terrainHeight;

        // Properties
        public DBitmap MiniMapBitmap { get; set; }
        public DBitmap PointBitmap { get; set; }

        public DMiniMap() { }

        public bool Initialize(SharpDX.Direct3D11.Device device, SharpDX.Direct3D11.DeviceContext deviceContext, DSystemConfiguration condifuration, float terrainWidth, float terrainHeight)
        {
            // Set the size of the mini-map minus the borders.
            m_mapSizeX = 150.0f;
            m_mapSizeY = 150.0f;

            // Initialize the location of the mini-map on the top right corner od the screen.
            m_mapLocationX = condifuration.Width - (int)m_mapSizeX - 10;
            m_mapLocationY = 10;

            // Store the terrain size.
            m_terrainWidth = terrainWidth;
            m_terrainHeight = terrainHeight;

            // Create the mini-map bitmap object.
            MiniMapBitmap = new DBitmap();
            // Initialize the mini-map bitmap object.
            if (!MiniMapBitmap.Initialize(device, condifuration, 154, 154, "minimap.bmp"))
                return false;

            // Create the point bitmap object.
            PointBitmap = new DBitmap();
            // Initialize the point bitmap object.
            if (!PointBitmap.Initialize(device, condifuration, 3, 3, "point.bmp"))
                return false;

            return true;
        }
        public void ShutDown()
        {
            // Release the point bitmap object.
            PointBitmap?.Shutdown();
            PointBitmap = null;
            // Release the mini-map bitmap object.
            MiniMapBitmap?.Shutdown();
            MiniMapBitmap = null;
        }
        public bool Render(SharpDX.Direct3D11.DeviceContext deviceContext, DShaderManager shaderManager, Matrix worldMatrix, Matrix viewMatrix, Matrix orthoMatrix)
        {
            // Put the mini-map bitmap vertex and index buffers on the graphics pipeline to prepare them for drawing.
            if (!MiniMapBitmap.Render(deviceContext, m_mapLocationX, m_mapLocationY))
                return false;

            // Render the mini-map bitmap using the texture shader.
            if (!shaderManager.RenderTextureShader(deviceContext, MiniMapBitmap.IndexCount, worldMatrix, viewMatrix, orthoMatrix, MiniMapBitmap.Texture.TextureResource))
                return false;

            // Put the point bitmap vertex and index buffers on the graphics pipeline to prepare them for drawing.
            if (!PointBitmap.Render(deviceContext, m_pointLocationX, m_pointLocationY))
                return false;
            // Render the point bitmap using the texture shader.
            if (!shaderManager.RenderTextureShader(deviceContext, PointBitmap.IndexCount, worldMatrix, viewMatrix, orthoMatrix, PointBitmap.Texture.TextureResource))
                return false;

            return true;
        }
        public void PositionUpdate(float positionX, float positionZ)
        {
            // Ensure the point does not leave the minimap borders even if the camera goes past the terrain borders.
            if (positionX < 0)
                positionX = 0;
            if (positionZ < 0)
                positionZ = 0;
            if (positionX > m_terrainWidth)    
                positionX = m_terrainWidth;   
            if (positionZ > m_terrainHeight)
                positionZ = m_terrainHeight;

            // Calculate the position of the camera on the minimap in terms of percentage.
            float percentX = positionX / m_terrainWidth;
            float percentY = 1.0f - (positionZ / m_terrainHeight);

            // Determine the pixel location of the point on the mini-map.
            m_pointLocationX = (m_mapLocationX + 2) + (int)(percentX * m_mapSizeX);
            m_pointLocationY = (m_mapLocationY + 2) + (int)(percentY * m_mapSizeY);

            // Subtract one from the location to center the point on the mini-map according to the 3x3 point pixel image size.
            m_pointLocationX = m_pointLocationX - 1;
            m_pointLocationY = m_pointLocationY - 1;
        }
    }
}