using DSharpDXRastertek.Tut38.System;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut38.Graphics
{
    public class DColorShader                   // 256 lines
    {
        // Structures.
        [StructLayout(LayoutKind.Sequential)]
        internal struct DMatrixBuffer
        {
            public Matrix world;
            public Matrix view;
            public Matrix projection;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct DTessellationBufferType
        {
            public float tessellationAmount;
            public Vector3 padding;
        }

        // Properties.
        public VertexShader VertexShader { get; set; }
        public HullShader HullShader { get; set; }
        public DomainShader DomainShader { get; set; }
        public PixelShader PixelShader { get; set; }
        public InputLayout Layout { get; set; }
        public SharpDX.Direct3D11.Buffer ConstantMatrixBuffer { get; set; }
        public SharpDX.Direct3D11.Buffer ConstantTessellationBuffer { get; set; }

        // Constructor
        public DColorShader() { }

        // Methods.
        public bool Initialize(Device device, IntPtr windowsHandle)
        {
            // Initialize the vertex and pixel shaders.
            return InitializeShader(device, windowsHandle, "color.vs", "color.hs", "color.ds", "Color.ps");
        }
        private bool InitializeShader(Device device, IntPtr windowsHandle, string vsFileName, string hsFileName, string dsFileName, string psFileName)
        {
            try
            {
                // Setup full pathes
                vsFileName = DSystemConfiguration.ShaderFilePath + vsFileName;
                hsFileName = DSystemConfiguration.ShaderFilePath + hsFileName;
                dsFileName = DSystemConfiguration.ShaderFilePath + dsFileName;
                psFileName = DSystemConfiguration.ShaderFilePath + psFileName;

                // Compile the vertex shader code.
                ShaderBytecode vertexShaderByteCode = ShaderBytecode.CompileFromFile(vsFileName, "ColorVertexShader", DSystemConfiguration.VertexShaderProfile, ShaderFlags.None, EffectFlags.None);
                // Compile the Hull shader code.
                ShaderBytecode hullShaderByteCode = ShaderBytecode.CompileFromFile(hsFileName, "ColorHullShader", DSystemConfiguration.HullShaderProfile, ShaderFlags.None, EffectFlags.None);
                // Compile the Domain shader code.
                ShaderBytecode domainShaderByteCode = ShaderBytecode.CompileFromFile(dsFileName, "ColorDomainShader", DSystemConfiguration.DomainShaderProfile, ShaderFlags.None, EffectFlags.None);
                // Compile the pixel shader code.
                ShaderBytecode pixelShaderByteCode = ShaderBytecode.CompileFromFile(psFileName, "ColorPixelShader", DSystemConfiguration.PixelShaderProfile, ShaderFlags.None, EffectFlags.None);
                
                // Create the vertex shader from the buffer.
                VertexShader = new VertexShader(device, vertexShaderByteCode);
                // Create the vertex shader from the buffer.
                HullShader = new HullShader(device, hullShaderByteCode);
                // Create the vertex shader from the buffer.
                DomainShader = new DomainShader(device, domainShaderByteCode);
                // Create the pixel shader from the buffer.
                PixelShader = new PixelShader(device, pixelShaderByteCode);
                
                // Now setup the layout of the data that goes into the shader.
                // This setup needs to match the VertexType structure in the Model and in the shader.
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

                // Create the vertex input the layout.
                Layout = new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), inputElements);

                // Release the vertex and pixel shader buffers, since they are no longer needed.
                vertexShaderByteCode.Dispose();
                pixelShaderByteCode.Dispose();

                // Setup the description of the dynamic matrix constant buffer that is in the vertex shader.
                BufferDescription matrixBufDesc = new BufferDescription() 
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<DMatrixBuffer>(), // was Matrix
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                };

                // Create the constant buffer pointer so we can access the vertex shader constant buffer from within this class.
                ConstantMatrixBuffer = new SharpDX.Direct3D11.Buffer(device, matrixBufDesc);

                // Setup the description of the dynamic tessellation constant buffer that is in the hull shader.
                BufferDescription tessellationBufferDesc = new BufferDescription()
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<DTessellationBufferType>(), // was Matrix
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                };

                // Create the constant buffer pointer so we can access the hull shader constant buffer from within this class.
                ConstantTessellationBuffer = new SharpDX.Direct3D11.Buffer(device, tessellationBufferDesc);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing shader. Error is " + ex.Message);
                return false;
            }
        }
        public void ShutDown()
        {
            // Shutdown the vertex and pixel shaders as well as the related objects.
            ShuddownShader();
        }
        private void ShuddownShader()
        {
            // Release the tessellation constant buffer.
            ConstantTessellationBuffer?.Dispose();
            ConstantTessellationBuffer = null;
            // Release the matrix constant buffer.
            ConstantMatrixBuffer?.Dispose();
            ConstantMatrixBuffer = null;
            // Release the layout.
            Layout?.Dispose();
            Layout = null;
            // Release the Vertex shader.
            VertexShader?.Dispose();
            VertexShader = null;
            // Release the Hull shader.
            PixelShader?.Dispose();
            PixelShader = null;
            // Release the Domain shader.
            HullShader?.Dispose();
            HullShader = null;
            // Release the Pixel shader.
            DomainShader?.Dispose();
            DomainShader = null;
        }
        public bool Render(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, float tessellationAmount)
        {
            // Set the shader parameters that it will use for rendering.
            if (!SetShaderParameters(deviceContext, worldMatrix, viewMatrix, projectionMatrix, tessellationAmount))
                return false; // , tessellationAmount

            // Now render the prepared buffers with the shader.
            RenderShader(deviceContext, indexCount);

            return true;
        }
        private bool SetShaderParameters(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, float tessellationAmount) 
        {
            try
            {
                // Transpose the matrices to prepare them for shader.
                worldMatrix.Transpose();
                viewMatrix.Transpose();
                projectionMatrix.Transpose();

                // Lock the constant buffer so it can be written to.
                DataStream mappedResource;
                deviceContext.MapSubresource(ConstantMatrixBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);

                // Copy the matrices into the constant buffer.
                DMatrixBuffer matrixBuffer = new DMatrixBuffer() 
                {
                    world = worldMatrix,
                    view = viewMatrix,
                    projection = projectionMatrix
                };
                mappedResource.Write(matrixBuffer);

                // Unlock the constant buffer.
                deviceContext.UnmapSubresource(ConstantMatrixBuffer, 0);

                // Set the position of the matrix constant buffer in the domain shader.
                int bufferSlotNuber = 0;

                // Finally set the constant buffer in the vertex shader with the updated values.
                deviceContext.DomainShader.SetConstantBuffer(bufferSlotNuber, ConstantMatrixBuffer);

                // Lock the tessellation constant buffer so it can be written to.
                deviceContext.MapSubresource(ConstantTessellationBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);

                // Copy the tessellation data into the constant buffer.
                DTessellationBufferType tessellationBuffer = new DTessellationBufferType()
                {
                    tessellationAmount = tessellationAmount,
                    padding = new Vector3()
                };
                mappedResource.Write(tessellationBuffer);

                // Unlock the tessellation constant buffer.
                deviceContext.UnmapSubresource(ConstantTessellationBuffer, 0);

                // Set the position of the tessellation constant buffer in the hull shader.
                bufferSlotNuber = 0;

                // Now set the tessellation constant buffer in the hull shader with the updated values.
                deviceContext.HullShader.SetConstantBuffer(bufferSlotNuber, ConstantTessellationBuffer);

                return true;
            }
            catch
            {
                return false;
            }
        }
        private void RenderShader(DeviceContext deviceContext, int indexCount)
        {
            // Set the vertex input layout.
            deviceContext.InputAssembler.InputLayout = Layout;

            // Set the vertex, hull, domain, and pixel shaders that will be used to render this triangle.
            deviceContext.VertexShader.Set(VertexShader);
            deviceContext.HullShader.Set(HullShader);
            deviceContext.DomainShader.Set(DomainShader);
            deviceContext.PixelShader.Set(PixelShader);

            // Render the triangle.
            deviceContext.DrawIndexed(indexCount, 0, 0);
        }
    }
}