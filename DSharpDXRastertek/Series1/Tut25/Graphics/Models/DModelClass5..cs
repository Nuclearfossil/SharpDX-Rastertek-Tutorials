using DSharpDXRastertek.Tut25.Graphics.Data;
using DSharpDXRastertek.Tut25.System;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace DSharpDXRastertek.Tut25.Graphics.Models
{
    public class DModel                 // 191 lines
    {
        // Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct DVertex
        {
            public Vector3 position;
            public Vector2 texture;
            public Vector3 normal;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct DModelFormat
        {
            public float x, y, z;
            public float tu, tv;
            public float nx, ny, nz;
        }

        // Properties
        private SharpDX.Direct3D11.Buffer VertexBuffer { get; set; }
        private SharpDX.Direct3D11.Buffer IndexBuffer { get; set; }
        private int VertexCount { get; set; }
        public int IndexCount { get; private set; }
        public DTextureArray TextureCollection { get; private set; }
        public DModelFormat[] ModelObject { get; private set; }

        // Constructor
        public DModel() { }

        // Methods
        public bool Initialize(SharpDX.Direct3D11.Device device, string modelFormatFilename, string[] textureFileNames)
        {
            // Load in the model data.
            if (!LoadModel(modelFormatFilename))
                return false;

            // Initialize the vertex and index buffer.
            if (!InitializeBuffers(device))
                return false;

            // Load the texture for this model.
            if (!LoadTextures(device, textureFileNames))
                return false;

            return true;
        }
        private bool LoadModel(string modelFormatFilename)
        {
            modelFormatFilename = DSystemConfiguration.ModelFilePath + modelFormatFilename;
            List<string> lines = null;

            try
            {
                lines = File.ReadLines(modelFormatFilename).ToList();

                var vertexCountString = lines[0].Split(new char[] { ':' })[1].Trim();
                VertexCount = int.Parse(vertexCountString);
                IndexCount = VertexCount;
                ModelObject = new DModelFormat[VertexCount];

                for (var i = 4; i < lines.Count && i < 4 + VertexCount; i++)
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

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private bool LoadTextures(SharpDX.Direct3D11.Device device, string[] textureFileNames)
        {
           for (var i = 0; i < textureFileNames.Length; i++)
				textureFileNames[i] = DSystemConfiguration.DataFilePath + textureFileNames[i];

            // Create the texture object.
            TextureCollection = new DTextureArray();

            // Initialize the texture object.
            TextureCollection.Initialize(device, textureFileNames);

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
            TextureCollection?.Shutdown();
            TextureCollection = null;
        }
        public void Render(SharpDX.Direct3D11.DeviceContext deviceContext)
        {
            // Put the vertex and index buffers on the graphics pipeline to prepare for drawings.
            RenderBuffers(deviceContext);
        }
        private bool InitializeBuffers(SharpDX.Direct3D11.Device device)
        {
            try
            {
                // Create the vertex array.
                var vertices = new DVertex[VertexCount];
                // Create the index array.
                var indices = new int[IndexCount];

                for (var i = 0; i < VertexCount; i++)
                {
                    vertices[i] = new DVertex()
                    {
                        position = new Vector3(ModelObject[i].x, ModelObject[i].y, ModelObject[i].z),
                        texture = new Vector2(ModelObject[i].tu, ModelObject[i].tv),
                        normal = new Vector3(ModelObject[i].nx, ModelObject[i].ny, ModelObject[i].nz)
                    };

                    indices[i] = i;
                }

                // Create the vertex buffer.
                VertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);

                // Create the index buffer.
                IndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indices);

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
            IndexBuffer?.Dispose();
            IndexBuffer = null;
            // Release the vertex buffer.
            VertexBuffer?.Dispose();
            VertexBuffer = null;
        }
        private void RenderBuffers(SharpDX.Direct3D11.DeviceContext deviceContext)
        {
            // Set the vertex buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<DVertex>(), 0));
            // Set the index buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }
    }
}