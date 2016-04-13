using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop;

namespace DSharpDXRastertek.Series2.Tut04.Graphics
{
    public class DModel
    {
        private SharpDX.Direct3D11.Buffer VertexBuffer { get; set; }
        private SharpDX.Direct3D11.Buffer IndexBuffer { get; set; }
        private int VertexCount { get; set; }
        public int IndexCount { get; set; }

        public DModel() { }

        public bool Initialize(Device device)
        {
            return InitializeBuffer(device);
        }
        public void ShutDown()
        {
            ShutDownBuffers();
        }
        public void Render(DeviceContext deviceContext)
        {
            RenderBuffers(deviceContext);
        }
        private bool InitializeBuffer(Device device)
        {
            VertexCount = 3;
            IndexCount = 3;

            var vertices = new[]
            {
                new DColorShader.DVertex()
                {
                    position = new RawVector3(-1, -1, 0),
                    color = new RawVector4(0, 1, 0, 1)
                },
                new DColorShader.DVertex()
                {
                    position = new RawVector3(0, 1, 0),
                    color = new RawVector4(0, 1, 0, 1)
                },
                new DColorShader.DVertex()
                {
                    position = new RawVector3(1, -1, 0),
                    color = new RawVector4(0, 1, 0, 1)
                }
            };

            int[] indicies = new int[]
            {
                    0,
                    1,
                    2
            };

            VertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);
            IndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indicies);

            vertices = null;
            indicies = null;

            return true;

        }
        private void ShutDownBuffers()
        {
            IndexBuffer?.Dispose();
            IndexBuffer = null;
            VertexBuffer?.Dispose();
            VertexBuffer = null;
        }
        private void RenderBuffers(DeviceContext deviceContext)
        {
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<DColorShader.DVertex>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, SharpDX.DXGI.Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }
    }
}