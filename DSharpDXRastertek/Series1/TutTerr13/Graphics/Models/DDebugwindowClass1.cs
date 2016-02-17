using DSharpDXRastertek.TutTerr13.Graphics.Shaders;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace DSharpDXRastertek.TutTerr13.Graphics.Models
{
    public class DDebugWindow                   // 184 lines
    {
        // Properties.
        public SharpDX.Direct3D11.Buffer VertexBuffer { get; set; }
        public SharpDX.Direct3D11.Buffer IndexBuffer { get; set; }
        public int VertexCount { get; set; }
        public int IndexCount { get; private set; }
        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        public int BitmapWidth { get; private set; }
        public int BitmapHeight { get; private set; }

        // Constructor
        public DDebugWindow() { }

        // Methods
        public bool Initialize(SharpDX.Direct3D11.Device device, int screeenWidth, int screenHeight, int bitmapWidth, int bitmapHeight)
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

            return true;
        }
        public void Shutdown()
        {
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
                var vertices = new DTextureShader.DVertex[VertexCount];
                // Create the index array.
                var indices = new int[IndexCount];

                // Load the index array with data.
                for (var i = 0; i < IndexCount; i++)
                    indices[i] = i;

                // Set up the description of the static vertex buffer.
                var vertexBuffer = new BufferDescription()
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<DTextureShader.DVertex>() * VertexCount,
                    BindFlags = BindFlags.VertexBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                };

                // Create the vertex buffer.
                VertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, vertices, vertexBuffer);

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
            // Calculate the screen coordinates of the left side of the bitmap.
            var left = (-(ScreenWidth >> 1)) + (float)positionX;
            // Calculate the screen coordinates of the right side of the bitmap.
            var right = left + BitmapWidth;
            // Calculate the screen coordinates of the top of the bitmap.
            var top = (ScreenHeight >> 1) - (float)positionY;
            // Calculate the screen coordinates of the bottom of the bitmap.
            var bottom = top - BitmapHeight;

            // Create and load the vertex array.
            var vertices = new[] 
			{
				new DTextureShader.DVertex()
				{
					position = new Vector3(left, top, 0),
					texture = new Vector2(0, 0)
				},
				new DTextureShader.DVertex()
				{
					position = new Vector3(right, bottom, 0),
					texture = new Vector2(1, 1)
				},
				new DTextureShader.DVertex()
				{
					position = new Vector3(left, bottom, 0),
					texture = new Vector2(0, 1)
				},
				new DTextureShader.DVertex()
				{
					position = new Vector3(left, top, 0),
					texture = new Vector2(0, 0)
				},
				new DTextureShader.DVertex()
				{
					position = new Vector3(right, top, 0),
					texture = new Vector2(1, 0)
				},
				new DTextureShader.DVertex()
				{
					position = new Vector3(right, bottom, 0),
					texture = new Vector2(1, 1)
				}
			};

            DataStream mappedResource;

            #region Vertex Buffer
            // Lock the vertex buffer so it can be written to.
            deviceContext.MapSubresource(VertexBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out mappedResource);

            // Copy the data into the vertex buffer.
            mappedResource.WriteRange<DTextureShader.DVertex>(vertices);

            // Unlock the vertex buffer.
            deviceContext.UnmapSubresource(VertexBuffer, 0);
            #endregion

            return true;
        }
        private void RenderBuffers(DeviceContext deviceContext)
        {
            // Set vertex buffer stride and offset.
            var stride = Utilities.SizeOf<DTextureShader.DVertex>();
            var offset = 0;

            // Set the vertex buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, stride, offset));
            
            // Set the index buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
            
            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }
    }
}