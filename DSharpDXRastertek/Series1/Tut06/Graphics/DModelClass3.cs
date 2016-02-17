using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace DSharpDXRastertek.Tut06.Graphics
{
    public class DModel                 // 136 lines
    {
        // Properties
        private SharpDX.Direct3D11.Buffer VertexBuffer { get; set; }
        private SharpDX.Direct3D11.Buffer IndexBuffer { get; set; }
        private int VertexCount { get; set; }
        public int IndexCount { get; private set; }
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
                // Set number of vertices in the index array.
                IndexCount = 3;

                // Create the vertex array and load it with data.
                var vertices = new[]
                {
					// Bottom left.
					new DLightShader.DVertex()
					{
						position = new Vector3(-1, -1, 0),
						texture = new Vector2(0, 1),
                        normal = new Vector3(0, 0, -1)
					},
					// Top middle.
					new DLightShader.DVertex()
					{
						position = new Vector3(0, 1, 0),
						texture = new Vector2(.5f, 0),
                        normal = new Vector3(0, 0, -1)
					},
					// Bottom right.
					new DLightShader.DVertex()
					{
						position = new Vector3(1, -1, 0),
						texture = new Vector2(1, 1),
                        normal = new Vector3(0, 0, -1)
					}
                };

                // Create Indicies to load into the IndexBuffer.
                int[] indices = new int[]
                {
                    0, // Bottom left.
					1, // Top middle.
					2  // Bottom right.
                };

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
        private bool LoadTexture(SharpDX.Direct3D11.Device device, string textureFileName)
        {
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
            // Return the index buffer.
            IndexBuffer?.Dispose();
            IndexBuffer = null;
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
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<DLightShader.DVertex>(), 0));
            // Set the index buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }
    }
}