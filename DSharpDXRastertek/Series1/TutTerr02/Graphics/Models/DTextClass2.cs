using DSharpDXRastertek.TutTerr02.Graphics.Data;
using DSharpDXRastertek.TutTerr02.Graphics.Shaders;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DSharpDXRastertek.TutTerr02.Graphics.Models
{
    public class DText                  // 347 lines
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
        }

        // Variables
        const int NUM_SENTENCES = 7;

        // Properties
        public DFont Font;
        public int ScreenWidth;
        public int ScreenHeight;
        public Matrix BaseViewMatrix;
        public DSentence[] sentences = new DSentence[NUM_SENTENCES];
        
        // Methods
        public bool Initialize(SharpDX.Direct3D11.Device device, DeviceContext deviceContext, IntPtr windowHandle, int screanWidth, int screenHeight, Matrix baseViewMatrix)
        {
            // Store the screen width and height.
            ScreenWidth = screanWidth;
            ScreenHeight = screenHeight;

            // Store the base view matrix.
            BaseViewMatrix = baseViewMatrix;

            // Create the font object.
            Font = new DFont();

            // Initialize the font object.
            if (!Font.Initialize(device, "fontdata.txt", "font.dds"))
                return false;

            // Initialize the first sentence.
            if (!InitializeSentence(out sentences[0], 150, device))
                return false;

            // Initialize the second sentence.
            if (!InitializeSentence(out sentences[1], 32, device))
                return false;

            // Initialize the third sentence.
            if (!InitializeSentence(out sentences[2], 150, device))
                return false;

            // Initialize the fourth sentence.
            if (!InitializeSentence(out sentences[3], 150, device))
                return false;

            // Initialize the Fifth sentence.
            if (!InitializeSentence(out sentences[4], 150, device))
                return false;

            // Initialize the Sixth sentence.
            if (!InitializeSentence(out sentences[5], 150, device))
                return false;

            // Initialize the Sixth sentence.
            if (!InitializeSentence(out sentences[6], 190, device))
                return false;

            return true;
        } 
        public void Shutdown()
        {
            // Release all sentances however many there may be.
            foreach (DSentence sentance in sentences)
                ReleaseSentences(sentance);

            // Release the font object.
            if (Font != null)
            {
                Font.Shutdown();
                Font = null;
            }
        }
        public bool Render(DeviceContext deviceContext, DFontShader fontShader, Matrix worldMatrix, Matrix orthoMatrix)
        {
            // Render all Sentances however many there mat be.
            foreach (DSentence sentance in sentences)
            {
                if (!RenderSentence(deviceContext, fontShader, sentance, worldMatrix, orthoMatrix))
                    return false;
            }

            return true;
        }
        public bool SetVideoCard(string videoCard, int videoMemory, DeviceContext deviceContext)
        {
            string formattedCpuUsage = string.Format("VideoCard: {0} Video Memory: {1}", videoCard, videoMemory);

            // Update the sentence vertex buffer with the new string information.
            if (!UpdateSentece(ref sentences[0], formattedCpuUsage, 20, 20, 0, 1, 0, deviceContext))
                return false;

            return true;
        }
        public bool SetFps(int fps, DeviceContext deviceContext)
        {
            // Truncate the FPS to below 10,000
            if (fps > 9999)
                fps = 9999;

            // Convert the FPS integer to string format.
            string fpsString = string.Format("FPS: {0:d4}", fps);

            // Setup Colour variables with a default to white,  for assigning colour based on performance.
            float red = 1, green = 1, blue = 1;

            // If fps is 60 or above set the fps color to green.
            if (fps >= 60)
            {
                red = 0;
                green = 1;
                blue = 0;
            }
            // If fps is below 60 set the fps color to yellow
            if (fps < 60)
            {
                red = 1;
                green = 1;
                blue = 0;
            }
            // If fps is below 30 set the fps to red.
            if (fps < 30)
            {
                red = 1;
                green = 0;
                blue = 0;
            }

            // Update the sentence vertex buffer with the new string information.
            if (!UpdateSentece(ref sentences[1], fpsString, 20, 40, red, green, blue, deviceContext))
                return false;

            return true;
        }
        public bool SetCpu(int cpu, DeviceContext deviceContext)
        {
            // Format string for this sentance to report CPU Usage percetange
            string formattedCpuUsage = string.Format("CPU: {0}%", cpu);

            // Update the sentence vertex buffer with the new string information.
            if (!UpdateSentece(ref sentences[2], formattedCpuUsage, 20, 60, 0, 1, 0, deviceContext))
                return false;

            return true;
        }
        public bool SetCameraPosition(float posX, float posY, float posZ, DeviceContext deviceContext)
        {
            // Convert the position from floating point to integer.
            int positionX = (int)posX;
            int positionY = (int)posY;
            int positionZ = (int)posZ;

            // Truncate the position if it exceeds either 9999 or -9999.
            if (positionX > 9999) 
                positionX = 9999;
            if (positionY > 9999) 
                positionY = 9999;
            if (positionZ > 9999) 
                positionZ = 9999;

            if (positionX < -9999)
                positionX = -9999;
            if (positionY < -9999) 
                positionY = -9999;
            if (positionZ < -9999) 
                positionZ = -9999;

            // Setup the X position string.
            string CamerXPosition = String.Format("X: {0}", positionX);
            // Update the sentence vertex buffer with the new string information.
            if (!UpdateSentece(ref sentences[3], CamerXPosition, 20, 120, 0, 1, 0, deviceContext))
                return false;

            string CamerYPosition = String.Format("Y: {0}", positionY);
            // Update the sentence vertex buffer with the new string information.
            if (!UpdateSentece(ref sentences[4], CamerYPosition, 20, 140, 0, 1, 0, deviceContext))
                return false;

            string CamerZPosition = String.Format("Z: {0}", positionZ);
            // Update the sentence vertex buffer with the new string information.
            if (!UpdateSentece(ref sentences[5], CamerZPosition, 20, 160, 0, 1, 0, deviceContext))
                return false;

            return true;
        }
        public bool SetCameraRotation(float rotX, float rotY, float rotZ, DeviceContext deviceContext)
        {
            // Convert the rotation from floating point to integer.
            int rotationX = (int)rotX;
            int rotationY = (int)rotY;
            int rotationZ = (int)rotZ;

            // Setup the X Rotation string.
            string CamerXRotation = String.Format("rX: {0}", rotationX);

            if (!UpdateSentece(ref sentences[6], CamerXRotation, 20, 190, 0, 1, 0, deviceContext))
                return false;

            return true;
        }

        private bool InitializeSentence(out DSentence sentence, int maxLength, SharpDX.Direct3D11.Device device)
        {
            // Create a new sentence object.
            sentence = new DSentence();

            // Initialize the sentence buffers to null;
            sentence.VertexBuffer = null;
            sentence.IndexBuffer = null;

            // Set the maximum length of the sentence.
            sentence.MaxLength = maxLength;

            // Set the number of vertices in vertex array.
            sentence.VertexCount = 6 * maxLength;
            // Set the number of vertices in the vertex array.
            sentence.IndexCount = sentence.VertexCount;

            // Create the vertex array.
            var vertices = new DFont.DVertexType[sentence.VertexCount];
            // Create the index array.
            var indices = new int[sentence.IndexCount];

            // Initialize the index array.
            for (var i = 0; i < sentence.IndexCount; i++)
                indices[i] = i;

            // Set up the description of the dynamic vertex buffer.
            var vertexBufferDesc = new BufferDescription()
            {
                Usage = ResourceUsage.Dynamic,
                SizeInBytes = Utilities.SizeOf<DFont.DVertexType>() * sentence.VertexCount,
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0
            };

            // Create the vertex buffer.
            sentence.VertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, vertices, vertexBufferDesc);

            // Create the index buffer.
            sentence.IndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indices);

            return true;
        }
        private bool UpdateSentece(ref DSentence sentence, string text, int positionX, int positionY, float red, float green, float blue, DeviceContext deviceContext)
        {
            // Store the Text to update the given sentence.
            sentence.sentenceText = text;

            // Store the color of the sentence.
            sentence.red = red;
            sentence.green = green;
            sentence.blue = blue;

            // Get the number of the letter in the sentence.
            var numLetters = text.Length;

            // Check for possible buffer overflow.
            if (numLetters > sentence.MaxLength)
                return false;

            // Calculate the X and Y pixel position on screen to start drawing to.
            var drawX = -(ScreenWidth >> 1) + positionX;
            var drawY = (ScreenHeight >> 1) - positionY;

            // Use the font class to build the vertex array from the sentence text and sentence draw location.
            List<DFont.DVertexType> vertices;
            Font.BuildVertexArray(out vertices, text, drawX, drawY);

            DataStream mappedResource;

            #region Vertex Buffer 
            // Lock the vertex buffer so it can be written to.
            deviceContext.MapSubresource(sentence.VertexBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out mappedResource);

            // Copy the data into the vertex buffer.
            mappedResource.WriteRange<DFont.DVertexType>(vertices.ToArray());

            // Unlock the vertex buffer.
            deviceContext.UnmapSubresource(sentence.VertexBuffer, 0);
            #endregion

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
        private bool RenderSentence(DeviceContext deviceContext, DFontShader fontShader, DSentence sentence, Matrix worldMatrix, Matrix orthoMatrix)
        {
            // Set vertex buffer stride and offset.
            var stride = Utilities.SizeOf<DFont.DVertexType>();
            var offset = 0;

            // Set the vertex buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(sentence.VertexBuffer, stride, offset));
            
            // Set the index buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetIndexBuffer(sentence.IndexBuffer, Format.R32_UInt, 0);
            
            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            
            // Create a pixel color vector with the input sentence color.
            var pixelColor = new Vector4(sentence.red, sentence.green, sentence.blue, 1);
            
            // Render the text using the font shader.
            if (!fontShader.Render(deviceContext, sentence.IndexCount, worldMatrix, BaseViewMatrix, orthoMatrix, Font.Texture.TextureResource, pixelColor))
                return false;

            return true;
        }
    }
}