using DSharpDXRastertek.Series2.Tut05.System;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;

namespace DSharpDXRastertek.Series2.Tut05.Graphics
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
        public DepthStencilState DepthStencilState { get; set; }
        private DepthStencilView DepthStencilView { get; set; }
        private RasterizerState RasterState { get; set; }
        public RawMatrix ProjectionMatrix { get; private set; }
        public RawMatrix WorldMatrix { get; private set; }

        public DDX11() { }

        public bool Initialize(DSystemConfiguration configuration, IntPtr windowHandle)
        {
            try
            {
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
                VideoCardDescription = adapterDescription.Description;
                monitor.Dispose();
                adapter.Dispose();
                factory.Dispose();

                var swapChainDesc = new SwapChainDescription()
                {
                    BufferCount = 1,
                    ModeDescription = new ModeDescription(configuration.Width, configuration.Height, rational, Format.R8G8B8A8_UNorm),
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

                var backBuffer = Texture2D.FromSwapChain<Texture2D>(SwapChain, 0);
                RenderTargetView = new RenderTargetView(device, backBuffer);
                backBuffer.Dispose();

                Texture2DDescription depthBufferDesc = new Texture2DDescription()
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

                DepthStencilStateDescription depthStencilDesc = new DepthStencilStateDescription()
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
                DeviceContext.OutputMerger.SetDepthStencilState(DepthStencilState, 1);

                DepthStencilViewDescription depthStencilViewDesc = new DepthStencilViewDescription()
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

                RasterizerStateDescription rasterDesc = new RasterizerStateDescription()
                {
                    IsAntialiasedLineEnabled = false,
                    CullMode = CullMode.Back,
                    DepthBias = 0,
                    DepthBiasClamp = .0f,
                    IsDepthClipEnabled = true,
                    FillMode = FillMode.Solid,
                    IsFrontCounterClockwise = false,
                    IsMultisampleEnabled = false,
                    IsScissorEnabled = false,
                    SlopeScaledDepthBias = .0f
                };
                RasterState = new RasterizerState(Device, rasterDesc);
                DeviceContext.Rasterizer.State = RasterState;
                DeviceContext.Rasterizer.SetViewport(0, 0, configuration.Width, configuration.Height, 0, 1);
                ProjectionMatrix = Matrix.PerspectiveFovLH((float)(Math.PI / 4), ((float)configuration.Width / (float)configuration.Height), DSystemConfiguration.ScreenNear, DSystemConfiguration.ScreenDepth);
                WorldMatrix = Matrix.Identity;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void ShutDown()
        {
            SwapChain?.SetFullscreenState(false, null);
            RasterState?.Dispose();
            RasterState = null;
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
        public void BeginScene(float red, float green, float blue, float alpha)
        {
            BeginScene(new Color4(red, green, blue, alpha));
        }
        private void BeginScene(RawColor4 givenColour)
        {
            DeviceContext.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth, 1, 0);
            DeviceContext.ClearRenderTargetView(RenderTargetView, givenColour);
        }
        public void EndScene()
        {
            if (VerticalSyncEnabled)
                SwapChain.Present(1, PresentFlags.None);
            else
                SwapChain.Present(0, PresentFlags.None);
        }
    }
}
