using DSharpDXRastertek.TutTerr09.Graphics.Shaders;
using DSharpDXRastertek.TutTerr09.System;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace DSharpDXRastertek.TutTerr09.Graphics.Models
{
    public class DTerrain                   // 662 lines
    {
        // Structs
        [StructLayout(LayoutKind.Sequential)]
        public struct DVertexType
        {
            public Vector3 position;
            public Vector2 texture;
            public Vector3 normal;
            public Vector4 color;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct DHeightMapType
        {
            public float x, y, z;
            public float nx, ny, nz;
            public float r, g, b;
            public int rIndex, gIndex, bIndex;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct DVectorTypeShareNormal
        {
            public float x, y, z;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct DMaterialGroupType
        {
            public int textureIndex1, textureIndex2, alphaIndex;
            public int red, green, blue;
            public SharpDX.Direct3D11.Buffer vertexBuffer, indexBuffer;
            public int vertexCount, indexCount;
            public DVertexType[] vertices;
            public int[] indices;
        }

        // Variables
        private int m_TerrainWidth, m_TerrainHeight, m_ColourMapWidth, m_ColourMapHeight;
        private int m_TextureCount, m_MaterialCount;

        // Properties                         s
        private int VertexCount { get; set; }
        public int IndexCount { get; private set; }
        public List<DHeightMapType> HeightMap = new List<DHeightMapType>();
        public DTexture[] Textures { get; set; }
        public DMaterialGroupType[] Materials { get; set; }

        // Constructor
        public DTerrain() { }

        // Methods.
        public bool Initialize(SharpDX.Direct3D11.Device device, string heightMapFilename, string materialsFilename, string materialMapFilename, string colorMapFilename)
        {
            // Load in the height map for the terrain.
            if (!LoadHeightMap(heightMapFilename))
                return false;

            // Normalize the height of the height map.
            NormalizeHeightMap();

            // Calculate the normals for the terrain data.
            if (!CalculateNormals())
                return false;

            // Load in the color map for the terrain.
            if (!LoadColourMap(device, colorMapFilename))
                return false;

            // Initialize the material groups for the terrain.
            if (!LoadMaterialFile(materialsFilename, materialMapFilename, device))
                return false;
             
            return true;
        }
        private bool LoadMaterialFile(string filename, string materialMapFilename, SharpDX.Direct3D11.Device device)
        {
            // Open the materials information text file.
            filename = DSystemConfiguration.ModelFilePath + filename;

            // Get all the lines containing the font data.
            string[] lines = File.ReadAllLines(filename);

            // Load each of the textures in.
            int textureIndex = 0, linesProcessed = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                // Short circuit any superfluous lines
                if (!lines[i].EndsWith(".bmp") && m_TextureCount > 0)
                    continue;

                // Read in the texture count.
                if (lines[i].StartsWith("Texture Count:") && Textures == null)
                {
                    string textureCount =  lines[i].Split(new char[]{ ':' })[i + 1].Trim();
                    m_TextureCount = int.Parse(textureCount);

                    //// Create the texture object array.
                    Textures = new DTexture[m_TextureCount];

                    // Therre is nothing further for this line to process so continue on.
                    continue;
                } 

                // Split this line with a file path into segments.
                var lineSegments = lines[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                // Check each segment of this line for a file path with a .dds file in it.
                foreach (string segment in lineSegments)
                    if (segment.EndsWith(".bmp") && segment.StartsWith("..") && segment.Contains("/"))
                    {
                        // we have a filepath to load in.
                        int startIndex = segment.LastIndexOf('/');
                        string trimmed = segment.Substring(startIndex + 1);

                        Textures[textureIndex] = new DTexture();

                        // Convert the character filename to WCHAR.
                        // Load the texture or alpha map.
                        if (!Textures[textureIndex++].Initialize(device, DSystemConfiguration.DataFilePath + trimmed))
                            return false;
                    }

                // check that we are not going to exceed the count of textures in the array and the ones read in from the file. if the array is full then stop line parsing execution.
                if (textureIndex == m_TextureCount)
                {
                    linesProcessed = i + 1;
                    break;
                }
            }

            // Load each of the material group indexes in.
            int materialIndex = 0;
            for (int i = linesProcessed; i < lines.Length; i++)
            {
                // Short circuit any superfluous lines
                if (string.IsNullOrEmpty(lines[i]) || !lines[i].EndsWith("0") && !lines[i].EndsWith("255") && !lines[i].EndsWith("128") && m_MaterialCount > 0)
                    continue;

                // Read in the material count.
                if (lines[i].StartsWith("Material Count:") && Materials == null)
                {
                    string materialCount = lines[i].Split(new char[] { ':' })[i - linesProcessed].Trim();
                    m_MaterialCount = int.Parse(materialCount);

                    //// Create the texture object array.
                    Materials = new DMaterialGroupType[m_MaterialCount];

                    // Therre is nothing further for this line to process so continue on.
                    continue;
                }

                // Split this line with a file path into segments.
                string[] lineSegments = lines[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                // Create the material group array.
                Materials[materialIndex] = new DMaterialGroupType();

                // Load each of the material group indexes in.
                int result;
                if (int.TryParse(lineSegments[1], out result))
                    Materials[materialIndex].textureIndex1 = result;
                if (int.TryParse(lineSegments[2], out result))
                    Materials[materialIndex].textureIndex2 = result;
                if (int.TryParse(lineSegments[3], out result))
                    Materials[materialIndex].alphaIndex = result;
                if (int.TryParse(lineSegments[4], out result))
                    Materials[materialIndex].red = result;
                if (int.TryParse(lineSegments[5], out result))
                    Materials[materialIndex].green = result;
                if (int.TryParse(lineSegments[6], out result))
                    Materials[materialIndex].blue = result;

                // Increase Material index now for the next one in the array to create for next line.
                materialIndex++;

                // check that we are not going to exceed the count of textures in the array and the ones read in from the file. if the array is full then stop line parsing execution.
                if (materialIndex == m_MaterialCount)
                    break;      
            }
          
            // Close the materials information text file.
            // Now load the material index map.
            if (!LoadMaterialMap(materialMapFilename))
                return false;

            // Load the vertex buffer for each material group with the terrain data.
            if (!LoadMaterialBuffers(device))
                return false;

            return true;
        }
        private bool LoadMaterialBuffers(SharpDX.Direct3D11.Device device)
        {
            // Create the value for the maximum number of vertices a material group could possibly have.
            int maxVertexCount = (m_TerrainWidth - 1) * (m_TerrainHeight - 1) * 6;
            // Set the index count to the same as the maximum vertex count.
            int maxIndexCount = maxVertexCount;

            // Initialize vertex and index arrays for each material group to the maximum size.
            for (int i = 0; i < m_MaterialCount; i++)
            {
                // Create the temporary vertex array for this material group.
                Materials[i].vertices = new DVertexType[maxVertexCount];

                // Create the temporary index array for this material group.
                Materials[i].indices = new int[maxIndexCount];

                // Initialize the counts to zero.
                Materials[i].vertexCount = 0;
                Materials[i].indexCount = 0;
            }

            // Now loop through the terrain and build the vertex arrays for each material group.
            for (int j = 0; j < (m_TerrainHeight - 1); j++)
            {
                for (int i = 0; i < (m_TerrainWidth - 1); i++)
                {
                    int index1 = (m_TerrainHeight * j) + i;          // Bottom left.
                    int index2 = (m_TerrainHeight * j) + (i + 1);      // Bottom ri ght.
                    int index3 = (m_TerrainHeight * (j + 1)) + i;      // Upper left.
                    int index4 = (m_TerrainHeight * (j + 1)) + (i + 1);  // Upper right.

                    // Query the upper left corner vertex for the material index.
                    int redIndex = HeightMap[index3].rIndex;
                    int greenIndex = HeightMap[index3].gIndex;
                    int blueIndex = HeightMap[index3].bIndex;

                    // Find which material group this vertex belongs to.
                    int index = 0;
                    bool found = false;
                    while (!found)
                    {
                        if ((redIndex == Materials[index].red) && (greenIndex == Materials[index].green) && (blueIndex == Materials[index].blue))
                            found = true;
                        else
                            index++;
                    }

                    // Set the index position in the vertex and index array to the count.
                    int vIndex = Materials[index].vertexCount;

                    // Upper left.
                    Materials[index].vertices[vIndex].position = new Vector3(HeightMap[index3].x, HeightMap[index3].y, HeightMap[index3].z);
                    Materials[index].vertices[vIndex].texture = new Vector2(0.0f, 0.0f);
                    Materials[index].vertices[vIndex].normal = new Vector3(HeightMap[index3].nx, HeightMap[index3].ny, HeightMap[index3].nz);
                    Materials[index].vertices[vIndex].color = new Vector4(HeightMap[index3].r, HeightMap[index3].g, HeightMap[index3].b, 1.0f);
                    Materials[index].indices[vIndex] = vIndex;
                    vIndex++;

                    // Upper right.
                    Materials[index].vertices[vIndex].position = new Vector3(HeightMap[index4].x, HeightMap[index4].y, HeightMap[index4].z);
                    Materials[index].vertices[vIndex].texture = new Vector2(1.0f, 0.0f);
                    Materials[index].vertices[vIndex].normal = new Vector3(HeightMap[index4].nx, HeightMap[index4].ny, HeightMap[index4].nz);
                    Materials[index].vertices[vIndex].color = new Vector4(HeightMap[index4].r, HeightMap[index4].g, HeightMap[index4].b, 1.0f);
                    Materials[index].indices[vIndex] = vIndex;
                    vIndex++;

                    // Bottom left.
                    Materials[index].vertices[vIndex].position = new Vector3(HeightMap[index1].x, HeightMap[index1].y, HeightMap[index1].z);
                    Materials[index].vertices[vIndex].texture = new Vector2(0.0f, 1.0f);
                    Materials[index].vertices[vIndex].normal = new Vector3(HeightMap[index1].nx, HeightMap[index1].ny, HeightMap[index1].nz);
                    Materials[index].vertices[vIndex].color = new Vector4(HeightMap[index1].r, HeightMap[index1].g, HeightMap[index1].b, 1.0f);
                    Materials[index].indices[vIndex] = vIndex;
                    vIndex++;

                    // Bottom left.
                    Materials[index].vertices[vIndex].position = new Vector3(HeightMap[index1].x, HeightMap[index1].y, HeightMap[index1].z);
                    Materials[index].vertices[vIndex].texture = new Vector2(0.0f, 1.0f);
                    Materials[index].vertices[vIndex].normal = new Vector3(HeightMap[index1].nx, HeightMap[index1].ny, HeightMap[index1].nz);
                    Materials[index].vertices[vIndex].color = new Vector4(HeightMap[index1].r, HeightMap[index1].g, HeightMap[index1].b, 1.0f);
                    Materials[index].indices[vIndex] = vIndex;
                    vIndex++;

                    // Upper right.
                    Materials[index].vertices[vIndex].position = new Vector3(HeightMap[index4].x, HeightMap[index4].y, HeightMap[index4].z);
                    Materials[index].vertices[vIndex].texture = new Vector2(1.0f, 0.0f);
                    Materials[index].vertices[vIndex].normal = new Vector3(HeightMap[index4].nx, HeightMap[index4].ny, HeightMap[index4].nz);
                    Materials[index].vertices[vIndex].color = new Vector4(HeightMap[index4].r, HeightMap[index4].g, HeightMap[index4].b, 1.0f);
                    Materials[index].indices[vIndex] = vIndex;
                    vIndex++;

                    // Bottom right.
                    Materials[index].vertices[vIndex].position = new Vector3(HeightMap[index2].x, HeightMap[index2].y, HeightMap[index2].z);
                    Materials[index].vertices[vIndex].texture = new Vector2(1.0f, 1.0f);
                    Materials[index].vertices[vIndex].normal = new Vector3(HeightMap[index2].nx, HeightMap[index2].ny, HeightMap[index2].nz);
                    Materials[index].vertices[vIndex].color = new Vector4(HeightMap[index2].r, HeightMap[index2].g, HeightMap[index2].b, 1.0f);
                    Materials[index].indices[vIndex] = vIndex;
                    vIndex++;

                    // Increment the vertex and index array counts.
                    Materials[index].vertexCount += 6;
                    Materials[index].indexCount += 6;
                }
            }

            // Now create the vertex and index buffers from the vertex and index arrays for each material group.
            for (int i = 0; i < m_MaterialCount; i++)
            {
                // Ensure that the Vertex Buffers are actually default and not dynamic here.
                BufferDescription vertexBufferDesc = new BufferDescription()
                {
                    Usage = ResourceUsage.Default,
                    SizeInBytes = Utilities.SizeOf<DVertexType>() * Materials[i].vertexCount,
                    BindFlags = BindFlags.VertexBuffer,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                };

                // Create the vertex buffer.
                Materials[i].vertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, Materials[i].vertices);

                // Create the index buffer.
                Materials[i].indexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, Materials[i].indices);

                // Remove verticies and Indicies.
                Materials[i].vertices = null;
                Materials[i].indices = null;
            }

            return true;
        }
        private bool LoadMaterialMap(string materialMapFilename)
        {
            Bitmap materialBitMap;

            try
            {
                // Open the color map file in binary.
                materialBitMap = new Bitmap(DSystemConfiguration.DataFilePath + materialMapFilename);
            }
            catch
            {
                return false;
            }

            // Make sure the material index map dimensions are the same as the terrain dimensions for 1 to 1 mapping.
            if ((materialBitMap.Width != m_TerrainWidth) || (materialBitMap.Height != m_TerrainHeight))
                return false;

            // Initialize the position in the image data buffer so each vertice has an material index associated with it.
            // Read the material index data into the height map structure.
            int index;
            for (int j = 0; j < m_TerrainHeight; j++)
                for (int i = 0; i < m_TerrainWidth; i++)
                {
                    index = (m_TerrainHeight * j) + i;

                    DHeightMapType tempCopy = HeightMap[index];
                    tempCopy.rIndex = (int)materialBitMap.GetPixel(i, j).R;
                    tempCopy.gIndex = (int)materialBitMap.GetPixel(i, j).G;
                    tempCopy.bIndex = (int)materialBitMap.GetPixel(i, j).B;
                    HeightMap[index] = tempCopy;
                }

            // Release the bitmap image data.
            materialBitMap.Dispose();
            materialBitMap = null;

            return true;
        }
        private bool LoadColourMap(SharpDX.Direct3D11.Device device, string colorMapFilename)
        {
            Bitmap colourBitMap;

            try
            {
                // Open the color map file in binary.
                colourBitMap = new Bitmap(DSystemConfiguration.DataFilePath + colorMapFilename);
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
                    tempCopy.r = colourBitMap.GetPixel(i, j).R /  255.0f; // 117.75f; //// 0.678431392
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
            HeightMap = new List<DHeightMapType>(m_TerrainWidth * m_TerrainHeight);

            // Read the image data into the height map
            for (var j = 0; j < m_TerrainHeight; j++)
                for (var i = 0; i < m_TerrainWidth; i++)
                    HeightMap.Add(new DHeightMapType()
                    {
                        x = (float)i,
                        y = (float)bitmap.GetPixel(i, j).R,
                        z = (float)j
                    });

            // Release the bitmap image data.
            bitmap.Dispose();
            bitmap = null;

            return true;
        }
        public void ShutDown()
        {
            // Release the materials for the terrain.
            ReleaseMaterials();

            // Release the height map data.
            ShutdownHeightMap();
        }
        private void ReleaseMaterials()
        {
            // Release the material groups.
            if (Materials != null)
            {
                for (int i = 0; i < m_MaterialCount; i++)
                {
                        Materials[i].vertexBuffer?.Dispose();
                        Materials[i].vertexBuffer = null;
                        Materials[i].indexBuffer?.Dispose();
                        Materials[i].indexBuffer = null;
                        Materials[i].vertices = null;
                        Materials[i].indices = null;
                }

                Materials = null;
            }
            // Release the terrain textures and alpha maps.
            if (Textures != null)
            {
                for (int i = 0; i < Textures.Length; i++)
                {
                    Textures[i]?.ShutDown();
                    Textures[i] = null;
                }
                Textures = null;
            }
        }
        private void ShutdownHeightMap()
        {
            // Release the HeightMap Data loaded from the file.
            HeightMap?.Clear();
            HeightMap = null;
        }
        public bool Render(DeviceContext deviceContext, DTerrainShader shader, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, Vector4 ambiantColour, Vector4 diffuseColour, Vector3 lightDirection)
        {
            // Set vertex buffer stride and offset.
            // Set the shader parameters that it will use for rendering.
            if (!shader.SetShaderParameters(deviceContext, worldMatrix, viewMatrix, projectionMatrix, ambiantColour, diffuseColour, lightDirection))
                return false;

            // Set the type of primitive that should be rendered from the vertex buffers, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            // Render each material group.
            for (int i = 0; i < m_MaterialCount; i++)
            {
                // Set the vertex buffer to active in the input assembler so it can be rendered.
                deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(Materials[i].vertexBuffer, Utilities.SizeOf<DVertexType>(), 0));

                // Set the index buffer to active in the input assembler so it can be rendered.
                deviceContext.InputAssembler.SetIndexBuffer(Materials[i].indexBuffer, Format.R32_UInt, 0);

                // If the material group has a valid second texture index then this is a blended terrain polygon.
                bool result;
                if (Materials[i].textureIndex2 != -1)
                {
                    result = shader.SetShaderTextures(deviceContext, Textures[Materials[i].textureIndex1].TextureResource, Textures[Materials[i].textureIndex2].TextureResource, Textures[Materials[i].alphaIndex].TextureResource, false);
                }
                else  // If not then it is just a single textured polygon.
                    result = shader.SetShaderTextures(deviceContext, Textures[Materials[i].textureIndex1].TextureResource, null, null, false);
                
                // Check if the textures were set or not.
                if (!result)
                    return false;

                // Now render the prepared buffers with the shader.
                shader.RenderShader(deviceContext, Materials[i].indexCount);
            }

            return true;
        }
    }
}