using DSharpDXRastertek.Tut30.System;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut30.Graphics.Shaders
{
    public class DLightShader                   // 342 lines
    {
        // Structs
        [StructLayout(LayoutKind.Sequential)]
        public struct DVertex
        {
            public Vector3 position;
            public Vector2 texture;
            public Vector3 normal;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct DMatrixBuffer
        {
            public Matrix world;
            public Matrix view;
            public Matrix projection;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct DLightColorBuffer
        {
            public Vector4 diffuseColor1;
            public Vector4 diffuseColor2;
            public Vector4 diffuseColor3;
            public Vector4 diffuseColor4;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct DLightPositionBuffer
        {
            public Vector4 lightPosition1;
            public Vector4 lightPosition2;
            public Vector4 lightPosition3;
            public Vector4 lightPosition4;
        }

        // Variables
        private DLightColorBuffer lightColorBuffer;
        private DLightPositionBuffer lightPositionBuffer;

        // Properties
        public VertexShader VertexShader { get; set; }
        public PixelShader PixelShader { get; set; }
        public InputLayout Layout { get; set; }
        public SharpDX.Direct3D11.Buffer ConstantMatrixBuffer { get; set; }
        public SharpDX.Direct3D11.Buffer ConstantLightDiffuseColorBuffer { get; set; }
        public SharpDX.Direct3D11.Buffer ConstantLightPositionBuffer { get; set; }
        public SamplerState SamplerState { get; set; }    
        public int NumLights { get; set; }

        // Constructor
        public DLightShader() 
        {
            NumLights = 4;

            lightColorBuffer = new DLightColorBuffer();
            lightPositionBuffer = new DLightPositionBuffer();
        }

        // Methods
        public bool Initialize(Device device, IntPtr windowsHandler)
        {
            // Initialize the vertex and pixel shaders.
            return InitializeShader(device, windowsHandler, "light.vs", "light.ps");
        }
        private bool InitializeShader(Device device, IntPtr windowsHandler, string vsFileName, string psFileName)
        {
            try
            {
                // Setup full pathes
                vsFileName = DSystemConfiguration.ShaderFilePath + vsFileName;
                psFileName = DSystemConfiguration.ShaderFilePath + psFileName;

                // Compile the Vertex Shader & Pixel Shader code.
                ShaderBytecode vertexShaderByteCode = ShaderBytecode.CompileFromFile(vsFileName, "LightVertexShader", DSystemConfiguration.VertexShaderProfile, ShaderFlags.None, EffectFlags.None);
                ShaderBytecode pixelShaderByteCode = ShaderBytecode.CompileFromFile(psFileName, "LightPixelShader", DSystemConfiguration.PixelShaderProfile, ShaderFlags.None, EffectFlags.None);

                // Create the Vertex & Pixel Shaders from the buffer.
                VertexShader = new VertexShader(device, vertexShaderByteCode);
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
                        AlignedByteOffset = InputElement.AppendAligned,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElement()
                    {
                        SemanticName = "TEXCOORD",
                        SemanticIndex = 0,
                        Format = SharpDX.DXGI.Format.R32G32_Float,
                        Slot = 0,
                        AlignedByteOffset = InputElement.AppendAligned,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    },
                    new InputElement()
                    {
                        SemanticName = "NORMAL",
                        SemanticIndex = 0,
                        Format = SharpDX.DXGI.Format.R32G32B32_Float,
                        Slot = 0,
                        AlignedByteOffset = InputElement.AppendAligned,
                        Classification = InputClassification.PerVertexData,
                        InstanceDataStepRate = 0
                    }
                };

                // Create the vertex input the layout. Kin dof like a Vertex Declaration.
                Layout = new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), inputElements);

                // Release the vertex and pixel shader buffers, since they are no longer needed.
                vertexShaderByteCode.Dispose();
                pixelShaderByteCode.Dispose();

                // Create a texture sampler state description.
                SamplerStateDescription samplerDesc = new SamplerStateDescription()
                {
                    Filter = Filter.MinMagMipLinear,
                    AddressU = TextureAddressMode.Wrap,
                    AddressV = TextureAddressMode.Wrap,
                    AddressW = TextureAddressMode.Wrap,
                    MipLodBias = 0,
                    MaximumAnisotropy = 1,
                    ComparisonFunction = Comparison.Always,
                    BorderColor = new Color4(0, 0, 0, 0),  // Black Border.
                    MinimumLod = 0,
                    MaximumLod = float.MaxValue
                };

                // Create the texture sampler state.
                SamplerState = new SamplerState(device, samplerDesc);

                // Setup the description of the dynamic matrix constant Matrix buffer that is in the vertex shader.
                BufferDescription matrixBufferDescription = new BufferDescription()
                {
                    Usage = ResourceUsage.Dynamic, // ResourceUsage.Default
                    SizeInBytes = Utilities.SizeOf<DMatrixBuffer>(),
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write, // CpuAccessFlags.None
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                };

                // Create the constant buffer pointer so we can access the vertex shader constant buffer from within this class.
                ConstantMatrixBuffer = new SharpDX.Direct3D11.Buffer(device, matrixBufferDescription);

                // Setup the description of the dynamic matrix constant Matrix buffer that is in the vertex shader.
                BufferDescription LightDiffuseBufferDescription = new BufferDescription()
                {
                    Usage = ResourceUsage.Dynamic, // ResourceUsage.Default
                    SizeInBytes = Utilities.SizeOf <DLightColorBuffer>(),
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write, // CpuAccessFlags.None
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                };


                ConstantLightDiffuseColorBuffer = new SharpDX.Direct3D11.Buffer(device, LightDiffuseBufferDescription);

                // Setup the description of the dynamic matrix constant Matrix buffer that is in the vertex shader.
                BufferDescription LightPositionBufferDescription = new BufferDescription()
                {
                    Usage = ResourceUsage.Dynamic, // ResourceUsage.Default
                    SizeInBytes = Utilities.SizeOf<DLightPositionBuffer>(),
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write, // CpuAccessFlags.None
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                };

                ConstantLightPositionBuffer = new SharpDX.Direct3D11.Buffer(device, LightPositionBufferDescription);

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
            // Release the light constant buffer.
            ConstantLightDiffuseColorBuffer?.Dispose();
            ConstantLightDiffuseColorBuffer = null;
            // Release the matrix constant buffer.
            ConstantLightPositionBuffer?.Dispose();
            ConstantLightPositionBuffer = null;
            // Release the matrix constant buffer.
            ConstantMatrixBuffer?.Dispose();
            ConstantMatrixBuffer = null;
            // Release the sampler state.
            SamplerState?.Dispose();
            SamplerState = null;
            // Release the layout.
            Layout?.Dispose();
            Layout = null;
            // Release the pixel shader.
            PixelShader?.Dispose();
            PixelShader = null;
            // Release the vertex shader.
            VertexShader?.Dispose();
            VertexShader = null;
        }
        public bool Render(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, ShaderResourceView texture, Vector4[] lightColors, Vector4[] lightPositions)
        {
            // Set the shader parameters that it will use for rendering.
            if (!SetShaderParameters(deviceContext, worldMatrix, viewMatrix, projectionMatrix, texture, lightColors, lightPositions))
                return false;

            // Now render the prepared buffers with the shader.
            RenderShader(deviceContext, indexCount);

            return true;
        }
        private bool SetShaderParameters(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, ShaderResourceView texture, Vector4[] lightColors, Vector4[] lightPositions)
        {
            try
            {
                DataStream mappedResource;
                int bufferPositionNumber;

                #region Constant Matrix Buffer
                // Transpose the matrices to prepare them for shader.
                worldMatrix.Transpose();
                viewMatrix.Transpose();
                projectionMatrix.Transpose();

                // Lock the constant buffer so it can be written to.
                deviceContext.MapSubresource(ConstantMatrixBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);

                //// Copy the passed in matrices into the constant buffer.
                DMatrixBuffer matrixBuffer = new DMatrixBuffer()
                {
                    world = worldMatrix,
                    view = viewMatrix,
                    projection = projectionMatrix
                };
                mappedResource.Write(matrixBuffer);

                // Unlock the constant buffer.
                deviceContext.UnmapSubresource(ConstantMatrixBuffer, 0);

                // Set the position of the constant buffer in the vertex shader.
                bufferPositionNumber = 0;

                // Finally set the constant buffer in the vertex shader with the updated values.
                deviceContext.VertexShader.SetConstantBuffer(bufferPositionNumber, ConstantMatrixBuffer);

                // Set shader resource in the pixel shader.
                deviceContext.PixelShader.SetShaderResource(0, texture);
                #endregion

                #region Light Diffuse Colour Buffer
                // Lock the camera constant buffer so it can be written to.
                deviceContext.MapSubresource(ConstantLightDiffuseColorBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out mappedResource);

                lightColorBuffer.diffuseColor1 = lightColors[0];
                lightColorBuffer.diffuseColor2 = lightColors[1];
                lightColorBuffer.diffuseColor3 = lightColors[2];
                lightColorBuffer.diffuseColor4 = lightColors[3];
                mappedResource.Write(lightColorBuffer);

                // Unlock the constant buffer.
                deviceContext.UnmapSubresource(ConstantLightDiffuseColorBuffer, 0);

                // Set the position of the Light Diffuse Color constant buffer in the vertex shader.
                bufferPositionNumber = 0;

                // Finally set the light constant buffer in the pixel shader with the updated values.
                deviceContext.PixelShader.SetConstantBuffer(bufferPositionNumber, ConstantLightDiffuseColorBuffer);
                #endregion

                #region Light Position Buffer
                // Now for the Lighting Position Shader.
                // Lock the light constant buffer so it can be written to.
                deviceContext.MapSubresource(ConstantLightPositionBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);

                lightPositionBuffer.lightPosition1 = lightPositions[0];
                lightPositionBuffer.lightPosition2 = lightPositions[1];
                lightPositionBuffer.lightPosition3 = lightPositions[2];
                lightPositionBuffer.lightPosition4 = lightPositions[3];
                mappedResource.Write(lightPositionBuffer);

                // Unlock the constant buffer.
                deviceContext.UnmapSubresource(ConstantLightPositionBuffer, 0);

               /// Set the position of the light constant buffer in the pixel shader.
                bufferPositionNumber = 1;

                // Now set the camera constant buffer in the vertex shader with the updated values.
                deviceContext.VertexShader.SetConstantBuffer(bufferPositionNumber, ConstantLightPositionBuffer);
                #endregion

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

            // Set the vertex and pixel shaders that will be used to render this triangle.
            deviceContext.VertexShader.Set(VertexShader);
            deviceContext.PixelShader.Set(PixelShader);

            // Set the sampler state in the pixel shader.
            deviceContext.PixelShader.SetSampler(0, SamplerState);

            // Render the triangle.
            deviceContext.DrawIndexed(indexCount, 0, 0);
        }
    }
}