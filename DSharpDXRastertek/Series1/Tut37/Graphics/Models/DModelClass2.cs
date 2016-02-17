using DSharpDXRastertek.Tut37.Graphics.Data;
using DSharpDXRastertek.Tut37.System;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System.Runtime.InteropServices;

namespace DSharpDXRastertek.Tut37.Graphics.Models
{
    public class DModel                 // 161 lines
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct DVertexType
        {
            public Vector3 position;
            public Vector2 texture;
        };
        [StructLayout(LayoutKind.Sequential)]
        public struct DInstanceType
        {
            public Vector3 position;
        };

        // Properties
        private SharpDX.Direct3D11.Buffer VertexBuffer { get; set; }
        private SharpDX.Direct3D11.Buffer InstanceBuffer { get; set; }
        public int VertexCount { get; set; }
        public int InstanceCount { get; private set; }
        public DTexture Texture { get; private set; }

        // Constructor
        public DModel() { }

        // Methods.
        public bool Initialize(SharpDX.Direct3D11.Device device, string textureFileName)
        {
            // Initialize the vertex and index buffer that hold the geometry for the triangle.
            if (!InitializeBuffers(device))
                return false;

            if (!LoadTexture(device, textureFileName))
                return false;

            return true;
        }
        private bool InitializeBuffers(SharpDX.Direct3D11.Device device)
        {
            try
            {
                // Set number of vertices in the vertex array.
                VertexCount = 3;

                // Create the vertex array and load it with data.
                DVertexType[] vertices = new[]
                {
					// Bottom left.
					new DVertexType()
					{
						position = new Vector3(-1, -1, 0),
						texture = new Vector2(0, 1)
					},
					// Top middle.
					new DVertexType()
					{
						position = new Vector3(0, 1, 0),
						texture = new Vector2(.5f, 0)
					},
					// Bottom right.
					new DVertexType()
					{
						position = new Vector3(1, -1, 0),
						texture = new Vector2(1, 1)
					}
                };

                // Set the number of instances in the array.
                InstanceCount = 4;

                DInstanceType[] instances = new DInstanceType[]
                {
                    new DInstanceType()
                    {
                        position = new Vector3(-1.5f, -1.5f, 5.0f)
                    },
                    new DInstanceType()
                    {
                        position = new Vector3(-1.5f,  1.5f, 5.0f)
                    },
                    new DInstanceType()
                    {
                        position = new Vector3( 1.5f, -1.5f, 5.0f)
                    },
                    new DInstanceType()
                    {
                        position = new Vector3( 1.5f,  1.5f, 5.0f)
                    }
                };

                // Create the vertex buffer.
                VertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);
                
                // Create the Instance instead of an Index Buffer buffer.
                InstanceBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, instances);

                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool LoadTexture(SharpDX.Direct3D11.Device device, string textureFileName)
        {
            textureFileName = DSystemConfiguration.DataFilePath + textureFileName;

            // Create the texture object.
            Texture = new DTexture();

            // Initialize the texture object.
            bool result = Texture.Initialize(device, textureFileName);

            return result;
        }
        public void ShutDown()
        {
            // Release the model texture.
            ReleaseTexture();

            // Release the vertex and index buffers.
            ShutdownBuffers();
        }
        private void ReleaseTexture()
        {
            // Release the texture object.
            Texture?.ShutDown();
            Texture = null;
        }
        private void ShutdownBuffers()
        {
            // Release the Instance buffer.
            InstanceBuffer?.Dispose();
            InstanceBuffer = null;
            // Release the vertex buffer.
            VertexBuffer?.Dispose();
            VertexBuffer = null;
        }
        public void Render(DeviceContext deviceContext)
        {
            // Put the vertex and index buffers on the graphics pipeline to prepare for drawings.
            RenderBuffers(deviceContext);
        }
        private void RenderBuffers(DeviceContext deviceContext)
        {
            // Set the vertex buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<DVertexType>(), 0), new VertexBufferBinding(InstanceBuffer, Utilities.SizeOf<DInstanceType>(), 0));
            
            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }
    }
}