using DSharpDXRastertek.Series2.TutTerr13.Graphics.Data;
using DSharpDXRastertek.Series2.TutTerr13.System;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace DSharpDXRastertek.Series2.TutTerr13.Graphics.Models
{
    public class DTerrain
    {
        // Structs
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
            // public float tu, tv;
            public float nx, ny, nz;
            public float r, g, b;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DVector
        {
            public float x, y, z;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct DColorVertexType
        {
            internal Vector3 position;
            internal Vector4 color;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct DTempVertex
        {
            public float x, y, z;
            public float tu, tv;
            public float nx, ny, nz;
        }

        // Variables
        public int m_CellCount, m_renderCount, m_cellsDrawn, m_cellsCulled;
        private int m_TerrainWidth, m_TerrainHeight; //, m_ColourMapWidth, m_ColourMapHeight;
        private float m_TerrainScale = 12.0f;
        private string m_TerrainHeightManName; //  m_ColorMapName;

        // Properties
        private int VertexCount { get; set; }
        public int IndexCount { get; private set; }
        public DTerrainCell[] TerrainCells { get; set; }
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

            if (!BuildTerrainModel())
                return false;

            // Calculate the tangent and binormal for the terrain model.
            CalculateTerrainVectors();

            // Create and load the cells with the terrain data.
            if (!LoadTerrainCells(device))
                return false;
            
            // We can now release the height map since it is no longer needed in memory once the 3D terrain model has been built.
            ShutdownHeightMap();

            return true;
        }
        private bool LoadTerrainCells(SharpDX.Direct3D11.Device device)
        {
            // Set the height and width of each terrain cell to a fixed 33x33 vertex array.
            int cellHeight = 33;
            int cellWidth = 33;

            // Calculate the number of cells needed to store the terrain data.
            int cellRowCount = (m_TerrainWidth - 1) / (cellWidth - 1);
            m_CellCount = cellRowCount * cellRowCount;

            // Create the terrain cell array.
            TerrainCells = new DTerrainCell[m_CellCount];

            // Loop through and initialize all the terrain cells.
            for (int j = 0; j < cellRowCount; j++)
                for (int i = 0; i < cellRowCount; i++)
                {
                    int index = (cellRowCount * j) + i;
                    TerrainCells[index] = new DTerrainCell();
                    if (!TerrainCells[index].Initialize(device, TerrainModel, i, j, cellHeight, cellWidth, m_TerrainWidth))
                        return false;
                }
             
            return true;
        }
        private bool LoadRawHeightMap()
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

                        // Upper left.
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

                        // Upper right.
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

                        // Bottom left.
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

                        // Bottom left.
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

                        // Upper right.
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

                        // Bottom right.
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
            // m_ColorMapName = setupLines[4].TrimStart("Color Map Filename: ".ToCharArray());

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
        public void ShutDown()
        {
            // Release the terrain cells.
            ShutdownTerrainCells();
            // Release the height map.
            ShutdownHeightMap();
        }
        private void ShutdownHeightMap()
        {
            // Release the height map array.
            TerrainModel = null;
            HeightMap?.Clear();
            HeightMap = null;
        }
        private void ShutdownTerrainCells()
        {
            // Release the terrain cell array.
            for (int i = 0; i < m_CellCount; i++)
                TerrainCells[i].ShutDown();

            TerrainCells = null;
        }
        public bool RenderCell(DeviceContext deviceContext, int cellID, DFrustum frustum)
        {
            // Check if the cell is visible.  If it is not visible then just return and don't render it.
            if (!frustum.CheckRectangle2(TerrainCells[cellID].m_maxWidth, TerrainCells[cellID].m_maxHeight, TerrainCells[cellID].m_maxDepth, TerrainCells[cellID].m_minWidth, TerrainCells[cellID].m_minHeight, TerrainCells[cellID].m_minDepth))
            {
                // Increment the number of cells that were culled.
                m_cellsCulled++;
                return false;
            }

            // If it is visible then render it.
            TerrainCells[cellID].Render(deviceContext);

            // Add the polygons in the cell to the render count.
            m_renderCount += (TerrainCells[cellID].VertexCount / 3);

            // Increment the number of cells that were actually drawn.
            m_cellsDrawn++;

            return true;
        }
        public void RenderCellLines(DeviceContext deviceContext, int cellID)
        {
            TerrainCells[cellID].RenderLineBuffers(deviceContext);
        }
        public int GetCellIndexCount(int childIndex)
        {
            return TerrainCells[childIndex].IndexCount;
        }
        public int GetCellLinesIndexCount(int childIndex)
        {
            return TerrainCells[childIndex].LineIndexCount;
        }
        public void Frame()
        {
            m_renderCount = 0;
            m_cellsDrawn = 0;
            m_cellsCulled = 0;
        }
        public bool GetHeightAtPosition(float inputX, float inputZ, out float height)
        {
            // the default height to return when off the grid.
            height = 99.0f;
            Vector3 vertex1, vertex2, vertex3;

            // Loop through all of the terrain cells to find out which one the inputX and inputZ would be inside.
            int cellId = -1;
            for (int i = 0; i < m_CellCount; i++)
                if (inputX < TerrainCells[i].m_maxWidth && inputX > TerrainCells[i].m_minWidth && inputZ < TerrainCells[i].m_maxDepth && inputZ > TerrainCells[i].m_minDepth)
                {
                    cellId = i;
                    i = m_CellCount;
                }

            // If we didn't find a cell then the input position is off the terrain grid.
            if (cellId == -1)
                return false;

            // If this is the right cell then check all the triangles in this cell to see what the height of the triangle at this position is.\
            int index;
            for (int i = 0; i < (TerrainCells[cellId].VertexCount / 3); i++)
            {
                index = i * 3;

                vertex1 = new Vector3()
                {
                    X = TerrainCells[cellId].VertexList[index].x,
                    Y = TerrainCells[cellId].VertexList[index].y,
                    Z = TerrainCells[cellId].VertexList[index].z
                };
                index++;
                vertex2 = new Vector3()
                {
                    X = TerrainCells[cellId].VertexList[index].x,
                    Y = TerrainCells[cellId].VertexList[index].y,
                    Z = TerrainCells[cellId].VertexList[index].z
                };
                index++;
                vertex3 = new Vector3()
                {
                    X = TerrainCells[cellId].VertexList[index].x,
                    Y = TerrainCells[cellId].VertexList[index].y,
                    Z = TerrainCells[cellId].VertexList[index].z
                };

                // Check to see if this is the polygon we are looking for.
                if (CheckHeightOfTriangle(inputX, inputZ, out height, vertex1, vertex2, vertex3))
                    return true;
            }
           
            return false;
        }
        private bool CheckHeightOfTriangle(float x, float z, out float height, Vector3 v0, Vector3 v1, Vector3 v2)
        {
            height = 0.0f;

            // Starting position of the ray that is being cast.
            Vector3 startVector = new Vector3(x, 0.0f, z);

            // The direction the ray is being cast going striahgt down.
            Vector3 directionVector = new Vector3(0.0f, -1.0f, 0.0f);

            // Calculate the two edges from the three points given.
            Vector3 edge1 = new Vector3(v1.X - v0.X, v1.Y - v0.Y, v1.Z - v0.Z);
            Vector3 edge2 = new Vector3(v2.X - v0.X, v2.Y - v0.Y, v2.Z - v0.Z);

            // Calculate the normal of the triangle from the two edges.
            Vector3 normal = new Vector3();
            normal.X = (edge1.Y * edge2.Z) - (edge1.Z * edge2.Y);
            normal.Y = (edge1.Z * edge2.X) - (edge1.X * edge2.Z);
            normal.Z = (edge1.X * edge2.Y) - (edge1.Y * edge2.X);

            float magnitude = (float)Math.Sqrt((normal.X * normal.X) + (normal.Y * normal.Y) + (normal.Z * normal.Z));
            normal.X = normal.X / magnitude;
            normal.Y = normal.Y / magnitude;
            normal.Z = normal.Z / magnitude;

            // Find the distance from the origin to the plane.
            float D = ((-normal.X * v0.X) + (-normal.Y * v0.Y) + (-normal.Z * v0.Z));

            // Get the denominator of the equation.
            float denominator = ((normal.X * directionVector.X) + (normal.Y * directionVector.Y) + (normal.Z * directionVector.Z));

            // Make sure the result doesn't get too close to zero to prevent divide by zero.
            if (Math.Abs(denominator) < 0.0001f)
                return false;

            // Get the numerator of the equation.
            float numerator = -1.0f * (((normal.X * startVector.X) + (normal.Y * startVector.Y) + (normal.Z * startVector.Z)) + D);

            // Calculate where we intersect the triangle.
            float t = numerator / denominator;

            // Find the intersection vector.
            Vector3 Q = new Vector3();
            Q.X = startVector.X + (directionVector.X * t);
            Q.Y = startVector.Y + (directionVector.Y * t);
            Q.Z = startVector.Z + (directionVector.Z * t);

            // Find the three edges of the triangle.
            Vector3 e1 = new Vector3();
            e1.X = v1.X - v0.X;
            e1.Y = v1.Y - v0.Y;
            e1.Z = v1.Z - v0.Z;
            Vector3 e2 = new Vector3();
            e2.X = v2.X - v1.X;
            e2.Y = v2.Y - v1.Y;
            e2.Z = v2.Z - v1.Z;
            Vector3 e3 = new Vector3();
            e3.X = v0.X - v2.X;
            e3.Y = v0.Y - v2.Y;
            e3.Z = v0.Z - v2.Z;

            // Calculate the normal for the first edge.
            Vector3 edgeNormal = new Vector3();
            edgeNormal.X = (e1.Y * normal.Z) - (e1.Z * normal.Y);
            edgeNormal.Y = (e1.Z * normal.X) - (e1.X * normal.Z);
            edgeNormal.Z = (e1.X * normal.Y) - (e1.Y * normal.X);

            // Calculate the determinant to see if it is on the inside, outside, or directly on the edge.
            Vector3 temp = new Vector3();
            temp.X = Q.X - v0.X;
            temp.Y = Q.Y - v0.Y;
            temp.Z = Q.Z - v0.Z;
            float determinant = ((edgeNormal.X * temp.X) + (edgeNormal.Y * temp.Y) + (edgeNormal.Z * temp.Z));

            // Check if it is outside.
            if (determinant > 0.001f)
                return false;

            // Calculate the normal for the second edge.
            edgeNormal.X = (e2.Y * normal.Z) - (e2.Z * normal.Y);
            edgeNormal.Y = (e2.Z * normal.X) - (e2.X * normal.Z);
            edgeNormal.Z = (e2.X * normal.Y) - (e2.Y * normal.X);

            // Calculate the determinant to see if it is on the inside, outside, or directly on the edge.
            temp.X = Q.X - v1.X;
            temp.Y = Q.Y - v1.Y;
            temp.Z = Q.Z - v1.Z;
            determinant = ((edgeNormal.X * temp.X) + (edgeNormal.Y * temp.Y) + (edgeNormal.Z * temp.Z));

            // Check if it is outside.
            if (determinant > 0.001f)
                return false;

            // Calculate the normal for the third edge.
            edgeNormal.X = (e3.Y * normal.Z) - (e3.Z * normal.Y);
            edgeNormal.Y = (e3.Z * normal.X) - (e3.X * normal.Z);
            edgeNormal.Z = (e3.X * normal.Y) - (e3.Y * normal.X);

            // Calculate the determinant to see if it is on the inside, outside, or directly on the edge.
            temp.X = Q.X - v2.X;
            temp.Y = Q.Y - v2.Y;
            temp.Z = Q.Z - v2.Z;
            determinant = ((edgeNormal.X * temp.X) + (edgeNormal.Y * temp.Y) + (edgeNormal.Z * temp.Z));

            // Check if it is outside.
            if (determinant > 0.001f)
                return false;

            // Now we have our height.
            height = Q.Y;

            return true;
        }
    }
}