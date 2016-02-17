using DSharpDXRastertek.TutTerr08.Graphics.Data;
using DSharpDXRastertek.TutTerr08.System;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Runtime.InteropServices;

namespace DSharpDXRastertek.TutTerr08.Graphics.Models
{
    public class DBitmap                    // 226 lines
    {
        // Structs
        [StructLayout(LayoutKind.Sequential)]
        public struct DVertexType
        {
            public Vector3 position;
            public Vector2 texture;
        }

        // Properties.
        public SharpDX.Direct3D11.Buffer VertexBuffer { get; set; }
        public SharpDX.Direct3D11.Buffer IndexBuffer { get; set; }
        public int VertexCount { get; set; }
        public int IndexCount { get; private set; }
        public DTexture Texture { get; private set; }
        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        public int BitmapWidth { get; private set; }
        public int BitmapHeight { get; private set; }

        // Constructor
        public DBitmap() { }

        // Methods
        public bool Initialize(SharpDX.Direct3D11.Device device, int screeenWidth, string textureFileName, string glowMapFileName, int screenHeight, int bitmapWidth, int bitmapHeight)
        {
            return Initialize(device, screeenWidth, screenHeight, textureFileName, bitmapWidth, bitmapHeight);
        }
        public bool Initialize(SharpDX.Direct3D11.Device device, int screeenWidth, int screenHeight, string textureFileName, int bitmapWidth, int bitmapHeight)
        {
            // Store the screen size.
            ScreenWidth = screeenWidth;
            ScreenHeight = screenHeight;

            // Store the size in pixels that this bitmap should be rendered at.
            BitmapWidth = bitmapWidth;
            BitmapHeight = bitmapHeight;

            // Initialize the vertex and index buffer.
            if (!InitializeBuffers(device))
                return false;

            // Load the texture for this model.
            if (!LoadTexture(device, textureFileName))
                return false;

            return true;
        }
        public void Shutdown()
        {
            // Release the model texture.
            ReleaseTexture();

            // Release the vertex and index buffers.
            ShutdownBuffers();
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
        public bool Render(DeviceContext deviceContext, int positionX, int positionY)
        {
            // Re-build the dynamic vertex buffer for rendering to possibly a different location on the screen.
            if (!UpdateBuffers(deviceContext, positionX, positionY))
                return false;

            // Put the vertex and index buffers on the graphics pipeline to prepare for drawings.
            RenderBuffers(deviceContext);

            return true;
        }
        private bool InitializeBuffers(SharpDX.Direct3D11.Device device)
        {
            try
            {
                // Set the number of the vertices and indices in the vertex and index array, accordingly.
                VertexCount = 6;
                IndexCount = 6;

                // Create the vertex array.
                var vertices = new DVertexType[VertexCount];
                // Create the index array.
                var indices = new int[IndexCount];

                // Load the index array with data.
                for (var i = 0; i < IndexCount; i++)
                    indices[i] = i;

                // Set up the description of the static vertex buffer.
                var vertexBufferDesc = new BufferDescription()
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<DVertexType>() * VertexCount,
                    BindFlags = BindFlags.VertexBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                };

                // Create the vertex buffer.
                VertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, vertices, vertexBufferDesc);

                // Create the index buffer.
                IndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indices);

                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool UpdateBuffers(DeviceContext deviceContext, int positionX, int positionY)
        {
            //// Calculate the screen coordinates of the left side of the bitmap.
            float left = (-(ScreenWidth / 2)) + (float)positionX;  // 
            // Calculate the screen coordinates of the right side of the bitmap.
            float right = left + BitmapWidth;
            // Calculate the screen coordinates of the top of the bitmap.
            float top = (ScreenHeight / 2) - (float)positionY;  //
            // Calculate the screen coordinates of the bottom of the bitmap.
            float bottom = top - BitmapHeight;

            // Create and load the vertex array.
            var vertices = new[] 
			{  
				new DVertexType()    // First triangle.
				{    // Top left.
					position = new Vector3(left, top, 0),
					texture = new Vector2(0, 0)
				},
				new DVertexType()
				{    // Bottom right.
					position = new Vector3(right, bottom, 0),
					texture = new Vector2(1, 1)
				},
				new DVertexType()
				{   // Bottom left.
					position = new Vector3(left, bottom, 0),
					texture = new Vector2(0, 1)
				},
				new DVertexType()   // Second triangle.
				{    // Top left.
					position = new Vector3(left, top, 0),
					texture = new Vector2(0, 0)
				},
				new DVertexType()
				{   // Top right.
					position = new Vector3(right, top, 0),
					texture = new Vector2(1, 0)
				},
				new DVertexType()
				{   // Bottom right.
					position = new Vector3(right, bottom, 0),
					texture = new Vector2(1, 1)
				}
			};

            DataStream mappedResource;

            #region Vertex Buffer
            // Lock the vertex buffer so it can be written to.
            deviceContext.MapSubresource(VertexBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out mappedResource);

            // Copy the data into the vertex buffer.
            mappedResource.WriteRange<DVertexType>(vertices);

            // Unlock the vertex buffer.
            deviceContext.UnmapSubresource(VertexBuffer, 0);
            #endregion

            return true;
        }
        private void RenderBuffers(DeviceContext deviceContext)
        {
            // Set vertex buffer stride and offset.
            var stride = Utilities.SizeOf<DVertexType>();
            var offset = 0;

            // Set the vertex buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, stride, offset));
            
            // Set the index buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
            
            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }
        private bool LoadTexture(SharpDX.Direct3D11.Device device, string textureFileName)
        {
            textureFileName = DSystemConfiguration.DataFilePath + textureFileName;
            // glowMapFileName = DSystemConfiguration.DataFilePath + glowMapFileName;

            // Create the texture object.
            Texture = new DTexture();

            // Initialize the texture object.
            if (!Texture.Initialize(device, textureFileName))
                return false;

            return true;
        }
        private void ReleaseTexture()
        {
            // Release the texture objects.
            Texture?.ShutDown();
            Texture = null;
        }
    }
}