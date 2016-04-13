using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop;

namespace DSharpDXRastertek.Series2.Tut05.Graphics
{
    public class DModel
    {
        private SharpDX.Direct3D11.Buffer VertexBuffer { get; set; }
        private SharpDX.Direct3D11.Buffer IndexBuffer { get; set; }
        private int VertexCount { get; set; }
        public int IndexCount { get; set; }
        public DTexture Texture { get; private set; }

        public DModel() { }

        public bool Initialize(Device device, string textureFileName)
        {
            if (!InitializeBuffers(device))
                return false;
            if (!LoadTexture(device, textureFileName))
                return false;

            return true;
        }
        private bool LoadTexture(SharpDX.Direct3D11.Device device, string textureFileName)
        {
            Texture = new DTexture();
            return Texture.Initialize(device, textureFileName);
        }
        public void ShutDown()
        {
            ReleaseTexture();
            ShutDownBuffers();
        }
        public void Render(DeviceContext deviceContext)
        {
            RenderBuffers(deviceContext);
        }
        private bool InitializeBuffers(Device device)
        {
            VertexCount = 3;
            IndexCount = 3;

            var vertices = new[]
            {
                new DTextureShader.DVertex()
                {
                    position = new RawVector3(-1, -1, 0),
                    texture = new RawVector2(0, 1)
                },
                new DTextureShader.DVertex()
                {
                    position = new RawVector3(0, 1, 0),
                    texture = new RawVector2(.5f, 0)
                },
                new DTextureShader.DVertex()
                {
                    position = new RawVector3(1, -1, 0),
                    texture = new RawVector2(1, 1)
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
        private void ReleaseTexture()
        {
            Texture?.ShutDown();
            Texture = null;
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
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<DTextureShader.DVertex>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, SharpDX.DXGI.Format.R32_UInt, 0);
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }
    }
}
