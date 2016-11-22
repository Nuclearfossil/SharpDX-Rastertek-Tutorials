using DSharpDXRastertek.Series2.TutTerr13.System;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;

namespace DSharpDXRastertek.Series2.TutTerr13.Graphics
{
    public class DDX11
    {
        private bool VerticalSyncEnabled { get; set; }
        public int VideoCardMemory { get; private set; }
        public string VideoCardDescription { get; private set; }
        private SwapChain SwapChain { get; set; }
        public SharpDX.Direct3D11.Device Device { get; private set; }
        public DeviceContext DeviceContext { get; private set; }
        private RenderTargetView RenderTargetView { get; set; }
        private Texture2D DepthStencilBuffer { get; set; }
        public DepthStencilState DepthStencilState { get; private set; }
        public DepthStencilView DepthStencilView { get; set; }
        private RasterizerState RasterState { get; set; }
        public RasterizerState RasterStateWirefram { get; set; }
        public Matrix ProjectionMatrix { get; private set; }
        public Matrix WorldMatrix { get; private set; }
        public Matrix OrthoMatrix { get; private set; }
        public DepthStencilState DepthDisabledStencilState { get; private set; }
        public BlendState AlphaEnableBlendingState { get; private set; }
        public BlendState AlphaDisableBlendingState { get; private set; }
        public ViewportF ViewPort { get; set; }
        private RasterizerState RasterStateNoCulling { get; set; }

        public DDX11() { }

        public bool Initialize(DSystemConfiguration configuration, IntPtr windowHandle)
        {
            try
            {
                #region Environment Configuration
                VerticalSyncEnabled = DSystemConfiguration.VerticalSyncEnabled;
                var factory = new Factory1();
                var adapter = factory.GetAdapter1(0);
                var monitor = adapter.GetOutput(0);
                var modes = monitor.GetDisplayModeList(Format.R8G8B8A8_UNorm, DisplayModeEnumerationFlags.Interlaced);
                var rational = new Rational(0, 1);
                if (VerticalSyncEnabled)
                {
                    foreach (var mode in modes)
                    {
                        if (mode.Width == configuration.Width && mode.Height == configuration.Height)
                        {
                            rational = new Rational(mode.RefreshRate.Numerator, mode.RefreshRate.Denominator);
                            break;
                        }
                    }
                }
                var adapterDescription = adapter.Description;
                VideoCardMemory = adapterDescription.DedicatedVideoMemory >> 10 >> 10;
                VideoCardDescription = string.Format("VideoCard: {0}", adapterDescription.Description.Trim('\0'));
                monitor.Dispose();
                adapter.Dispose();
                factory.Dispose();
                #endregion

                #region Initialize swap chain and d3d device
                var swapChainDesc = new SwapChainDescription()
                {
                    BufferCount = 1,
                    ModeDescription = new ModeDescription(configuration.Width, configuration.Height, rational, Format.R8G8B8A8_UNorm) { Scaling = DisplayModeScaling.Unspecified, ScanlineOrdering = DisplayModeScanlineOrder.Unspecified },
                    Usage = Usage.RenderTargetOutput,
                    OutputHandle = windowHandle, 
                    SampleDescription = new SampleDescription(1, 0),
                    IsWindowed = !DSystemConfiguration.FullScreen,
                    Flags = SwapChainFlags.None,
                    SwapEffect = SwapEffect.Discard
                };
                SharpDX.Direct3D11.Device device;
                SwapChain swapChain;
                SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, swapChainDesc, out device, out swapChain);
                Device = device;
                SwapChain = swapChain;
                DeviceContext = device.ImmediateContext;
                #endregion

                #region Initialize buffers
                var backBuffer = Texture2D.FromSwapChain<Texture2D>(SwapChain, 0);
                RenderTargetView = new RenderTargetView(device, backBuffer);
                backBuffer.Dispose();
                var depthBufferDesc = new Texture2DDescription()
                {
                    Width = configuration.Width,
                    Height = configuration.Height,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = Format.D24_UNorm_S8_UInt,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.DepthStencil,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None
                };
                DepthStencilBuffer = new Texture2D(device, depthBufferDesc);
                #endregion

                #region Initialize Depth Enabled Stencil
                var depthStencilDesc = new DepthStencilStateDescription()
                {
                    IsDepthEnabled = true,
                    DepthWriteMask = DepthWriteMask.All,
                    DepthComparison = Comparison.Less,
                    IsStencilEnabled = true,
                    StencilReadMask = 0xFF,
                    StencilWriteMask = 0xFF,
                    FrontFace = new DepthStencilOperationDescription()
                    {
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Increment,
                        PassOperation = StencilOperation.Keep,
                        Comparison = Comparison.Always
                    },
                    BackFace = new DepthStencilOperationDescription()
                    {
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Decrement,
                        PassOperation = StencilOperation.Keep,
                        Comparison = Comparison.Always
                    }
                };
                DepthStencilState = new DepthStencilState(Device, depthStencilDesc);
                #endregion

                #region Initialize Output Merger
                DeviceContext.OutputMerger.SetDepthStencilState(DepthStencilState, 1);
                var depthStencilViewDesc = new DepthStencilViewDescription()
                {
                    Format = Format.D24_UNorm_S8_UInt,
                    Dimension = DepthStencilViewDimension.Texture2D,
                    Texture2D = new DepthStencilViewDescription.Texture2DResource()
                    {
                        MipSlice = 0
                    }
                };
                DepthStencilView = new DepthStencilView(Device, DepthStencilBuffer, depthStencilViewDesc);
                DeviceContext.OutputMerger.SetTargets(DepthStencilView, RenderTargetView);
                #endregion

                #region Initialize Raster State
                var rasterDesc = new RasterizerStateDescription()
                {
                    IsAntialiasedLineEnabled = false,
                    CullMode = CullMode.Back,
                    DepthBias = 0,
                    DepthBiasClamp = 0.0f,
                    IsDepthClipEnabled = true,
                    FillMode = FillMode.Solid,
                    IsFrontCounterClockwise = false,
                    IsMultisampleEnabled = false,
                    IsScissorEnabled = false,
                    SlopeScaledDepthBias = 0.0f
                };
                RasterState = new RasterizerState(Device, rasterDesc);

                var rasterDescWireFrame = new RasterizerStateDescription()
                {
                    IsAntialiasedLineEnabled = false,
                    CullMode = CullMode.Back,
                    DepthBias = 0,
                    DepthBiasClamp = 0.0f,
                    IsDepthClipEnabled = true,
                    FillMode = FillMode.Wireframe,
                    IsFrontCounterClockwise = false,
                    IsMultisampleEnabled = false,
                    IsScissorEnabled = false,
                    SlopeScaledDepthBias = 0.0f
                };
                RasterStateWirefram = new RasterizerState(Device, rasterDescWireFrame);

                // Setup a raster description which turns off back face culling.
                var rasterNoCullDesc = new RasterizerStateDescription()
                {
                    IsAntialiasedLineEnabled = false,
                    CullMode = CullMode.None,
                    DepthBias = 0,
                    DepthBiasClamp = .0f,
                    IsDepthClipEnabled = true,
                    FillMode = FillMode.Solid,
                    IsFrontCounterClockwise = false,
                    IsMultisampleEnabled = false,
                    IsScissorEnabled = false,
                    SlopeScaledDepthBias = .0f
                };

                // Create the no culling rasterizer state.
                RasterStateNoCulling = new RasterizerState(Device, rasterNoCullDesc);
                #endregion

                #region Initialize Rasterizer
                DeviceContext.Rasterizer.State = RasterState;
                ViewPort = new ViewportF(0.0f, 0.0f, (float)configuration.Width, (float)configuration.Height, 0.0f, 1.0f);
                DeviceContext.Rasterizer.SetViewport(ViewPort);
                #endregion

                #region Initialize matrices
                ProjectionMatrix = Matrix.PerspectiveFovLH((float)(Math.PI / 4), ((float)configuration.Width / (float)configuration.Height), DSystemConfiguration.ScreenNear, DSystemConfiguration.ScreenDepth);
                WorldMatrix = Matrix.Identity;
                OrthoMatrix = Matrix.OrthoLH(configuration.Width, configuration.Height, DSystemConfiguration.ScreenNear, DSystemConfiguration.ScreenDepth);
                #endregion

                #region Initialize Depth Disabled Stencil
                var depthDisabledStencilDesc = new DepthStencilStateDescription()
                {
                    IsDepthEnabled = false,
                    DepthWriteMask = DepthWriteMask.All,
                    DepthComparison = Comparison.Less,
                    IsStencilEnabled = true,
                    StencilReadMask = 0xFF,
                    StencilWriteMask = 0xFF,
                    FrontFace = new DepthStencilOperationDescription()
                    {
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Increment,
                        PassOperation = StencilOperation.Keep,
                        Comparison = Comparison.Always
                    },
                    BackFace = new DepthStencilOperationDescription()
                    {
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Decrement,
                        PassOperation = StencilOperation.Keep,
                        Comparison = Comparison.Always
                    }
                };
                DepthDisabledStencilState = new DepthStencilState(Device, depthDisabledStencilDesc);
                #endregion

                #region Initialize Blend States
                var blendStateDesc = new BlendStateDescription();
                blendStateDesc.AlphaToCoverageEnable = false;
                blendStateDesc.IndependentBlendEnable = false;
                blendStateDesc.RenderTarget[0].IsBlendEnabled = true;
                blendStateDesc.RenderTarget[0].BlendOperation = BlendOperation.Add;
                blendStateDesc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
                blendStateDesc.RenderTarget[0].SourceBlend = BlendOption.One;
                blendStateDesc.RenderTarget[0].DestinationBlend = BlendOption.One;
                blendStateDesc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
                blendStateDesc.RenderTarget[0].DestinationAlphaBlend = BlendOption.Zero;
                blendStateDesc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
                AlphaEnableBlendingState = new BlendState(device, blendStateDesc);

                blendStateDesc.RenderTarget[0].IsBlendEnabled = false;
                blendStateDesc.AlphaToCoverageEnable = false;
                AlphaDisableBlendingState = new BlendState(device, blendStateDesc);
                #endregion

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void ShutDown()
        {
            SwapChain?.SetFullscreenState(false, null);
            AlphaEnableBlendingState?.Dispose();
            AlphaEnableBlendingState = null;
            AlphaDisableBlendingState?.Dispose();
            AlphaDisableBlendingState = null;
            DepthDisabledStencilState?.Dispose();
            DepthDisabledStencilState = null;
            RasterState?.Dispose();
            RasterState = null;
            RasterStateNoCulling?.Dispose();
            RasterStateNoCulling = null;
            RasterStateWirefram?.Dispose();
            RasterStateWirefram = null;
            DepthStencilView?.Dispose();
            DepthStencilView = null;
            DepthStencilState?.Dispose();
            DepthStencilState = null;
            DepthStencilBuffer?.Dispose();
            DepthStencilBuffer = null;
            RenderTargetView?.Dispose();
            RenderTargetView = null;
            DeviceContext?.Dispose();
            DeviceContext = null;
            Device?.Dispose();
            Device = null;
            SwapChain?.Dispose();
            SwapChain = null;
        }
        public void TurnOnAlphaBlending()
        {
            var blendFactor = new Color4(0, 0, 0, 0);
            DeviceContext.OutputMerger.SetBlendState(AlphaEnableBlendingState, blendFactor, -1);
        }
        public void TurnOffAlphaBlending()
        {
            var blendFactor = new Color4(0, 0, 0, 0);
            DeviceContext.OutputMerger.SetBlendState(AlphaDisableBlendingState, blendFactor, -1);
        }
        public void BeginScene(float red, float green, float blue, float alpha)
        {
            BeginScene(new Color4(red, green, blue, alpha));
        }
        public void BeginScene(Color4 color)
        {
            DeviceContext.ClearRenderTargetView(RenderTargetView, color);
            DeviceContext.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
        }
        public void EndScene()
        {
            if (VerticalSyncEnabled)
            {
                SwapChain.Present(1, PresentFlags.None);
            }
            else
            {
                SwapChain.Present(0, PresentFlags.None);
            }
        }
        public void TurnZBufferOn()
        {
            DeviceContext.OutputMerger.SetDepthStencilState(DepthStencilState, 1);
        }
        public void TurnZBufferOff()
        {
            DeviceContext.OutputMerger.SetDepthStencilState(DepthDisabledStencilState, 1);
        }
        public void EnableWireFrame()
        {
            // Set the wire frame rasterizer state.
            DeviceContext.Rasterizer.State = RasterStateWirefram;
        }
        public void DisableWireFrame()
        {
            // Set the solid fill rasterizer state.
            DeviceContext.Rasterizer.State = RasterState;
        }
        public void TurnOnCulling()
        {
            // Set the culling rasterizer state.
            DeviceContext.Rasterizer.State = RasterState;
        }
        public void TurnOffCulling()
        {
            DeviceContext.Rasterizer.State = RasterStateNoCulling;
        }
    }
}
