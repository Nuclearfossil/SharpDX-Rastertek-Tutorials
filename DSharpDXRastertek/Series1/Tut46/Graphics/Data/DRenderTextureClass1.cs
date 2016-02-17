using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace DSharpDXRastertek.Tut46.Graphics.Data
{
    public class DRenderTexture                 // 146 lines
    {
        // Properties
        private Texture2D RenderTargetTexture { get; set; }
        private RenderTargetView RenderTargetView { get; set; }
        public ShaderResourceView ShaderResourceView { get; private set; }
        private Texture2D DepthStencilBuffer { get; set; }
        public DepthStencilView DepthStencilView { get; set; }
        public ViewportF ViewPort { get; set; }
        public Matrix OrthoMatrix { get; set; }
        public int TextureWidth { get; set; }
        public int TextureHeight { get; set; }

        // Puvlix Methods
        public bool Initialize(SharpDX.Direct3D11.Device device, int givenWidth, int givenHeight, float givenNear, float givenFar)
        {
            TextureWidth = givenWidth;
            TextureHeight = givenHeight;

            try
            {
                // Initialize and set up the render target description.
                Texture2DDescription textureDesc = new Texture2DDescription()
                {
                    Width = TextureWidth,
                    Height = TextureHeight,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = Format.R32G32B32A32_Float,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None
                };

                // Create the render target texture.
                RenderTargetTexture = new Texture2D(device, textureDesc);

                // Initialize and setup the render target view 
                RenderTargetViewDescription renderTargetViewDesc = new RenderTargetViewDescription()
                {
                    Format = textureDesc.Format,
                    Dimension = RenderTargetViewDimension.Texture2D
                };
                renderTargetViewDesc.Texture2D.MipSlice = 0;

                // Create the render target view.
                RenderTargetView = new RenderTargetView(device, RenderTargetTexture, renderTargetViewDesc);

                // Initialize and setup the shader resource view 
                ShaderResourceViewDescription shaderResourceViewDesc = new ShaderResourceViewDescription()
                {
                    Format = textureDesc.Format,
                    Dimension = ShaderResourceViewDimension.Texture2D,
                };
                shaderResourceViewDesc.Texture2D.MipLevels = 1;
                shaderResourceViewDesc.Texture2D.MostDetailedMip = 0;

                // Create the render target view.
                ShaderResourceView = new ShaderResourceView(device, RenderTargetTexture, shaderResourceViewDesc);
                
                // Initialize the description of the depth buffer.
                Texture2DDescription depthBufferDesc = new Texture2DDescription()
                {
                    Width = TextureWidth,
                    Height = TextureHeight,
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
                
                // Initialize the depth stencil view.
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
                ViewPort = new ViewportF(0.0f, 0.0f, (float)TextureWidth, (float)TextureHeight, 0.0f, 1.0f);   
                
                // Create an orthographic projection matrix for 2D rendering.
                OrthoMatrix = Matrix.OrthoLH(TextureWidth, TextureHeight, givenNear, givenFar);

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
            ShaderResourceView?.Dispose();
            ShaderResourceView = null;
            RenderTargetView?.Dispose();
            RenderTargetView = null;
            RenderTargetTexture?.Dispose();
            RenderTargetTexture = null;
        }
        public void SetRenderTarget(DeviceContext context)
        {
            // Bind the render target view and depth stencil buffer to the output pipeline.
            context.OutputMerger.SetTargets(DepthStencilView, RenderTargetView);

            // Set the viewport.
            context.Rasterizer.SetViewport(ViewPort);
        }
        public void ClearRenderTarget(DeviceContext context, float red, float green, float blue, float alpha)
        {
            // Setup the color the buffer to.
            var color = new Color4(red, green, blue, alpha);

            // Clear the back buffer.
            context.ClearRenderTargetView(RenderTargetView, color);

            // Clear the depth buffer.
            context.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
        }
    }
}