using DSharpDXRastertek.Series2.Tut04.System;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop;
using System;
using System.Runtime.InteropServices;

namespace DSharpDXRastertek.Series2.Tut04.Graphics
{
    public class DColorShader
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct DVertex
        {
            public RawVector3 position;
            public RawVector4 color;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct DMatrixBuffer
        {
            public RawMatrix world;
            public RawMatrix view;
            public RawMatrix projection;
        }

        public VertexShader VertexShader { get; set; }
        public PixelShader PixelShader { get; set; }
        public InputLayout Layout { get; set; }
        public SharpDX.Direct3D11.Buffer ConstantMatrixBuffer { get; set; }

        public DColorShader() { }

        public bool Initialize(Device device, IntPtr windowsHandle)
        {
            return InitializeShader(device, windowsHandle, "Color.vs", "Color.ps");
        }
        private bool InitializeShader(Device device, IntPtr windowsHandle, string vsFileName, string psFileName)
        {
            try
            {
                vsFileName = DSystemConfiguration.ShaderFilePath + vsFileName;
                psFileName = DSystemConfiguration.ShaderFilePath + psFileName;
                ShaderBytecode vertexShaderByteCode = ShaderBytecode.CompileFromFile(vsFileName, "ColorVertexShader", "vs_4_0", ShaderFlags.None, EffectFlags.None);
                ShaderBytecode pixelShaderByteCode = ShaderBytecode.CompileFromFile(psFileName, "ColorPixelShader", "ps_4_0", ShaderFlags.None, EffectFlags.None);
                VertexShader = new VertexShader(device, vertexShaderByteCode);
                PixelShader = new PixelShader(device, pixelShaderByteCode);

                InputElement[] inputElements = new InputElement[]
                {
                    new InputElement()
                    {
                        SemanticName = "POSITION",
                        SemanticIndex = 0,
                        Format = SharpDX.DXGI.Format.R32G32B32_Float,
                        Slot = 0,
                        AlignedByteOffset = 0,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElement()
                    {
                        SemanticName = "COLOR",
                        SemanticIndex = 0,
                        Format = SharpDX.DXGI.Format.R32G32B32A32_Float,
                        Slot = 0,
                        AlignedByteOffset = InputElement.AppendAligned,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    }
                };

                Layout = new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), inputElements);
                vertexShaderByteCode.Dispose();
                pixelShaderByteCode.Dispose();
                BufferDescription matrixBufDesc = new BufferDescription()
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<DMatrixBuffer>(), // was Matrix
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                };
                ConstantMatrixBuffer = new SharpDX.Direct3D11.Buffer(device, matrixBufDesc);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public void ShutDown()
        {
            ShuddownShader();
        }
        private void ShuddownShader()
        {
            ConstantMatrixBuffer?.Dispose();
            ConstantMatrixBuffer = null;
            Layout?.Dispose();
            Layout = null;
            PixelShader?.Dispose();
            PixelShader = null;
            VertexShader?.Dispose();
            VertexShader = null;
        }
        public bool Render(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            if (!SetShaderParameters(deviceContext, worldMatrix, viewMatrix, projectionMatrix))
                return false;

            RenderShader(deviceContext, indexCount);
            return true;
        }
        private bool SetShaderParameters(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix)
        {
            try
            {
                worldMatrix.Transpose();
                viewMatrix.Transpose();
                projectionMatrix.Transpose();

                DataStream mappedResource;
                deviceContext.MapSubresource(ConstantMatrixBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);
                DMatrixBuffer matrixBuffer = new DMatrixBuffer()
                {
                    world = worldMatrix,
                    view = viewMatrix,
                    projection = projectionMatrix
                };
                mappedResource.Write(matrixBuffer);
                deviceContext.UnmapSubresource(ConstantMatrixBuffer, 0);
                int bufferSlotNuber = 0;
                deviceContext.VertexShader.SetConstantBuffer(bufferSlotNuber, ConstantMatrixBuffer);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void RenderShader(DeviceContext deviceContext, int indexCount)
        {
            deviceContext.InputAssembler.InputLayout = Layout;
            deviceContext.VertexShader.Set(VertexShader);
            deviceContext.PixelShader.Set(PixelShader);
            deviceContext.DrawIndexed(indexCount, 0, 0);
        }
    }
}
