using DSharpDXRastertek.TutTerr08.Graphics.Shaders;
using SharpDX;
using SharpDX.Direct3D11;
using System;

namespace DSharpDXRastertek.TutTerr08.Graphics.Models
{
    public class DMiniMapClass                  // 126 lines
    {
        // Variables
        private int m_MapLocationX, m_MapLocationY, m_PointLocationX, m_PointLocationY;
        private float m_MapSizeX, m_MapSizeY, m_TerrainWidth, m_TerrainHeight;

        // Properties
        public Matrix ViewMatrix { get; set; }
        public DBitmap MiniMapBitmap { get; set; }
        public DBitmap Border { get; set; }
        public DBitmap Point { get; set; }

        // Methods
        public bool Initialize(Device device, IntPtr windowHandler, int screenWidth, int screenHeight, Matrix viewMatrix, float terrainWidth, float terrainHeight)
        {
            // Initialize the location of the mini-map on the screen.
            m_MapLocationX = 150;
            m_MapLocationY = 75;

            // Set the size of the mini-map.
            m_MapSizeX = 150.0f;
            m_MapSizeY = 150.0f;

            // Store the base view matrix.
            ViewMatrix = viewMatrix;

            // Store the terrain size.
            m_TerrainWidth = terrainWidth;
            m_TerrainHeight = terrainHeight;

            // Create the mini-map bitmap object.
            MiniMapBitmap = new DBitmap();

            // Initialize the mini-map bitmap object.
            if (!MiniMapBitmap.Initialize(device, screenWidth, screenHeight, "colorm01.bmp", 150, 150))
                return false;

            // Create the border bitmap object.
            Border = new DBitmap();

            // Initialize the border bitmap object.
            if (!Border.Initialize(device, screenWidth, screenHeight,"border01.bmp", 154, 154 ))
                return false;

            // Create the point bitmap object.
            Point = new DBitmap();

            // Initialize the point bitmap object.
            if (!Point.Initialize(device, screenWidth, screenHeight, "point01.bmp", 3, 3))
                return false;

            return true;
        }
        public void ShutDown()
        {
            // Release the point bitmap object.
            Point?.Shutdown();
            Point = null;
            // Release the border bitmap object.
            Border?.Shutdown();
            Border = null;
            // Release the mini-map bitmap object.
            MiniMapBitmap?.Shutdown();
            MiniMapBitmap = null;
        }
        public bool Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix orthoMatrix, DTextureShader textureShader)
        {
            // Put the border bitmap vertex and index buffers on the graphics pipeline to prepare them for drawing.
            if (!Border.Render(deviceContext, (m_MapLocationX - 2), (m_MapLocationY - 2)))
                return false;

            // Render the border bitmap using the texture shader.
            if (!textureShader.Render(deviceContext, Border.IndexCount, worldMatrix, ViewMatrix, orthoMatrix, Border.Texture.TextureResource))
                return false;

            // Put the mini-map bitmap vertex and index buffers on the graphics pipeline to prepare them for drawing.
            if (!MiniMapBitmap.Render(deviceContext, m_MapLocationX, m_MapLocationY))
                return false;

            // Render the mini-map bitmap using the texture shader.
            if (!textureShader.Render(deviceContext, MiniMapBitmap.IndexCount, worldMatrix, ViewMatrix, orthoMatrix,MiniMapBitmap.Texture.TextureResource ))
                return false;

            // Put the point bitmap vertex and index buffers on the graphics pipeline to prepare them for drawing.
            if (!Point.Render(deviceContext, m_PointLocationX, m_PointLocationY))
                return false;

            // Render the point bitmap using the texture shader.
            if (!textureShader.Render(deviceContext, Point.IndexCount, worldMatrix, ViewMatrix, orthoMatrix, Point.Texture.TextureResource))
                return false;

            return true;
        }
        public void PositionUpdate(float positionX, float positionZ)
        {
            // Ensure the point does not leave the minimap borders even if the camera goes past the terrain borders.
            if (positionX < 0)
                positionX = 0.0f;
            if (positionZ < 0)
                positionZ = 0.0f;
            if (positionX > m_TerrainWidth)
                positionX = m_TerrainWidth;
            if (positionX > m_TerrainHeight)
                positionX = m_TerrainHeight;

            // Calculate the position of the camera on the minimap in terms of percentage.
            float percentX = positionX / m_TerrainWidth;
            float percentY = 1.0f - (positionZ / m_TerrainHeight);

            // Determine the pixel location of the point on the mini-map.
            m_PointLocationX = m_MapLocationX + (int)(percentX * m_MapSizeX);
            m_PointLocationY = m_MapLocationY + (int)(percentY * m_MapSizeY);

            // Subtract one from the location to center the point on the mini-map according to the 3x3 point pixel image size.
            m_PointLocationX = m_PointLocationX - 1;
            m_PointLocationY = m_PointLocationY - 1;
        }
    }
}