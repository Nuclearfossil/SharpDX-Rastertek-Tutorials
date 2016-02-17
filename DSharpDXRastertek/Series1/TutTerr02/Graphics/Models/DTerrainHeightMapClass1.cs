using DSharpDXRastertek.TutTerr02.System;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace DSharpDXRastertek.TutTerr02.Graphics.Models
{
    public class DTerrainHeightMap                  // 273 lines
    {
        // Structs
        [StructLayout(LayoutKind.Sequential)]
        internal struct DVertexType
        {
            internal Vector3 position;
            internal Vector4 color;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct DHeightMapType
        {
            public float x, y, z;
        }

        // Variables
        private int m_TerrainWidth, m_TerrainHeight;

        // Properties
        private SharpDX.Direct3D11.Buffer VertexBuffer { get; set; }
        private SharpDX.Direct3D11.Buffer IndexBuffer { get; set; }
        private int VertexCount { get; set; }
        public int IndexCount { get; private set; }
        public List<DHeightMapType> HeightMap = new List<DHeightMapType>();

        // Constructor
        public DTerrainHeightMap() { }

        // Methods.
        public bool Initialize(SharpDX.Direct3D11.Device device, string heightMapFileName)
        {
            // Load in the height map for the terrain.
            if (!LoadHeightMap(heightMapFileName))
                return false;

            // Normalize the height of the height map.
            NormalizeHeightMap();

            // Initialize the vertex and index buffer that hold the geometry for the terrain.
            if (!InitializeBuffers(device))
                return false;

            return true;
        }
        private void NormalizeHeightMap()
        {
            for (var i = 0; i < HeightMap.Count; i++)
			{
				var temp = HeightMap[i];
				temp.y /= 15;
				HeightMap[i] = temp;
			}
        }
        private bool LoadHeightMap(string heightMapFileName)
        {
            Bitmap bitmap;

            try
            {
                // Open the height map file in binary.
                bitmap = new Bitmap(DSystemConfiguration.DataFilePath + heightMapFileName);
            }
            catch
            {
                return false;
            }

            // Save the dimensions of the terrain.
            m_TerrainWidth = bitmap.Width;
            m_TerrainHeight = bitmap.Height;

            // Create the structure to hold the height map data.
            // HeightMap = new DHeightMapType[imsamgeSize];
            HeightMap = new List<DHeightMapType>(m_TerrainWidth * m_TerrainHeight);

            // Read the image data into the height map
            for (var j = 0; j < m_TerrainHeight; j++)
                for (var i = 0; i < m_TerrainWidth; i++)
                    HeightMap.Add(new DHeightMapType()
                    {
                        x = i,
                        y = bitmap.GetPixel(i, j).R,
                        z = j
                    });

            return true;
        }
        private bool InitializeBuffers(SharpDX.Direct3D11.Device device)
        {
            try
            {
                // Calculate the number of vertices in the terrain mesh. 2 verticies per line, 3 lines per triangle and 2 trangles per quad is 12 verticies per quad.
                VertexCount = (m_TerrainWidth - 1) * (m_TerrainHeight - 1) * 12;
                // Set the index count to the same as the vertex count.
                IndexCount = VertexCount;

                // Create the vertex array.
                DVertexType[] vertices = new DVertexType[VertexCount];                
                // Create the index array.
                int[] indices = new int[IndexCount];

                // Initialize the index to the vertex array.
                int index = 0;       
                // Load the vertex and index arrays with the terrain data.
                for (int j = 0; j < (m_TerrainHeight - 1); j++)
                {
                    for (int i = 0; i < (m_TerrainWidth - 1); i++)
                    {
                        int indexBottomLeft1 = (m_TerrainHeight * j) + i;          // Bottom left.
                        int indexBottomRight2 = (m_TerrainHeight * j) + (i + 1);      // Bottom right.
                        int indexUpperLeft3 = (m_TerrainHeight * (j + 1)) + i;      // Upper left.
                        int indexUpperRight4 = (m_TerrainHeight * (j + 1)) + (i + 1);  // Upper right.

                        #region First Triangle
                        // Upper left.
                        vertices[index] = new DVertexType()
                        {
                            position = new Vector3(HeightMap[indexUpperLeft3].x, HeightMap[indexUpperLeft3].y, HeightMap[indexUpperLeft3].z),
                            color = Vector4.One
                        };
                        indices[index] = index++;
                        // Upper right.
                        vertices[index] = new DVertexType()
                        {
                            position = new Vector3(HeightMap[indexUpperRight4].x, HeightMap[indexUpperRight4].y, HeightMap[indexUpperRight4].z),
                            color = Vector4.One
                        };
                        indices[index] = index++;
                        // Upper right.
                        vertices[index] = new DVertexType()
                        {
                            position = new Vector3(HeightMap[indexUpperRight4].x, HeightMap[indexUpperRight4].y, HeightMap[indexUpperRight4].z),
                            color = Vector4.One
                        };
                        indices[index] = index++;
                        // Bottom left.
                        vertices[index] = new DVertexType()
                        {
                            position = new Vector3(HeightMap[indexBottomLeft1].x, HeightMap[indexBottomLeft1].y, HeightMap[indexBottomLeft1].z),
                            color = Vector4.One
                        };
                        indices[index] = index++;
                        // Bottom left.
                        vertices[index] = new DVertexType()
                        {
                            position = new Vector3(HeightMap[indexBottomLeft1].x, HeightMap[indexBottomLeft1].y, HeightMap[indexBottomLeft1].z),
                            color = Vector4.One
                        };
                        indices[index] = index++;
                        // Upper left.
                        vertices[index] = new DVertexType()
                        {
                            position = new Vector3(HeightMap[indexUpperLeft3].x, HeightMap[indexUpperLeft3].y, HeightMap[indexUpperLeft3].z),
                            color = Vector4.One
                        };
                        indices[index] = index++;
                        #endregion

                        #region Second Triangle
                        // Bottom Left
                        vertices[index] = new DVertexType()
                        {
                            position = new Vector3(HeightMap[indexBottomLeft1].x, HeightMap[indexBottomLeft1].y, HeightMap[indexBottomLeft1].z),
                            color = Vector4.One
                        };
                        indices[index] = index++;
                        // Upper right.
                        vertices[index] = new DVertexType()
                        {
                            position = new Vector3(HeightMap[indexUpperRight4].x, HeightMap[indexUpperRight4].y, HeightMap[indexUpperRight4].z),
                            color = Vector4.One
                        };
                        indices[index] = index++;
                        // Upper right.
                        vertices[index] = new DVertexType()
                        {
                            position = new Vector3(HeightMap[indexUpperRight4].x, HeightMap[indexUpperRight4].y, HeightMap[indexUpperRight4].z),
                            color = Vector4.One
                        };
                        indices[index] = index++;
                        // Bottom right.
                        vertices[index] = new DVertexType()
                        {
                            position = new Vector3(HeightMap[indexBottomRight2].x, HeightMap[indexBottomRight2].y, HeightMap[indexBottomRight2].z),
                            color = Vector4.One
                        };
                        indices[index] = index++;
                        // Bottom right.
                        vertices[index] = new DVertexType()
                        {
                            position = new Vector3(HeightMap[indexBottomRight2].x, HeightMap[indexBottomRight2].y, HeightMap[indexBottomRight2].z),
                            color = Vector4.One
                        };
                        indices[index] = index++;
                        // Bottom left.
                        vertices[index] = new DVertexType()
                        {
                            position = new Vector3(HeightMap[indexBottomLeft1].x, HeightMap[indexBottomLeft1].y, HeightMap[indexBottomLeft1].z),
                            color = Vector4.One
                        };
                        indices[index] = index++;
                        #endregion
                    }
                }

                // Create the vertex buffer.
                VertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);

                // Create the index buffer.
                IndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indices);

                // Release the arrays now that the buffers have been created and loaded.
                vertices = null;
                indices = null;

                return true;
            }
            catch
            {
                return false;
            }
        }
       
        public void ShutDown()
        {
            // Release the vertex and index buffers.
            ShutdownBuffers();

            // Release the height map data.
            ShutdownHeightMap();
        }
        private void ShutdownHeightMap()
        {
            // Release the HeightMap Data loaded from the file.
            HeightMap?.Clear();
            HeightMap = null;
        }
        private void ShutdownBuffers()
        {
            // Return the index buffer.
            IndexBuffer?.Dispose();
            IndexBuffer = null;
            // Release the vertex buffer.
            VertexBuffer?.Dispose();
            VertexBuffer = null;
        }
        public void Render(DeviceContext deviceContext)
        {
            // Put the vertex and index buffers on the graphics pipeline to prepare them for drawing.
            RenderBuffers(deviceContext);
        }
        private void RenderBuffers(DeviceContext deviceContext)
        {
            // Set the vertex buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<DVertexType>(), 0));
            // Set the index buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
        }
    }
}