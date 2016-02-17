using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace DSharpDXRastertek.Tut50.Graphics.Data
{
    public class DDeferredBuffers                   // 173 lines
    {
        // Variables
        private int BUFFER_COUNT = 2;

        // Properties
        private Texture2D[] RenderTargetTexture2DArray { get; set; }
        private RenderTargetView[] RenderTargetViewArray { get; set; }
        public ShaderResourceView[] ShaderResourceViewArray { get; private set; }
        private Texture2D DepthStencilBuffer { get; set; }
        private DepthStencilView DepthStencilView { get; set; }
        private ViewportF ViewPort { get; set; }

        // Constructor
        public DDeferredBuffers()
        {
            // Initialize Arrays to size.
            RenderTargetTexture2DArray = new Texture2D[BUFFER_COUNT];
            RenderTargetViewArray = new RenderTargetView[BUFFER_COUNT];
            ShaderResourceViewArray = new ShaderResourceView[BUFFER_COUNT];
        }

        // Puvlix Methods
        public bool Initialize(SharpDX.Direct3D11.Device device, int textureWidth, int textureHeight)
        {
            try
            {
                // Initialize the render target texture description.
                Texture2DDescription textureDesc = new Texture2DDescription()
                {
                    Width = textureWidth,
                    Height = textureHeight,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = Format.R32G32B32A32_Float,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None
                };

                // Create the render target textures.
                for (int i = 0; i < BUFFER_COUNT; i++)
                    RenderTargetTexture2DArray[i] = new Texture2D(device, textureDesc);

                // Initialize and setup the render target view 
                RenderTargetViewDescription renderTargetViewDesc = new RenderTargetViewDescription()
                {
                    Format = textureDesc.Format,
                    Dimension = RenderTargetViewDimension.Texture2D,
                };
                renderTargetViewDesc.Texture2D.MipSlice = 0;

                // Create the render target view.
                for (int i = 0; i < BUFFER_COUNT; i++)
                    RenderTargetViewArray[i] = new RenderTargetView(device, RenderTargetTexture2DArray[i], renderTargetViewDesc);

                // Initialize and setup the shader resource view 
                ShaderResourceViewDescription shaderResourceViewDesc = new ShaderResourceViewDescription()
                {
                    Format = textureDesc.Format,
                    Dimension = ShaderResourceViewDimension.Texture2D,
                };
                shaderResourceViewDesc.Texture2D.MipLevels = 1;
                shaderResourceViewDesc.Texture2D.MostDetailedMip = 0;

                // Create the render target view.
                for (int i = 0; i < BUFFER_COUNT; i++)
                    ShaderResourceViewArray[i] = new ShaderResourceView(device, RenderTargetTexture2DArray[i], shaderResourceViewDesc);

                // Initialize the description of the depth buffer.
                Texture2DDescription depthBufferDesc = new Texture2DDescription()
                {
                    Width = textureWidth,
                    Height = textureHeight,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = Format.D24_UNorm_S8_UInt,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.DepthStencil,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None
                };

                // Create the texture for the depth buffer using the filled out description.
                DepthStencilBuffer = new Texture2D(device, depthBufferDesc);

                // Set up the depth stencil view description.
                DepthStencilViewDescription depthStencilViewBufferDesc = new DepthStencilViewDescription()
                {
                    Format = SharpDX.DXGI.Format.D24_UNorm_S8_UInt,
                    Dimension = DepthStencilViewDimension.Texture2D
                };
                depthStencilViewBufferDesc.Texture2D.MipSlice = 0;

                // Create the depth stencil view.
                DepthStencilView = new DepthStencilView(device, DepthStencilBuffer, depthStencilViewBufferDesc);

                // Setup the viewport for rendering.
                ViewPort = new ViewportF()
                {
                    Width = (float)textureWidth,
                    Height = (float)textureHeight,
                    MinDepth = 0.0f,
                    MaxDepth = 1.0f,
                    X = 0.0f,
                    Y = 0.0f
                };

                return true;
            }
			catch
			{
				return false;
			}
        }
        public void Shutdown()
        {
            DepthStencilView?.Dispose();
            DepthStencilView = null;
            DepthStencilBuffer?.Dispose();
            DepthStencilBuffer = null;
            for (int i = 0; i < BUFFER_COUNT; i++)
            {
                ShaderResourceViewArray[i]?.Dispose();
                ShaderResourceViewArray[i] = null;
            }
            ShaderResourceViewArray = null;
            for (int i = 0; i < BUFFER_COUNT; i++)
            {
                RenderTargetViewArray[i]?.Dispose();
                RenderTargetViewArray[i] = null;
            }
            RenderTargetViewArray = null;
            for (int i = 0; i < BUFFER_COUNT; i++)
            {
                RenderTargetTexture2DArray[i]?.Dispose();
                RenderTargetTexture2DArray[i] = null;
            }
            RenderTargetTexture2DArray = null;
        }
        public void SetRenderTargets(DeviceContext deviceContext)
        {
            // Bind the render target view array and depth stencil buffer to the output render pipeline.
            deviceContext.OutputMerger.SetTargets(DepthStencilView, BUFFER_COUNT, RenderTargetViewArray);
            // deviceContext.OutputMerger.SetTargets(DepthStencilView, RenderTargetViewArray);

            // Set the viewport.
            deviceContext.Rasterizer.SetViewport(ViewPort);
        }
        public void ClearRenderTargets(DeviceContext deviceContext, float red, float green, float blue, float alpha)
        {
            // Setup the color the buffer to.
            var color = new Color4(red, green, blue, alpha);

            // Clear the render target buffers.
            foreach (RenderTargetView renderTargetView in RenderTargetViewArray)
                deviceContext.ClearRenderTargetView(renderTargetView, color);

            // Clear the depth buffer.
            deviceContext.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
        }
    }
}