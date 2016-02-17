using DSharpDXRastertek.Tut33.System;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut33.Graphics.Shaders
{
    public class DFireShader                    // 248 lines
    {
        // Structs
        [StructLayout(LayoutKind.Sequential)]
        internal struct DMatrixBuffer
        {
            public Matrix world;
            public Matrix view;
            public Matrix projection;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct DNoiseBuffer
        {
            public float frameTime;
            public Vector3 scrollSpeeds;
            public Vector3 scales;
            public float padding;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct DDistortionBuffer
        {
            public Vector2 distprtion1;
            public Vector2 distprtion2;
            public Vector2 distprtion3;
            public float distortionScale;
            public float distortionBias;
        }

        // Properties
        public VertexShader VertexShader { get; set; }
        public PixelShader PixelShader { get; set; }
        public InputLayout Layout { get; set; }
        public SharpDX.Direct3D11.Buffer ConstantMatrixBuffer { get; set; }
        public SharpDX.Direct3D11.Buffer ConstantNoiseBuffer { get; set; }
        public SharpDX.Direct3D11.Buffer ConstantDistortionBuffer { get; set; }
        public SamplerState SamplerState1Wrap { get; set; }
        public SamplerState SamplerState2Clamp { get; set; }

        // Constructor
        public DFireShader() { }

        // Methods
        public bool Initialize(Device device, IntPtr windowsHandler)
        {
            // Initialize the vertex and pixel shaders.
            return InitializeShader(device, windowsHandler, "fire.vs", "fire.ps");
        }
        private bool InitializeShader(Device device, IntPtr windowsHandler, string vsFileName, string psFileName)
        {
            try
            {
                // Setup full pathes
                vsFileName = DSystemConfiguration.ShaderFilePath + vsFileName;
                psFileName = DSystemConfiguration.ShaderFilePath + psFileName;

                // Compile the Vertex Shader & Pixel Shader code.
                ShaderBytecode vertexShaderByteCode = ShaderBytecode.CompileFromFile(vsFileName, "FireVertexShader", DSystemConfiguration.VertexShaderProfile, ShaderFlags.None, EffectFlags.None);
                ShaderBytecode pixelShaderByteCode = ShaderBytecode.CompileFromFile(psFileName, "FirePixelShader", DSystemConfiguration.PixelShaderProfile, ShaderFlags.None, EffectFlags.None);

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
                    }
                };

                // Create the vertex input the layout. Kin dof like a Vertex Declaration.
                Layout = new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), inputElements);

                // Release the vertex and pixel shader buffers, since they are no longer needed.
                vertexShaderByteCode.Dispose();
                pixelShaderByteCode.Dispose();

                // Setup the description of the dynamic matrix constant Matrix buffer that is in the vertex shader.
                BufferDescription matrixBufferDescription = new BufferDescription() 
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<DMatrixBuffer>(),
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                };

                // Create the constant buffer pointer so we can access the vertex shader constant buffer from within this class.
                ConstantMatrixBuffer = new SharpDX.Direct3D11.Buffer(device, matrixBufferDescription);


                // Setup the description of the dynamic noise constant buffer that is in the vertex shader.
                BufferDescription noiseBufferDescription = new BufferDescription()
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<DNoiseBuffer>(),
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                };

                // Create the noise buffer pointer so we can access the vertex shader constant buffer from within this class.
                ConstantNoiseBuffer = new SharpDX.Direct3D11.Buffer(device, noiseBufferDescription);

                // Setup the description of the dynamic distortion constant buffer that is in the pixel shader.
                BufferDescription distortionBufferDescription = new BufferDescription()
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<DNoiseBuffer>(),
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                };

                // Create the distortion buffer pointer so we can access the pixel shader constant buffer from within this class.
                ConstantDistortionBuffer = new SharpDX.Direct3D11.Buffer(device, distortionBufferDescription);

                // Create a texture sampler state description in WRAP mode.
                SamplerStateDescription samplerWRAPDesc = new SamplerStateDescription() 
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
                SamplerState1Wrap = new SamplerState(device, samplerWRAPDesc);

                // Create a second texture sampler state description for a Clamp sampler.
                SamplerStateDescription samplerCLAMPDesc = new SamplerStateDescription() 
                {
                    Filter = Filter.MinMagMipLinear,
                    AddressU = TextureAddressMode.Clamp,
                    AddressV = TextureAddressMode.Clamp,
                    AddressW = TextureAddressMode.Clamp,
                    MipLodBias = 0,
                    MaximumAnisotropy = 1,
                    ComparisonFunction = Comparison.Always,
                    BorderColor = new Color4(0, 0, 0, 0),  // Black Border.
                    MinimumLod = 0,
                    MaximumLod = float.MaxValue
                };

                // Create the second CLAMP based texture sampler state.
                SamplerState2Clamp = new SamplerState(device, samplerCLAMPDesc);

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
            // Release the first WRAP sampler state.
            SamplerState1Wrap?.Dispose();
            SamplerState1Wrap = null;
            // Release the Second CLAMP sampler state.
            SamplerState2Clamp?.Dispose();
            SamplerState2Clamp = null;
            // Release the matrix constant buffer.
            ConstantMatrixBuffer?.Dispose();
            ConstantMatrixBuffer = null;
            // Release the distortion constant buffer.
            ConstantDistortionBuffer?.Dispose();
            ConstantDistortionBuffer = null;
            // Release the noise constant buffer.
            ConstantNoiseBuffer?.Dispose();
            ConstantNoiseBuffer = null;
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
        public bool Render(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, ShaderResourceView fireTexture, ShaderResourceView noiseTexture, ShaderResourceView alphaTexture, float frameTime, Vector3 scrollSpeeds, Vector3 scales, Vector2 distortion1, Vector2 distortion2, Vector2 distortion3,  float distortionScale, float distortionBias)
        {
            // Set the shader parameters that it will use for rendering.
            if (!SetShaderParameters(deviceContext, worldMatrix, viewMatrix, projectionMatrix, fireTexture, noiseTexture, alphaTexture, frameTime, scrollSpeeds, scales, distortion1, distortion2, distortion3, distortionScale, distortionBias))
                return false;

            // Now render the prepared buffers with the shader.
            RenderShader(deviceContext, indexCount);

            return true;
        }
        private bool SetShaderParameters(DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, ShaderResourceView fireTexture, ShaderResourceView noiseTexture, ShaderResourceView alphaTexture, float frameTime, Vector3 scrollSpeeds, Vector3 scales, Vector2 distortion1, Vector2 distortion2, Vector2 distortion3, float distortionScale, float distortionBias)
        {
            try
            {
                #region Set Matrix Shader Resources
                // Transpose the matrices to prepare them for shader.
                worldMatrix.Transpose();
                viewMatrix.Transpose();
                projectionMatrix.Transpose();

                // Lock the constant buffer so it can be written to.
                DataStream mappedResource;
                deviceContext.MapSubresource(ConstantMatrixBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);

                // Copy the passed in matrices into the constant buffer.
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
                int bufferPositionNumber = 0;

                // Finally set the constant buffer in the vertex shader with the updated values.
                deviceContext.VertexShader.SetConstantBuffer(bufferPositionNumber, ConstantMatrixBuffer);
                #endregion

                #region Set Noise Constant Buffer Shader Resources
                // Lock the constant buffer so it can be written to.
                deviceContext.MapSubresource(ConstantNoiseBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);

                // Copy the passed in data, frame time, speeds, and scaes into the Noise constant buffer.
                DNoiseBuffer noiseBuffer = new DNoiseBuffer()
                {
                    frameTime = frameTime,
                     scrollSpeeds = scrollSpeeds,
                     scales = scales,
                     padding = 0.0f
                };
                mappedResource.Write(noiseBuffer);

                // Unlock the noise constant buffer.
                deviceContext.UnmapSubresource(ConstantNoiseBuffer, 0);

                // Set the position of the noise constant buffer in the vertex shader.
                bufferPositionNumber = 1;

                // Now set the noise constant buffer in the vertex shader with the updated values.
                deviceContext.VertexShader.SetConstantBuffer(bufferPositionNumber, ConstantNoiseBuffer);
                #endregion

                #region Set the 3 shader texture resources in the pixel shader.
                // Set the three shader texture resources in the pixel shader.
                deviceContext.PixelShader.SetShaderResources(0, fireTexture, noiseTexture, alphaTexture);
                #endregion

                #region Set Distortion Constant Buffer Shader Resources
                // Lock the distortion constant buffer so it can be written to.
                deviceContext.MapSubresource(ConstantDistortionBuffer, MapMode.WriteDiscard, MapFlags.None, out mappedResource);

                // Copy the passed in data, distortion 1 2 & 3 distortionScale & distortionBias into the Noise constant buffer.
                DDistortionBuffer distortionBuffer = new DDistortionBuffer()
                {
                    distprtion1 = distortion1,
                    distprtion2 = distortion2,
                    distprtion3 = distortion3,
                    distortionScale = distortionScale,
                    distortionBias = distortionBias
                };
                mappedResource.Write(distortionBuffer);

                // Unlock the distortion constant buffer.
                deviceContext.UnmapSubresource(ConstantDistortionBuffer, 0);

                // Set the position of the distortion constant buffer in the pixel shader.
                bufferPositionNumber = 0;

                // Now set the distortion constant buffer in the pixel shader with the updated values.
                deviceContext.PixelShader.SetConstantBuffer(bufferPositionNumber, ConstantDistortionBuffer);
                #endregion

                return true;
            }
            catch (Exception)
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
            deviceContext.PixelShader.SetSampler(0, SamplerState1Wrap);
            deviceContext.PixelShader.SetSampler(1, SamplerState2Clamp);

            // Render the triangle.
            deviceContext.DrawIndexed(indexCount, 0, 0);
        }
    }
}