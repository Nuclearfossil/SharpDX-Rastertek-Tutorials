using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace DSharpDXRastertek.Series2.TutTerr12.Graphics.Models
{
    public class DTerrainCell
    {
        // variables
        public float m_maxWidth, m_maxHeight, m_maxDepth, m_minWidth, m_minHeight, m_minDepth;
        public float m_positionX, m_positionY, m_positionZ;

        // Properties
        private SharpDX.Direct3D11.Buffer VertexBuffer { get; set; }
        private SharpDX.Direct3D11.Buffer IndexBuffer { get; set; }
        private SharpDX.Direct3D11.Buffer LineVertexBuffer { get; set; }
        private SharpDX.Direct3D11.Buffer LineIndexBuffer { get; set; }
        public int VertexCount { get; set; }
        public int IndexCount { get; private set; }
        public int LineIndexCount { get; set; }
        public DTerrain.DVector[] VertexList { get; set; }

        public DTerrainCell(){ }

        internal bool Initialize(SharpDX.Direct3D11.Device device, DTerrain.DHeightMapType[] terrainModel, int nodeIndexX, int nodeIndexY, int cellHeight, int cellWidth, int terrainWidth)
        {
            // Load the rendering buffers with the terrain data for this cell index.
            if (!InitializeBuffers(device, nodeIndexX, nodeIndexY, cellHeight, cellWidth, terrainWidth, terrainModel))
                return false;

            // Calculuate the dimensions of this cell.
            CalculateCellDimensions();

            // Build the debug line buffers to produce the bounding box around this cell.
            if (!BuildLineBuffers(device))
                return false;

            return true;
        }
        private bool InitializeBuffers(SharpDX.Direct3D11.Device device, int nodeIndexX, int nodeIndexY, int cellHeight, int cellWidth, int terrainWidth, DTerrain.DHeightMapType[] terrainModel)
        {
            // Calculate the number of vertices in this terrain cell.
            VertexCount = (cellHeight - 1) * (cellWidth - 1) * 6;

            // Set the index count to the same as the vertex count.
            IndexCount = VertexCount;

            // Create the vertex array.
            DTerrain.DVertexType[] vertices = new DTerrain.DVertexType[VertexCount];
            VertexList = new DTerrain.DVector[VertexCount];

            // Create the index array.
            int[] indices = new int[IndexCount];

            // Setup the indexes into the terrain model data and the local vertex/index array.
            int modelIndex = ((nodeIndexX * (cellWidth - 1)) + (nodeIndexY * (cellHeight - 1) * (terrainWidth - 1))) * 6;
            int index = 0;

            // Load the vertex and index arrays with the terrain data.
            // for (var i = 0; i < VertexCount; i++)
            for (int j = 0; j < (cellHeight - 1); j++)
            {
                for (int i = 0; i < ((cellWidth - 1) * 6); i++)
                {
                    vertices[index] = new DTerrain.DVertexType()
                    {
                        position = new Vector3(terrainModel[modelIndex].x, terrainModel[modelIndex].y, terrainModel[modelIndex].z),
                        texture = new Vector2(terrainModel[modelIndex].tu, terrainModel[modelIndex].tv),
                        normal = new Vector3(terrainModel[modelIndex].nx, terrainModel[modelIndex].ny, terrainModel[modelIndex].nz),
                        tangent = new Vector3(terrainModel[modelIndex].tx, terrainModel[modelIndex].ty, terrainModel[modelIndex].tz),
                        binormal = new Vector3(terrainModel[modelIndex].bx, terrainModel[modelIndex].by, terrainModel[modelIndex].bz),
                        color = new Vector3(terrainModel[modelIndex].r, terrainModel[modelIndex].g, terrainModel[modelIndex].b)
                    };
                    VertexList[index] = new DTerrain.DVector()
                    {
                        x = terrainModel[modelIndex].x,
                        y = terrainModel[modelIndex].y,
                        z = terrainModel[modelIndex].z
                    };
                    indices[index] = index;
                    modelIndex++;
                    index++;
                }
                modelIndex += (terrainWidth * 6) - (cellWidth * 6);
            }

            // Now create the vertex buffer.
            VertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);
   
            // Create the index buffer.
            IndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indices);

            // Release the arrays now that the buffers have been created and loaded.
            vertices = null;
            indices = null;

            return true;
        }
        private void CalculateCellDimensions()
        {
            // Initialize the dimensions of the node.
            m_maxWidth = -1000000.0f;
            m_maxHeight = -1000000.0f;
            m_maxDepth = -1000000.0f;
            m_minWidth = 1000000.0f;
            m_minHeight = 1000000.0f;
            m_minDepth = 1000000.0f;

            for (int i = 0; i < VertexCount; i++)
            {
                float width = VertexList[i].x;
                float height = VertexList[i].y;
                float depth = VertexList[i].z;

                // Check if the width exceeds the minimum or maximum.
                if (width > m_maxWidth)
                    m_maxWidth = width;     
                if (width < m_minWidth)
                    m_minWidth = width;
                
                // Check if the height exceeds the minimum or maximum.
                if (height > m_maxHeight)
                    m_maxHeight = height;     
                if (height < m_minHeight)
                    m_minHeight = height;
               
                // Check if the depth exceeds the minimum or maximum.
                if (depth > m_maxDepth)
                    m_maxDepth = depth;   
                if (depth < m_minDepth)
                    m_minDepth = depth;
            }

            // Calculate the center position of this cell.
            m_positionX = (m_maxWidth - m_minWidth) + m_minWidth;
            m_positionY = (m_maxHeight - m_minHeight) + m_minHeight;
            m_positionZ = (m_maxDepth - m_minDepth) + m_minDepth;
        }
        private bool BuildLineBuffers(SharpDX.Direct3D11.Device device)
        {
            // Set the color of the lines to orange.
            Vector4 lineColor = new Vector4(1.0f, 0.5f, 0.0f, 1.0f);

            // Set the number of vertices in the vertex array.
            LineIndexCount = 24;
            // Set the number of indices in the index array.
            int indexCount = LineIndexCount;

            // Create the vertex array.
            DTerrain.DColorVertexType[] vertices = new DTerrain.DColorVertexType[LineIndexCount];

            // Create the index array.
            int[] indicesLies = new int[indexCount];

            // Load the vertex and index array with data.
            int index = 0;

            // 8 Horizontal lines.
            vertices[index].position = new Vector3(m_minWidth, m_minHeight, m_minDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_maxWidth, m_minHeight, m_minDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_minWidth, m_minHeight, m_maxDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_maxWidth, m_minHeight, m_maxDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_minWidth, m_minHeight, m_minDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_minWidth, m_minHeight, m_maxDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_maxWidth, m_minHeight, m_minDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_maxWidth, m_minHeight, m_maxDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_minWidth, m_maxHeight, m_minDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_maxWidth, m_maxHeight, m_minDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_minWidth, m_maxHeight, m_maxDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_maxWidth, m_maxHeight, m_maxDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_minWidth, m_maxHeight, m_minDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_minWidth, m_maxHeight, m_maxDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_maxWidth, m_maxHeight, m_minDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_maxWidth, m_maxHeight, m_maxDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            // 4 Verticle lines.
            vertices[index].position = new Vector3(m_maxWidth, m_maxHeight, m_maxDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_maxWidth, m_minHeight, m_maxDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_minWidth, m_maxHeight, m_maxDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_minWidth, m_minHeight, m_maxDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_maxWidth, m_maxHeight, m_minDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_maxWidth, m_minHeight, m_minDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_minWidth, m_maxHeight, m_minDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;
            index++;

            vertices[index].position = new Vector3(m_minWidth, m_minHeight, m_minDepth);
            vertices[index].color = lineColor;
            indicesLies[index] = index;

            LineVertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);

            // Create the index buffer.
            LineIndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indicesLies);

            // Release the arrays now that the buffers have been created and loaded.
            vertices = null;
            indicesLies = null;

            return true;
        }
        internal void Render(DeviceContext deviceContext)
        {
            // Put the vertex and index buffers on the graphics pipeline to prepare them for drawing.
            RenderBuffers(deviceContext);
        }

        private void RenderBuffers(DeviceContext deviceContext)
        {
            // Set the vertex buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<DTerrain.DVertexType>(), 0));
            // Set the index buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }

        public void RenderLineBuffers(DeviceContext deviceContext)
        {
            // Set the vertex buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(LineVertexBuffer, Utilities.SizeOf<DTerrain.DColorVertexType>(), 0));
            // Set the index buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetIndexBuffer(LineIndexBuffer, Format.R32_UInt, 0);
            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
        }

        internal void ShutDown()
        {
            // Release the line rendering buffers.
            ShutdownLineBuffers();

            // Release the cell rendering buffers.
            ShutdownBuffers();
        }
        private void ShutdownBuffers()
        {
            // Release the public vertex list.
            VertexList = null;
            // Return the index buffer.
            IndexBuffer?.Dispose();
            IndexBuffer = null;
            // Release the vertex buffer.
            VertexBuffer?.Dispose();
            VertexBuffer = null;
        }
        private void ShutdownLineBuffers()
        {
            // Release the index buffer.
            LineIndexBuffer?.Dispose();
            LineIndexBuffer = null;
            // Release the vertex buffer.
            LineVertexBuffer?.Dispose();
            LineVertexBuffer = null;
        }
    }
}