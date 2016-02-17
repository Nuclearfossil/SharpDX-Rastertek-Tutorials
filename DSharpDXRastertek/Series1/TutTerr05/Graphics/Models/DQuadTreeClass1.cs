using DSharpDXRastertek.TutTerr05.Graphics.Data;
using DSharpDXRastertek.TutTerr05.Graphics.Shaders;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Runtime.InteropServices;

namespace DSharpDXRastertek.TutTerr05.Graphics.Models
{
    public class DQuadTree                  // 454 lines
    {
        // Structs
        [StructLayout(LayoutKind.Sequential)]
        public struct DNodeType
        {
            public float positionX, positionZ, width;
            public int TriangleCount;
            public SharpDX.Direct3D11.Buffer VertexBuffer, IndexBuffer;
            public DNodeType[] Nodes;
        }

        // Variables
        private readonly int MAX_TRIANGLES = 10000;

        // Properties
        public int TriangleCount { get; set; }
        public int DrawCount { get; set; }
        public DTerrain.DVertexType[] VertexList { get; set; }
        public DNodeType ParentNode { get; set; }

        // Methods
        public bool Initialize(DTerrain terrain, SharpDX.Direct3D11.Device device)
        {
            // Get the number of vertices in the terrain vertex array.
            int vertexCount = terrain.VertexCount;

            // Store the total triangle count for the vertex list.
            TriangleCount = vertexCount / 3;

            // Create a vertex array to hold all of the terrain vertices.
            VertexList = new DTerrain.DVertexType[vertexCount];

            // Copy the terrain vertices into the vertex list.
            VertexList = terrain.Verticies;

            // Calculate the center x,z and the width of the mesh.
            float centerX, centerZ, width;
            CalculateMeshDimensions(vertexCount, out centerX, out centerZ, out width);

            // Create the parent node for the quad tree.
            ParentNode = new DNodeType();
            DNodeType createdNode;
            
            // Recursively build the quad tree based on the vertex list data and mesh dimensions.
            CreateTreeNode2(device, ParentNode, centerX, centerZ, width, out createdNode);
            ParentNode = createdNode;

            // Release the vertex list since the quad tree now has the vertices in each node.
            VertexList = null;

            return true;
        }
        private void CreateTreeNode2(SharpDX.Direct3D11.Device device, DNodeType node, float positionX, float positionZ, double width, out DNodeType createdNode)
        {
            // Store the node position and size.
            node.positionX = positionX;
            node.positionZ = positionZ;
            node.width = (float)width;

            // Initialize the triangle count to zero for the node.
            node.TriangleCount = 0;

            // Initialize the vertex and index buffer to null.
            node.VertexBuffer = null;
            node.IndexBuffer = null;

            // Count the number of triangles that are inside this node.
            int numTriangles = CountTriangles(positionX, positionZ, width);

            // Case 1: If there are no triangles in this node then return as it is empty and requires no processing.
            if (numTriangles == 0)
            { 
                createdNode = node;
                return;
            }

            // Initialize the children nodes of this node to null.
            node.Nodes = new DNodeType[4];

            // Case 2: If there are too many triangles in this node then split it into four equal sized smaller tree nodes.
            if (numTriangles > MAX_TRIANGLES)
            {
                for (int i = 0; i < 4; i++)
                {  
                    // Calculate the position offsets for the new child node.
                    float offsetX = (((i % 2) < 1) ? -1.0f : 1.0f) * (float)(width / 4.0f);
                    float offsetZ = (((i % 4) < 2) ? -1.0f : 1.0f) * (float)(width / 4.0f);
                   
                    // See if there are any triangles in the new node.
                    int count = CountTriangles((positionX + offsetX), (positionZ + offsetZ), (width / 2.0D));

                    if (count > 0)
                    {
                        // If there are triangles inside where this new node would be then create the child node.
                        node.Nodes[i] = new DNodeType();
                        DNodeType aNewNode;
                        
                        // Extend the tree starting from this new child node now.
                        CreateTreeNode2(device, node.Nodes[i], (positionX + offsetX), (positionZ + offsetZ), (width / 2.0D), out aNewNode);

                        node.Nodes[i] = aNewNode;
                        createdNode = aNewNode;
                    }
                }

                createdNode = node;
            }

            // Case 3: If this node is not empty and the triangle count for it is less than the max then this node is at the bottom of the tree so create the list of triangles to store in it.
            node.TriangleCount = numTriangles;

            // Calculate the number of vertices.
            int vertexCount = numTriangles * 3;

            // Create the vertex array.
            DTerrain.DVertexType[] vertices = new DTerrain.DVertexType[vertexCount];

            // Create the index array
            int[] indices = new int[vertexCount];

            // Initialize the index for this new vertex and index array.
            int index = 0;

            // Go through all the triangles in the vertex list.
            for (int i = 0; i < TriangleCount; i++)
            {
                // If the triangle is inside this node then add it to the vertex array.
                if (IsTriangleContained(i, positionX, positionZ, width))
                {
                    // Calculate the index into the terrain vertex list.
                    int vertexIndex = i * 3;

                    // Get the three vertices of this triangle from the vertex list.
                    vertices[index] = VertexList[vertexIndex];
                    indices[index] = index++;

                    vertexIndex++;
                    vertices[index] = VertexList[vertexIndex];
                    indices[index] = index++;

                    vertexIndex++;
                    vertices[index] = VertexList[vertexIndex];
                    indices[index] = index++;
                }
            }

            // Set up the description of the vertex buffer. no Need its static
            // Create the vertex buffer.
            node.VertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);

            // Create the index buffer.
            node.IndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indices);

            // Release the vertex and index arrays now that the data is stored in the buffers in the node.
            vertices = null;
            indices = null;

            createdNode = node;

            return;
        }
        private DNodeType CreateTreeNode(SharpDX.Direct3D11.Device device, float positionX, float positionZ, float width)
        {
            DNodeType Node = new DNodeType();

            // Store the node position and size.
            Node.positionX = positionX;
            Node.positionZ = positionZ;
            Node.width = width;

            // Initialize the triangle count to zero for the node.
            Node.TriangleCount = 0;

            // Initialize the vertex and index buffer to null.
            Node.VertexBuffer = null;
            Node.IndexBuffer = null;

            // Count the number of triangles that are inside this node.
            int numTriangles = CountTriangles(positionX, positionZ, width);

            // Case 1: If there are no triangles in this node then return as it is empty and requires no processing.
            if (numTriangles == 0)
                return Node;

            // Initialize the children nodes of this node to null.
            Node.Nodes = new DNodeType[4];


            // Case 2: If there are too many triangles in this node then split it into four equal sized smaller tree nodes.
            if (numTriangles > MAX_TRIANGLES)
            {
                for (int i = 0; i < 4; i++)
                {
                    // Calculate the position offsets for the new child node.
                    float offsetX = (((i % 2) < 1) ? -1.0f : 1.0f) * (width / 4.0f);
                    float offsetZ = (((i % 4) < 2) ? -1.0f : 1.0f) * (width / 4.0f);

                    // See if there are any triangles in the new node.
                    int count = CountTriangles((positionX + offsetX), (positionZ + offsetZ), (width / 2.0f));
                    
                    if (count > 0)
                    {
                        // If there are triangles inside where this new node would be then create the child node.
                        Node.Nodes[i] = CreateTreeNode(device, (positionX + offsetX), (positionZ + offsetZ), width / 2);
                    }
                }

               return Node;
            }

            // Case 3: If this node is not empty and the triangle count for it is less than the max then 
            // This node is at the bottom of the tree so create the list of triangles to store in it.
            if (numTriangles > MAX_TRIANGLES)
                return Node;

            Node.TriangleCount = numTriangles;

            // Calculate the number of vertices.
            int vertexCount = numTriangles * 3;

            // Create the vertex array.
            DTerrain.DVertexType[] vertices = new DTerrain.DVertexType[vertexCount];

            // Create the index array
            int[] indices = new int[vertexCount];

            // Initialize the index for this new vertex and index array.
            int index = 0;

            // Go through all the triangles in the vertex list.
            for (int i = 0; i < TriangleCount; i++)
            {
                // If the triangle is inside this node then add it to the vertex array.
                if (IsTriangleContained(i, positionX, positionZ, width))
                {
                    // Calculate the index into the terrain vertex list.
                    int vertexIndex = i * 3;

                    // Get the three vertices of this triangle from the vertex list.
                    vertices[index] = VertexList[vertexIndex];
                    indices[index] = index++;

                    vertexIndex++;
                    vertices[index] = VertexList[vertexIndex];
                    indices[index] = index++;

                    vertexIndex++;
                    vertices[index] = VertexList[vertexIndex];
                    indices[index] = index++;
                }
            }

            // Set up the description of the vertex buffer. no Need its static
            // Create the vertex buffer.
            Node.VertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);

            // Create the index buffer.
            Node.IndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indices);

            // Release the vertex and index arrays now that the data is stored in the buffers in the node.
            vertices = null;
            indices = null;

            return Node;
        }
        private int CountTriangles(float positionX, float positionZ, double width)
        {
            // Initialize the count to zero.
            int count = 0;

            // Go through all the triangles in the entire mesh and check which ones should be inside this node.
            for (int i = 0; i < TriangleCount; i++)
            {
                // If the triangle is inside the node then increment the count by one.
                if (IsTriangleContained(i, positionX, positionZ, width))
                    count++;
            }

            return count;
        }
        private bool IsTriangleContained(int index, float positionX, float positionZ, double width)
        {
            // Calculate the radius of this node
            double radius = width / 2.0f;

            // Get the index into the vertex list.
            int vertexIndex = index * 3;

            // Get the three vertices of this triangle from the vertex list.
            float x1 = VertexList[vertexIndex].position.X;
            float z1 = VertexList[vertexIndex].position.Z;
            vertexIndex++;
            float x2 = VertexList[vertexIndex].position.X;
            float z2 = VertexList[vertexIndex].position.Z;
            vertexIndex++;
            float x3 = VertexList[vertexIndex].position.X;
            float z3 = VertexList[vertexIndex].position.Z;

            // Check to see if the minimum of the x coordinates of the triangle is inside the node.
            float minimumX = Math.Min(x1, Math.Min(x2, x3));
            if (minimumX > (positionX + radius))
                return false;

            // Check to see if the maximum of the x coordinates of the triangle is inside the node.
            float maximumX = Math.Max(x1, Math.Max(x2, x3));
            if (maximumX < (positionX - radius))
                return false;

            // Check to see if the minimum of the z coordinates of the triangle is inside the node.
            float minimumZ = Math.Min(z1, Math.Min(z2, z3));
            if (minimumZ > (positionZ + radius))
                return false;

            // Check to see if the maximum of the z coordinates of the triangle is inside the node.
            float maximumZ = Math.Max(z1, Math.Max(z2, z3));
            if (maximumZ < (positionZ - radius))
                return false;

            return true;
        }
        private void CalculateMeshDimensions(int vertexCount, out float centerX, out float centerZ, out float meshWidth)
        {
            // Initialize the center position of the mesh to zero.
            centerX = 0.0f;
            centerZ = 0.0f;

            // Sum all the vertices in the mesh.
            for (int i = 0; i < vertexCount; i++)
            {
                centerX += VertexList[i].position.X;
                centerZ += VertexList[i].position.Z;
            }

            // And then divide it by the number of vertices to find the mid-point of the mesh.
            centerX = centerX / (float)vertexCount;
            centerZ = centerZ / (float)vertexCount;

            // Initialize the maximum and minimum size of the mesh.
            float maxWidth = 0.0f;
            float maxDepth = 0.0f;
            float minWidth = Math.Abs(VertexList[0].position.X - centerX);
            float minDepth = Math.Abs(VertexList[0].position.Z - centerZ);

            // Go through all the vertices and find the maximum and minimum width and depth of the mesh.
            for (int i = 0; i < vertexCount; i++)
            {
                float width = Math.Abs(VertexList[i].position.X - centerX);
                float depth = Math.Abs(VertexList[i].position.Z - centerZ);

                if (width > maxWidth) 
                    maxWidth = width;
                if (depth > maxDepth) 
                    maxDepth = depth;
                if (width < minWidth) 
                    minWidth = width;
                if (depth < minDepth) 
                    minDepth = depth;
            }

            // Find the absolute maximum value between the min and max depth and width.
            float maxX = Math.Max(Math.Abs(minWidth), Math.Abs(maxWidth));
            float maxZ = Math.Max(Math.Abs(minDepth), Math.Abs(maxDepth));

            // Calculate the maximum diameter of the mesh.
            meshWidth = Math.Max(maxX, maxZ) * 2.0f;
        }
        public void Shutdown()
        {
            ReleaseNode(ParentNode);
        }
        private void ReleaseNode(DNodeType theNode)
        {
            // Watch to see if this actually closes the Node structs.
            if (theNode.VertexBuffer == null)
                return;

            // Recursively go down the tree and release the bottom nodes first
            for (int i = 0; i < 4; i++)
                if (theNode.Nodes[i].VertexBuffer != null)
                    ReleaseNode(theNode.Nodes[i]);

            // Release the vertex buffer for this node
            theNode.VertexBuffer?.Dispose();
            theNode.VertexBuffer = null;
            // Release the index buffer for this node
            theNode.IndexBuffer?.Dispose();
            theNode.IndexBuffer = null;
        }
        public void Render(DeviceContext deviceContext, DFrustum frustum, DTerrainShader terrainShader)
        {
            // Reset the number of the triangles that are drawn for this frame.
            DrawCount = 0;

            // Render each node that is visible at the parent node and moving down the tree.
            RenderNode(deviceContext, ParentNode, frustum, terrainShader);
        }
        private void RenderNode(DeviceContext deviceContext, DNodeType node, DFrustum frustum, DTerrainShader terrainShader)
        {
            // Check to see if the node can be viewed, height doesn't matter in a quad tree.
            // If it can't be seen then none of its children can either, so don't continue down the tree, this is where the speed is gained.
            if (!frustum.CheckCube(new SharpDX.Vector3(node.positionX, 0.0f, node.positionZ), (node.width / 2.0f)))  //   63.7506409
                return;

            // If it can be seen then check all four child nodes to see if they can also be seen.
            int count = 0;
            for (int i = 0; i < 4; i++)
            {   // parentNode.width > 0.0f && parentNode.Nodes[i].VertexBuffer != null
                if (node.Nodes[i].width > 0.0f) //node.Nodes != null/ parentNode.Nodes[i].width > 0.0f
                {
                    count++;
                    RenderNode(deviceContext, node.Nodes[i], frustum, terrainShader);
                }
            }

            // If there were any children nodes then there is no need to continue as parent nodes won't contain any triangles to render.
            if (count != 0)
                return;

            int vertexCount = node.TriangleCount * 3;

            // Otherwise if this node can be seen and has triangles in it then render these triangles.
            // Set vertex buffer stride and offset.
            // Set the vertex buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(node.VertexBuffer, Utilities.SizeOf<DTerrain.DVertexType>(), 0));

            // Set the index buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetIndexBuffer(node.IndexBuffer, Format.R32_UInt, 0);

            // Set the type of primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            // Determine the number of indices in this node.
            int indexCount = node.TriangleCount * 3;

            // Call the terrain shader to render the polygons in this node.
            terrainShader.RenderShader(deviceContext, indexCount);

            // Increase the count of the number of polygons that have been rendered during this frame.
            DrawCount += node.TriangleCount;
        }
    }
}