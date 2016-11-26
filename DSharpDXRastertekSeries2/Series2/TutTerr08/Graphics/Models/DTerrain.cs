using DSharpDXRastertek.Series2.TutTerr08.System;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace DSharpDXRastertek.Series2.TutTerr08.Graphics.Models
{
    public class DTerrain
    {
        // Structs
        [StructLayout(LayoutKind.Sequential)]
        public struct DVertexType
        {
            public Vector3 position;
            public Vector2 texture;
            public Vector3 normal;
            public Vector3 tangent;
            public Vector3 binormal;
            public Vector3 color;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct DHeightMapType
        {
            public float x, y, z;
            public float tu, tv;
            public float nx, ny, nz;
            public float tx, ty, tz;
            public float bx, by, bz;
            public float r, g, b;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct DVectorTypeShareNormal
        {
            public float x, y, z;
            public float tu, tv;
            public float nx, ny, nz;
            public float r, g, b;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct DVector
        {
            public float x, y, z;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct DTempVertex
        {
            public float x, y, z;
            public float tu, tv;
            public float nx, ny, nz;
        }

        // Variables
        private int m_TerrainWidth, m_TerrainHeight, m_ColourMapWidth, m_ColourMapHeight;
        private float m_TerrainScale = 12.0f;
        private string m_TerrainHeightManName, m_ColorMapName;

        // Properties
        private SharpDX.Direct3D11.Buffer VertexBuffer { get; set; }
        private SharpDX.Direct3D11.Buffer IndexBuffer { get; set; }
        private int VertexCount { get; set; }
        public int IndexCount { get; private set; }

        public List<DHeightMapType> HeightMap = new List<DHeightMapType>();
        public DHeightMapType[] TerrainModel { get; set; }


        // Constructor
        public DTerrain() { }

        // Methods.
        public bool Initialize(SharpDX.Direct3D11.Device device, string setupFilename)
        {
            // Get the terrain filename, dimensions, and so forth from the setup file.
            if (!LoadSetupFile(setupFilename))
                return false;

            // Initialize the terrain height map with the data from the raw file.
            if (!LoadRawHeightMap())
                return false;

            // Setup the X and Z coordinates for the height map as well as scale the terrain height by the height scale value.
            SetTerrainCoordinates();

            // Calculate the normals for the terrain data.
            if (!CalculateNormals())
                return false;

            // Load in the ColorMap for the terrain
            if (!LoadColorMap())
                return false;

            if (!BuildTerrainModel())
                return false;

            // Calculate the tangent and binormal for the terrain model.
            CalculateTerrainVectors();

            // Initialize the vertex and index buffer that hold the geometry for the terrain.
            if (!InitializeBuffers(device))
                return false;
            
            return true;
        }

        private bool LoadRawHeightMap()
        {
            try
            {
                byte[] bytesData = File.ReadAllBytes(DSystemConfiguration.TextureFilePath + m_TerrainHeightManName);

                //// Create the structure to hold the height map data.
                HeightMap = new List<DHeightMapType>(m_TerrainWidth * m_TerrainHeight);

                ushort tester;
                int ind = 0, index = 0;
                // Copy the image data into the height map array.
                for (int j = 0; j < m_TerrainHeight; j++)
                {
                    for (int i = 0; i < m_TerrainWidth; i++)
                    {
                        index = (m_TerrainWidth * j) + i;

                        // Store the height at this point in the height map array.
                        tester = BitConverter.ToUInt16(bytesData, ind);

                        HeightMap.Add(new DHeightMapType()
                        {
                            x = i,
                            y = tester,
                            z = j
                        });

                        ind += 2;
                    }
                }
                bytesData = null;
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool BuildTerrainModel()
        {
            try
            {
                // Set the number of vertices in the model.
                VertexCount = (m_TerrainWidth - 1) * (m_TerrainHeight - 1) * 6;
                // Create the terrain model array.
                TerrainModel = new DHeightMapType[VertexCount];

                // Load the terrain model with the height map terrain data.
                int index = 0;
                for (int j = 0; j < (m_TerrainHeight - 1); j++)
                {
                    for (int i = 0; i < (m_TerrainWidth - 1); i++)
                    {
                        int index1 = (m_TerrainWidth * j) + i;          // Bottom left.
                        int index2 = (m_TerrainWidth * j) + (i + 1);      // Bottom right.
                        int index3 = (m_TerrainWidth * (j + 1)) + i;      // Upper left.
                        int index4 = (m_TerrainWidth * (j + 1)) + (i + 1);  // Upper right.

                        // Upper left.  index3
                        TerrainModel[index].x = HeightMap[index3].x;
                        TerrainModel[index].y = HeightMap[index3].y;
                        TerrainModel[index].z = HeightMap[index3].z;
                        TerrainModel[index].nx = HeightMap[index3].nx;
                        TerrainModel[index].ny = HeightMap[index3].ny;
                        TerrainModel[index].nz = HeightMap[index3].nz;
                        TerrainModel[index].tu = 0.0f;
                        TerrainModel[index].tv = 0.0f;
                        TerrainModel[index].r = HeightMap[index3].r;
                        TerrainModel[index].g = HeightMap[index3].g;
                        TerrainModel[index].b = HeightMap[index3].b;
                        index++;

                        // Upper right.  index4
                        TerrainModel[index].x = HeightMap[index4].x;
                        TerrainModel[index].y = HeightMap[index4].y;
                        TerrainModel[index].z = HeightMap[index4].z;
                        TerrainModel[index].nx = HeightMap[index4].nx;
                        TerrainModel[index].ny = HeightMap[index4].ny;
                        TerrainModel[index].nz = HeightMap[index4].nz;
                        TerrainModel[index].tu = 1.0f;
                        TerrainModel[index].tv = 0.0f;
                        TerrainModel[index].r = HeightMap[index4].r;
                        TerrainModel[index].g = HeightMap[index4].g;
                        TerrainModel[index].b = HeightMap[index4].b;
                        index++;

                        // Bottom left.   index1
                        TerrainModel[index].x = HeightMap[index1].x;
                        TerrainModel[index].y = HeightMap[index1].y;
                        TerrainModel[index].z = HeightMap[index1].z;
                        TerrainModel[index].nx = HeightMap[index1].nx;
                        TerrainModel[index].ny = HeightMap[index1].ny;
                        TerrainModel[index].nz = HeightMap[index1].nz;
                        TerrainModel[index].tu = 0.0f;
                        TerrainModel[index].tv = 1.0f;
                        TerrainModel[index].r = HeightMap[index1].r;
                        TerrainModel[index].g = HeightMap[index1].g;
                        TerrainModel[index].b = HeightMap[index1].b;
                        index++;

                        // Bottom left.  index1
                        TerrainModel[index].x = HeightMap[index1].x;
                        TerrainModel[index].y = HeightMap[index1].y;
                        TerrainModel[index].z = HeightMap[index1].z;
                        TerrainModel[index].nx = HeightMap[index1].nx;
                        TerrainModel[index].ny = HeightMap[index1].ny;
                        TerrainModel[index].nz = HeightMap[index1].nz;
                        TerrainModel[index].tu = 0.0f;
                        TerrainModel[index].tv = 1.0f;
                        TerrainModel[index].r = HeightMap[index1].r;
                        TerrainModel[index].g = HeightMap[index1].g;
                        TerrainModel[index].b = HeightMap[index1].b;
                        index++;

                        // Upper right.  index4
                        TerrainModel[index].x = HeightMap[index4].x;
                        TerrainModel[index].y = HeightMap[index4].y;
                        TerrainModel[index].z = HeightMap[index4].z;
                        TerrainModel[index].nx = HeightMap[index4].nx;
                        TerrainModel[index].ny = HeightMap[index4].ny;
                        TerrainModel[index].nz = HeightMap[index4].nz;
                        TerrainModel[index].tu = 1.0f;
                        TerrainModel[index].tv = 0.0f;
                        TerrainModel[index].r = HeightMap[index4].r;
                        TerrainModel[index].g = HeightMap[index4].g;
                        TerrainModel[index].b = HeightMap[index4].b;
                        index++;

                        // Bottom right.  index2
                        TerrainModel[index].x = HeightMap[index2].x;
                        TerrainModel[index].y = HeightMap[index2].y;
                        TerrainModel[index].z = HeightMap[index2].z;
                        TerrainModel[index].nx = HeightMap[index2].nx;
                        TerrainModel[index].ny = HeightMap[index2].ny;
                        TerrainModel[index].nz = HeightMap[index2].nz;
                        TerrainModel[index].tu = 1.0f;
                        TerrainModel[index].tv = 1.0f;
                        TerrainModel[index].r = HeightMap[index2].r;
                        TerrainModel[index].g = HeightMap[index2].g;
                        TerrainModel[index].b = HeightMap[index2].b;
                        index++;
                    }
                }

                // For memorys sake, clear these no longer referenced 'DHeightMapType' List since they were just passed to another array above.
                HeightMap?.Clear();
                HeightMap = null;
            }
            catch
            {
                return false;
            }

            return true;
        }
        private void CalculateTerrainVectors()
        {
            // Calculate the number of vertices in the terrain mesh.
            VertexCount = (m_TerrainWidth - 1) * (m_TerrainHeight - 1) * 6;
            // Calculate the number of faces in the terrain model.
            int faceCount = VertexCount / 3;

            // Initialize the index to the model data.
            int index = 0;
            DTempVertex vertex1, vertex2, vertex3;
            DVector tangent = new DVector();
            DVector binormal = new DVector();
           
            // Go through all the faces and calculate the the tangent, binormal, and normal vectors.
            for (int i = 0; i < faceCount; i++)
            {
                // Get the three vertices for this face from the terrain model.
                vertex1.x = TerrainModel[index].x;
                vertex1.y = TerrainModel[index].y;
                vertex1.z = TerrainModel[index].z;
                vertex1.tu = TerrainModel[index].tu;
                vertex1.tv = TerrainModel[index].tv;
                vertex1.nx = TerrainModel[index].nx;
                vertex1.ny = TerrainModel[index].ny;
                vertex1.nz = TerrainModel[index].nz;
                index++;
                vertex2.x = TerrainModel[index].x;
                vertex2.y = TerrainModel[index].y;
                vertex2.z = TerrainModel[index].z;
                vertex2.tu = TerrainModel[index].tu;
                vertex2.tv = TerrainModel[index].tv;
                vertex2.nx = TerrainModel[index].nx;
                vertex2.ny = TerrainModel[index].ny;
                vertex2.nz = TerrainModel[index].nz;
                index++;
                vertex3.x = TerrainModel[index].x;
                vertex3.y = TerrainModel[index].y;
                vertex3.z = TerrainModel[index].z;
                vertex3.tu = TerrainModel[index].tu;
                vertex3.tv = TerrainModel[index].tv;
                vertex3.nx = TerrainModel[index].nx;
                vertex3.ny = TerrainModel[index].ny;
                vertex3.nz = TerrainModel[index].nz;
                index++;

                /// MAKE SUEW that the tangent nad Binoemals calculated are being sent into the model after the method below.
                // Calculate the tangent and binormal of that face.
                CalculateTangentBinormal(vertex1, vertex2, vertex3, out tangent, out binormal);

                // Store the tangent and binormal for this face back in the model structure.
                var temp = TerrainModel[index - 1];
                temp.tx = tangent.x;
                temp.ty = tangent.y;
                temp.tz = tangent.z;
                temp.bx = binormal.x;
                temp.by = binormal.y;
                temp.bz = binormal.z;
                TerrainModel[index - 1] = temp;

                var temp2 = TerrainModel[index - 2];
                temp2.tx = tangent.x;
                temp2.ty = tangent.y;
                temp2.tz = tangent.z;
                temp2.bx = binormal.x;
                temp2.by = binormal.y;
                temp2.bz = binormal.z;
                TerrainModel[index - 2] = temp2;

                var temp3 = TerrainModel[index - 3];
                temp3.tx = tangent.x;
                temp3.ty = tangent.y;
                temp3.tz = tangent.z;
                temp3.bx = binormal.x;
                temp3.by = binormal.y;
                temp3.bz = binormal.z;
                TerrainModel[index - 3] = temp3;
            }
        }
        private void CalculateTangentBinormal(DTempVertex vertex1, DTempVertex vertex2, DTempVertex vertex3, out DVector tangent, out DVector binormal)
        {
            float[] vector1 = new float[3];
            float[] vector2 = new float[3];
            float[] tuVector = new float[2];
            float[] tvVector = new float[2];

            // Calculate the two vectors for this face.
            vector1[0] = vertex2.x - vertex1.x;
            vector1[1] = vertex2.y - vertex1.y;
            vector1[2] = vertex2.z - vertex1.z;
            vector2[0] = vertex3.x - vertex1.x;
            vector2[1] = vertex3.y - vertex1.y;
            vector2[2] = vertex3.z - vertex1.z;

            // Calculate the tu and tv texture space vectors.
            tuVector[0] = vertex2.tu - vertex1.tu;
            tvVector[0] = vertex2.tv - vertex1.tv;
            tuVector[1] = vertex3.tu - vertex1.tu;
            tvVector[1] = vertex3.tv - vertex1.tv;

            // Calculate the denominator of the tangent/binormal equation.
            float den = 1.0f / (tuVector[0] * tvVector[1] - tuVector[1] * tvVector[0]);

            // Calculate the cross products and multiply by the coefficient to get the tangent and binormal.
            tangent.x = (tvVector[1] * vector1[0] - tvVector[0] * vector2[0]) * den;
            tangent.y = (tvVector[1] * vector1[1] - tvVector[0] * vector2[1]) * den;
            tangent.z = (tvVector[1] * vector1[2] - tvVector[0] * vector2[2]) * den;
            binormal.x = (tuVector[0] * vector2[0] - tuVector[1] * vector1[0]) * den;
            binormal.y = (tuVector[0] * vector2[1] - tuVector[1] * vector1[1]) * den;
            binormal.z = (tuVector[0] * vector2[2] - tuVector[1] * vector1[2]) * den;

            // Calculate the length of this normal.
            float length = (float)Math.Sqrt((tangent.x * tangent.x) + (tangent.y * tangent.y) + (tangent.z * tangent.z));

            // Normalize the normal and then store it
            binormal.x = binormal.x / length;
            binormal.y = binormal.y / length;
            binormal.z = binormal.z / length;
        }

        private bool LoadColorMap()
        {
            Bitmap colourBitMap;

            try
            {
                // Open the color map file in binary.
                colourBitMap = new Bitmap(DSystemConfiguration.TextureFilePath + m_ColorMapName);
            }
            catch
            {
                return false;
            }

            // This check is optional.
            // Make sure the color map dimensions are the same as the terrain dimensions for easy 1 to 1 mapping.
            m_ColourMapWidth = colourBitMap.Width;
            m_ColourMapHeight = colourBitMap.Height;
            if ((m_ColourMapWidth != m_TerrainWidth) || (m_ColourMapHeight != m_TerrainHeight))
                return false;

            // Read the image data into the color map portion of the height map structure.
            int index;
            for (int j = 0; j < m_ColourMapHeight; j++)
                for (int i = 0; i < m_ColourMapWidth; i++)
                {
                    index = (m_ColourMapHeight * j) + i;

                    DHeightMapType tempCopy = HeightMap[index];
                    tempCopy.r = colourBitMap.GetPixel(i, j).R / 255.0f; // 117.75f; //// 0.678431392
                    tempCopy.g = colourBitMap.GetPixel(i, j).G / 255.0f;  //117.75f; // 0.619607866
                    tempCopy.b = colourBitMap.GetPixel(i, j).B / 255.0f;  // 117.75f; // 0.549019635
                    HeightMap[index] = tempCopy;
                }

            // Release the bitmap image data.
            colourBitMap.Dispose();
            colourBitMap = null;

            return true;
        }
        private bool CalculateNormals()
        {
            // Create a temporary array to hold the un-normalized normal vectors.
            int index;
            float length;
            Vector3 vertex1, vertex2, vertex3, vector1, vector2, sum;
            DVectorTypeShareNormal[] normals = new DVectorTypeShareNormal[(m_TerrainHeight - 1) * (m_TerrainWidth - 1)];

            // Go through all the faces in the mesh and calculate their normals.
            for (int j = 0; j < (m_TerrainHeight - 1); j++)
            {
                for (int i = 0; i < (m_TerrainWidth - 1); i++)
                {
                    int index1 = (j * m_TerrainHeight) + i;
                    int index2 = (j * m_TerrainHeight) + (i + 1);
                    int index3 = ((j + 1) * m_TerrainHeight) + i;

                    // Get three vertices from the face.
                    vertex1.X = HeightMap[index1].x;
                    vertex1.Y = HeightMap[index1].y;
                    vertex1.Z = HeightMap[index1].z;
                    vertex2.X = HeightMap[index2].x;
                    vertex2.Y = HeightMap[index2].y;
                    vertex2.Z = HeightMap[index2].z;
                    vertex3.X = HeightMap[index3].x;
                    vertex3.Y = HeightMap[index3].y;
                    vertex3.Z = HeightMap[index3].z;

                    // Calculate the two vectors for this face.
                    vector1 = vertex1 - vertex3;
                    vector2 = vertex3 - vertex2;

                    index = (j * (m_TerrainHeight - 1)) + i;

                    // Calculate the cross product of those two vectors to get the un-normalized value for this face normal.
                    Vector3 vecTestCrossProduct = Vector3.Cross(vector1, vector2);
                    normals[index].x = vecTestCrossProduct.X;
                    normals[index].y = vecTestCrossProduct.Y;
                    normals[index].z = vecTestCrossProduct.Z;
                }
            }

            // Now go through all the vertices and take an average of each face normal 	
            // that the vertex touches to get the averaged normal for that vertex.
            for (int j = 0; j < m_TerrainHeight; j++)
            {
                for (int i = 0; i < m_TerrainWidth; i++)
                {
                    // Initialize the sum.
                    sum = Vector3.Zero;

                    // Initialize the count.
                    int count = 9;

                    // Bottom left face.
                    if (((i - 1) >= 0) && ((j - 1) >= 0))
                    {
                        index = ((j - 1) * (m_TerrainHeight - 1)) + (i - 1);

                        sum[0] += normals[index].x;
                        sum[1] += normals[index].y;
                        sum[2] += normals[index].z;
                        count++;
                    }
                    // Bottom right face.
                    if ((i < (m_TerrainWidth - 1)) && ((j - 1) >= 0))
                    {
                        index = ((j - 1) * (m_TerrainHeight - 1)) + i;

                        sum[0] += normals[index].x;
                        sum[1] += normals[index].y;
                        sum[2] += normals[index].z;
                        count++;
                    }
                    // Upper left face.
                    if (((i - 1) >= 0) && (j < (m_TerrainHeight - 1)))
                    {
                        index = (j * (m_TerrainHeight - 1)) + (i - 1);

                        sum[0] += normals[index].x;
                        sum[1] += normals[index].y;
                        sum[2] += normals[index].z;
                        count++;
                    }
                    // Upper right face.
                    if ((i < (m_TerrainWidth - 1)) && (j < (m_TerrainHeight - 1)))
                    {
                        index = (j * (m_TerrainHeight - 1)) + i;

                        sum.X += normals[index].x;
                        sum.Y += normals[index].y;
                        sum.Z += normals[index].z;
                        count++;
                    }

                    // Take the average of the faces touching this vertex.
                    sum.X = (sum.X / (float)count);
                    sum.Y = (sum.Y / (float)count);
                    sum.Z = (sum.Z / (float)count);

                    // Calculate the length of this normal.
                    length = (float)Math.Sqrt((sum.X * sum.X) + (sum.Y * sum.Y) + (sum.Z * sum.Z));

                    // Get an index to the vertex location in the height map array.
                    index = (j * m_TerrainHeight) + i;

                    // Normalize the final shared normal for this vertex and store it in the height map array.
                    DHeightMapType editHeightMap = HeightMap[index];
                    editHeightMap.nx = (sum.X / length);
                    editHeightMap.ny = (sum.Y / length);
                    editHeightMap.nz = (sum.Z / length);
                    HeightMap[index] = editHeightMap;
                }
            }

            // Release the temporary normals.
            normals = null;

            return true;
        }
        private bool LoadSetupFile(string setupFilename)
        {
            // Open the setup file.  If it could not open the file then exit.
            setupFilename = DSystemConfiguration.DataFilePath + setupFilename;

            // Get all the lines containing the font data.
            var setupLines = File.ReadAllLines(setupFilename);

            // Read in the terrain file name.
            m_TerrainHeightManName = setupLines[0].Trim("Terrain Filename: ".ToCharArray());
            // Read in the terrain height & width.
            m_TerrainHeight = int.Parse(setupLines[1].Trim("Terrain Height: ".ToCharArray()));
            m_TerrainWidth = int.Parse(setupLines[2].Trim("Terrain Width: ".ToCharArray()));
            // Read in the terrain height scaling.
            m_TerrainScale = float.Parse(setupLines[3].Trim("Terrain Scaling: ".ToCharArray()));
            // Read in the ColorMap File Name.
            m_ColorMapName = setupLines[4].TrimStart("Color Map Filename: ".ToCharArray());

            return true;
        }
        private void SetTerrainCoordinates()
        {
            for (var i = 0; i < HeightMap.Count; i++)
            {
                var temp = HeightMap[i];
                temp.y /= m_TerrainScale;
                HeightMap[i] = temp;
            }
        }
        private bool InitializeBuffers(SharpDX.Direct3D11.Device device)
        {
            try
            {
                // Calculate the number of vertices in the terrain mesh.
                VertexCount = (m_TerrainWidth - 1) * (m_TerrainHeight - 1) * 6;
                // Set the index count to the same as the vertex count.
                IndexCount = VertexCount;

                // Create the vertex array.
                DVertexType[] vertices = new DVertexType[VertexCount];
                // Create the index array.
                int[] indices = new int[IndexCount];

                // Load the vertex and index arrays with the terrain data.
                for (var i = 0; i < VertexCount; i++)
                {
                    vertices[i] = new DVertexType()
                    {
                        position = new Vector3(TerrainModel[i].x, TerrainModel[i].y, TerrainModel[i].z),
                        texture = new Vector2(TerrainModel[i].tu, TerrainModel[i].tv),
                        normal = new Vector3(TerrainModel[i].nx, TerrainModel[i].ny, TerrainModel[i].nz),
                        tangent = new Vector3(TerrainModel[i].tx, TerrainModel[i].ty, TerrainModel[i].tz),
                        binormal = new Vector3(TerrainModel[i].bx, TerrainModel[i].by, TerrainModel[i].bz),
                        color = new Vector3(TerrainModel[i].r, TerrainModel[i].g, TerrainModel[i].b)
                    };

                    indices[i] = i;
                }

                // Yo AVOID OutOfMemory Exceptions being trown first,  release the height map since it is no longer needed in memory once the 3D terrain model has been built.
                ShutdownHeightMap();

                // Create the vertex buffer.
                VertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);

                // Create the index buffer.
                IndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indices);

                // Release the arrays now that the buffers have been created and loaded.
                vertices = null;
                indices = null;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
        public void ShutDown()
        {
            // Release the vertex and index buffers.
            ShutdownBuffers();
            // Release the height map.
            ShutdownHeightMap();
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
        private void ShutdownHeightMap()
        {
            // Release the height map array.    
            TerrainModel = null;
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
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }
    }
}