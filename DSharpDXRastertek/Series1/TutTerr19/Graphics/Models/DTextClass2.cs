using DSharpDXRastertek.TutTerr19.Graphics.Data;
using DSharpDXRastertek.TutTerr19.Graphics.Shaders;
using DSharpDXRastertek.TutTerr19.System;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DSharpDXRastertek.TutTerr19.Graphics.Models
{
    public class DText                  // 237 lines
    {
        // Structs
        [StructLayout(LayoutKind.Sequential)]
        public struct DSentence
        {
            public SharpDX.Direct3D11.Buffer VertexBuffer;
            public SharpDX.Direct3D11.Buffer IndexBuffer;
            public int VertexCount;
            public int IndexCount;
            public int MaxLength;
            public float red;
            public float green;
            public float blue;
            public string sentenceText;
            public int yPos;
        }

        // Variables
        const int NUM_SENTENCES = 12;

        // Properties
        public int ScreenWidth;
        public int ScreenHeight;
        public DSentence[] sentences = new DSentence[NUM_SENTENCES];

        public bool Shadowed { get; set; }
        public int MaxLength { get; set; }
        public int VertexCount { get; set; }
        public int IndexCount { get; set; }
        public Vector4 PixelColour { get; set; }
        public SharpDX.Direct3D11.Buffer VertexBuffer { get; set; }
        public SharpDX.Direct3D11.Buffer VertexBuffer2 { get; set; }
        public SharpDX.Direct3D11.Buffer IndexBuffer { get; set; }
        public SharpDX.Direct3D11.Buffer IndexBuffer2 { get; set; }
        
        // Methods
        public bool Initialize(SharpDX.Direct3D11.Device device, DFont font, DSystemConfiguration configuration, int maxLength, string text,int positionX, int positionY, float red, float green, float blue, bool shadowed, SharpDX.Direct3D11.DeviceContext deviceContext)
        {
            // Store if this sentence is shadowed or not.
            Shadowed = shadowed;

            // Store the screen width and height.
            ScreenWidth = configuration.Width;
            ScreenHeight = configuration.Height;

            // Store the maximum length of the sentence.
            MaxLength = maxLength;

            // Initalize the sentence.
            if (!InitializeSentence(device, font, text, positionX, positionY, red, green, blue, deviceContext))
                return false;

            return true;
        }
        public void Shutdown()
        {
            // Release all sentances however many there may be.
            foreach (DSentence sentance in sentences)
                ReleaseSentences(sentance);
            sentences = null;

            // Release the DText vertex buffer.
            VertexBuffer?.Dispose();
            VertexBuffer = null;
            VertexBuffer2?.Dispose();
            VertexBuffer2 = null;
            // Release the DText index buffer.
            IndexBuffer?.Dispose();
            IndexBuffer = null;
            VertexBuffer2?.Dispose();
            VertexBuffer2 = null;
        }
        public bool Render(DeviceContext deviceContext, DShaderManager shaderManager, Matrix worldMatrix, Matrix viewMatrix, Matrix orthoMatrix, ShaderResourceView fontTexture)
        {
            // Draw the sentence.
            if (!RenderSentence(deviceContext, shaderManager, worldMatrix, viewMatrix, orthoMatrix, fontTexture))
                return false;

            return true;
        }
        private bool InitializeSentence(SharpDX.Direct3D11.Device device, DFont font, string text, int positionX, int positionY, float red, float green, float blue, SharpDX.Direct3D11.DeviceContext deviceContext)
        {
            // Set the vertex and index count.
            VertexCount = 6 * MaxLength;
            IndexCount = 6 * MaxLength;

            // Create the vertex array.
            DFont.DVertexType[] vertices = new DFont.DVertexType[VertexCount];
            // Create the index array.
            int[] indices = new int[IndexCount];

            //// Initialize vertex array to zeros at first.
            for(int i = 0; i < vertices.Length; i++)
            {
                vertices[i].position = Vector3.Zero;
                vertices[i].texture = Vector2.Zero;
            }

            // Initialize the index array.
            for (var i = 0; i < IndexCount; i++)
                indices[i] = i;

            // Set up the description of the dynamic vertex buffer.
            var vertexBufferDesc = new BufferDescription()
            {
                Usage = ResourceUsage.Dynamic,
                SizeInBytes = Utilities.SizeOf<DFont.DVertexType>() * VertexCount,
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0
            };

            // Create the vertex buffer.
            VertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, vertices, vertexBufferDesc);

            // Create the index buffer.
            IndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indices);

            // If shadowed create the second vertex and index buffer.
            if (Shadowed)
            {
                try
                {
                    VertexBuffer2 = SharpDX.Direct3D11.Buffer.Create(device, vertices, vertexBufferDesc);
                    IndexBuffer2 = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indices);
                }
                catch
                {
                    return false;
                }
            }

            // Release the vertex array as it is no longer needed.
            // Release the index array as it is no longer needed.
            vertices = null;
            indices = null;

            // Now add the text data to the sentence buffers.
            if (!UpdateSentence2(font, text, positionX, positionY, red, green, blue, deviceContext))
                return false;

            return true;
        }
        public bool UpdateSentence2(DFont font, string text, int positionX, int positionY, float red, float green, float blue, DeviceContext deviceContext)
        {
            // Store the color of the sentence.
            PixelColour = new Vector4(red, green, blue, 1.0f);

            // Get the number of letters in the sentence.
            var numLetters = text.Length;

            // Check for possible buffer overflow.
            if (numLetters > MaxLength)
                return false;

            // Calculate the X and Y pixel position on screen to start drawing to.
            var drawX = -(ScreenWidth >> 1) + positionX;
            var drawY = (ScreenHeight >> 1) - positionY;

            // Use the font class to build the vertex array from the sentence text and sentence draw location.
            List<DFont.DVertexType> vertices;
            font.BuildVertexArray(out vertices, text, drawX, drawY);

            // Empty all remaining vertices
            for (int i = text.Length; i < MaxLength; i++)
            {
                var emptyVertex = new DFont.DVertexType();
                emptyVertex.position = Vector3.Zero;
                emptyVertex.texture = Vector2.Zero;
                vertices.Add(emptyVertex);
            }

            DataStream mappedResource;

            #region Vertex Buffer
            // Lock the vertex buffer so it can be written to.
            deviceContext.MapSubresource(VertexBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out mappedResource);

            // Copy the data into the vertex buffer.
            mappedResource.WriteRange<DFont.DVertexType>(vertices.ToArray());

            // Unlock the vertex buffer.
            deviceContext.UnmapSubresource(VertexBuffer, 0);
            #endregion

            // If shadowed then do the same for the second vertex buffer but offset by one pixel.
            if (Shadowed)
            {
                // Perform same mapping and writings code as above except for the shadows offset position for the effect.
            }

            vertices?.Clear();
            vertices = null;

            return true;
        }
        private void ReleaseSentences(DSentence sentence)
        {
            // Release the sentence vertex buffer.
            sentence.VertexBuffer?.Dispose();
            sentence.VertexBuffer = null;
            // Release the sentence index buffer.
            sentence.IndexBuffer?.Dispose();
            sentence.IndexBuffer = null;
        }
        private bool RenderSentence(DeviceContext deviceContext, DShaderManager shaderManager, Matrix worldMatrix, Matrix viewMatrix, Matrix orthoMatrix, ShaderResourceView fontTexture)
        {
            // Set vertex buffer stride and offset.
            var stride = Utilities.SizeOf<DFont.DVertexType>();
            var offset = 0;

            // If shadowed then render the shadow text first.
            if (Shadowed)
            {
                Vector4 shadowColour = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
                deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer2, stride, offset));
                deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer2, Format.R32_UInt, 0);
                deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                shaderManager.RenderFontShader(deviceContext, IndexCount, worldMatrix, viewMatrix, orthoMatrix, fontTexture, shadowColour);
            }

            // Set the vertex buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, stride, offset));
            // Set the index buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            shaderManager.RenderFontShader(deviceContext, IndexCount, worldMatrix, viewMatrix, orthoMatrix, fontTexture, PixelColour);

            return true;
        }
    }
}