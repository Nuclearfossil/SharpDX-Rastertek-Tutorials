using DSharpDXRastertek.Tut49.Graphics.Data;
using DSharpDXRastertek.Tut49.System;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace DSharpDXRastertek.Tut49.Graphics.Models
{
    public class DTreeModel                 // 299 lines
    {
        // Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct DModelFormat
        {
            public float x, y, z;
            public float tu, tv;
            public float nx, ny, nz;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct DVertexModel
        {
            public Vector3 position;
            public Vector2 texture;
            public Vector3 normal;
        } 

        // Variables
        private Vector3 Position;

        // Properties
        private SharpDX.Direct3D11.Buffer TrunkVertexBuffer { get; set; }
        private SharpDX.Direct3D11.Buffer TrunkIndexBuffer { get; set; }
        private SharpDX.Direct3D11.Buffer LeafVertexBuffer { get; set; }
        private SharpDX.Direct3D11.Buffer LeafIndexBuffer { get; set; }
        public int LoadingCount { get; private set; }
        public int TrunkIndexCount { get; private set; }
        public int LeafIndexCount { get; private set; }
        public DTexture TrunkTexture { get; set; }
        public DTexture LeafTexture { get; set; }
        public DModelFormat[] ModelObject { get; private set; }

        // Constructor 
        public DTreeModel(){ }

        // Methods
        public bool Initialize(SharpDX.Direct3D11.Device device, string trunkModelFilename, string trunkTextureFilename, string leafModelFilename, string leafTextureFilename, float scale)
        {
            // Load in the tree trunk model data.
            if (!LoadModel(trunkModelFilename))
                return false;

            // Store the trunk index count;
            TrunkIndexCount = LoadingCount;

            // Initialize the vertex and index buffer that hold the geometry for the tree trunk.
            if (!InitializeTrunkBuffers(device, scale))
                return false;

            // Release the tree trunk model data since it is loaded into the buffers.
            ReleaseModel();

            // Load in the tree leaf model data.
            if (!LoadModel(leafModelFilename))
                return false;

            // Store the leaf index count;
            LeafIndexCount = LoadingCount;

            // Initialize the vertex and index buffer that hold the geometry for the tree leaves.
            if (!InitializeLeafBuffers(device, scale))
                return false;

            // Release the tree leaf model data.
            ReleaseModel();

            // Load the textures for the tree model.
            if (!LoadTextures(device, trunkTextureFilename, leafTextureFilename))
                return false;

            return true;
        }
        private bool LoadModel(string modelFormatFilename)
        {
            modelFormatFilename = DSystemConfiguration.ModelFilePath + @"Trees\" + modelFormatFilename;
            List<string> lines = null;

            try
            {
                // Open the model file.
                lines = File.ReadLines(modelFormatFilename).ToList();

                // Read in the vertex count.
                var vertexCountString = lines[0].Split(new char[] { ':' })[1].Trim();
                LoadingCount = int.Parse(vertexCountString);
                
                // Create the model using the vertex count that was read in.
                ModelObject = new DModelFormat[LoadingCount];

                // Read in the vertex data.
                for (var i = 4; i < lines.Count && i < 4 + LoadingCount; i++)
                {
                    var modelArray = lines[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    ModelObject[i - 4] = new DModelFormat()
                    {
                        x = float.Parse(modelArray[0]),
                        y = float.Parse(modelArray[1]),
                        z = float.Parse(modelArray[2]),
                        tu = float.Parse(modelArray[3]),
                        tv = float.Parse(modelArray[4]),
                        nx = float.Parse(modelArray[5]),
                        ny = float.Parse(modelArray[6]),
                        nz = float.Parse(modelArray[7])
                    };
                }

                // Close the model file.
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private bool LoadTextures(SharpDX.Direct3D11.Device device, string trunkFilename, string leafFilename)
        {
            trunkFilename = DSystemConfiguration.DataFilePath + @"Trees\" +  trunkFilename;
            leafFilename = DSystemConfiguration.DataFilePath + @"Trees\" + leafFilename;

            // Create the trunk texture object.
            TrunkTexture = new DTexture();

            // Initialize the texture object.
            if (!TrunkTexture.Initialize(device, trunkFilename))
                return false;

            // Create the leaf texture object.
            LeafTexture = new DTexture();

            // Initialize the leaf texture object.
            if (!LeafTexture.Initialize(device, leafFilename))
                return false;

            return true;
        }
        public void Shutdown()
        {
            // Release the model texture.
            ReleaseTextures();

            // Release the vertex and index buffers.
            ShutdownBuffers();

            // Release the model data.
            ReleaseModel();
        }
        private void ReleaseModel()
        {
            ModelObject = null;
        }
        private void ReleaseTextures()
        {
            // Release the textures object.
            LeafTexture?.ShutDown();
            LeafTexture = null;
            TrunkTexture?.ShutDown();
            TrunkTexture = null;
        }
        public void RenderTrunk(SharpDX.Direct3D11.DeviceContext deviceContext)
        {
            // Put the vertex and index buffers on the graphics pipeline to prepare them for drawing.
            RenderTrunkBuffers(deviceContext);
        }
        public void RenderLeaves(SharpDX.Direct3D11.DeviceContext deviceContext)
        {
            // Put the vertex and index buffers on the graphics pipeline to prepare them for drawing.
            RenderLeafBuffers(deviceContext);
        }
        private bool InitializeLeafBuffers(SharpDX.Direct3D11.Device device, float scale)
        {
            try
            {
                // Create the vertex array.
                var vertices = new DVertexModel[LeafIndexCount];
                // Create the index array.
                var indices = new int[TrunkIndexCount];

                for (var i = 0; i < LeafIndexCount; i++)
                {
                    vertices[i] = new DVertexModel()
                    {
                        position = new Vector3(ModelObject[i].x * scale, ModelObject[i].y * scale, ModelObject[i].z * scale),
                        texture = new Vector2(ModelObject[i].tu, ModelObject[i].tv),
                        normal = new Vector3(ModelObject[i].nx, ModelObject[i].ny, ModelObject[i].nz)
                    };

                    indices[i] = i;
                }

                // Create the vertex buffer.
                LeafVertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);

                // Create the index buffer.
                LeafIndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indices);

                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool InitializeTrunkBuffers(SharpDX.Direct3D11.Device device, float scale)
        {
            try
            {
                // Create the vertex array.
                var vertices = new DVertexModel[TrunkIndexCount];
                // Create the index array.
                var indices = new int[TrunkIndexCount];

                for (var i = 0; i < TrunkIndexCount; i++)
                {
                    vertices[i] = new DVertexModel()
                    {
                        position = new Vector3(ModelObject[i].x * scale, ModelObject[i].y * scale, ModelObject[i].z * scale),
                        texture = new Vector2(ModelObject[i].tu, ModelObject[i].tv),
                        normal = new Vector3(ModelObject[i].nx, ModelObject[i].ny, ModelObject[i].nz)
                    };

                    indices[i] = i;
                }

                // Create the vertex buffer.
                TrunkVertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);

                // Create the index buffer.
                TrunkIndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indices);

                return true;
            }
            catch
            {
                return false;
            }
        }
        private void ShutdownBuffers()
        {
            // Return the index buffer.
            TrunkIndexBuffer?.Dispose();
            TrunkIndexBuffer = null;
            LeafIndexBuffer?.Dispose();
            LeafIndexBuffer = null;
            // Release the vertex buffer.
            TrunkVertexBuffer?.Dispose();
            TrunkVertexBuffer = null;
            LeafVertexBuffer?.Dispose();
            LeafVertexBuffer = null;
        }
        private void RenderTrunkBuffers(SharpDX.Direct3D11.DeviceContext deviceContext)
        {
            // Set the vertex buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(TrunkVertexBuffer, Utilities.SizeOf<DVertexModel>(), 0));
            
            // Set the index buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetIndexBuffer(TrunkIndexBuffer, Format.R32_UInt, 0);
            
            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }
        private void RenderLeafBuffers(SharpDX.Direct3D11.DeviceContext deviceContext)
        {
            // Set the vertex buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(LeafVertexBuffer, Utilities.SizeOf<DVertexModel>(), 0));

            // Set the index buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetIndexBuffer(LeafIndexBuffer, Format.R32_UInt, 0);

            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }
        public void SetPosition(float x, float y, float z)
        {
            Position.X = x;
            Position.Y = y;
            Position.Z = z;
        }
        public Vector3 GetPosition()
        {
            return Position;
        }
    }
}