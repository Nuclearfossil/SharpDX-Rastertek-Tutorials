using DSharpDXRastertek.TutTerr12.Graphics.Data;
using DSharpDXRastertek.TutTerr12.Graphics.Shaders;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DSharpDXRastertek.TutTerr12.Graphics.Models
{
    public class DText                  // 458 lines
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
        public DFont Font;
        public DFontShader FontShader;
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
            if (!Font.Initialize(device, "fontdata.txt", "font.bmp"))
                return false;

            // Create the font shader object.
            FontShader = new DFontShader();

            // Initialize the font shader object.
            if (!FontShader.Initialize(device, windowHandle))
                return false;

            // Initialize the first sentence.
            if (!InitializeSentence(out sentences[0], 150, 20, device))
                return false;

            // Initialize the second sentence.
            if (!InitializeSentence(out sentences[1], 32, 40, device))
                return false;

            // Initialize the third sentence.
            if (!InitializeSentence(out sentences[2], 16, 70, device))
                return false;

            // Initialize the fourth sentence.
            if (!InitializeSentence(out sentences[3], 16, 90, device))
                return false;

            // Initialize the Fifth sentence.
            if (!InitializeSentence(out sentences[4], 16, 120, device))
                return false;

            // Initialize the Sixth sentence.
            if (!InitializeSentence(out sentences[5], 16, 140, device))
                return false;

            // Initialize the Seventh sentence.
            if (!InitializeSentence(out sentences[6], 16, 160, device))
                return false;

            // Initialize the Eighth sentence.
            if (!InitializeSentence(out sentences[7], 16, 190, device))
                return false;

            // Initialize the Eighth sentence.
            if (!InitializeSentence(out sentences[8], 16, 210, device))
                return false;

            // Initialize the Eighth sentence.
            if (!InitializeSentence(out sentences[9], 16, 230, device))
                return false;

            // Initialize the Eighth sentence.
            if (!InitializeSentence(out sentences[10], 32, 260, device))
                return false;

            // Initialize the Eighth sentence.
            if (!InitializeSentence(out sentences[11], 32, 290, device))
                return false;

            return true;
        }
        public void Shutdown()
        {
            // Release all sentances however many there may be.
            foreach (DSentence sentance in sentences)
                ReleaseSentences(sentance);

            // Release the font shader object.
            FontShader?.Shuddown();
            FontShader = null;
            // Release the font object.
            Font?.Shutdown();
            Font = null;
        }
        public bool Render(DeviceContext deviceContext, Matrix worldMatrix, Matrix orthoMatrix)
        {
            // Render all Sentances however many there mat be.
            foreach (DSentence sentance in sentences)
            {
                if (!RenderSentence(deviceContext, sentance, worldMatrix, orthoMatrix))
                    return false;
            }

            return true;
        }
        public bool SetVideoCard(string videoCard, int videoMemory, DeviceContext deviceContext)
        {
            string formattedVidCardDesc = string.Format("VideoCard: {0}", videoCard);
            string formattedVidCardMem = string.Format("Video Memory: {0}", videoMemory);

            // Update the sentence vertex buffer with the new string information.
            if (!UpdateSentece(ref sentences[0], formattedVidCardDesc, 20, 20, 0, 1, 0, deviceContext))
                return false;

            // Update the sentence vertex buffer with the new string information.
            if (!UpdateSentece(ref sentences[1], formattedVidCardMem, 20, 40, 0, 1, 0, deviceContext))
                return false;

            return true;
        }
        public bool SetFps(int fps, DeviceContext deviceContext)
        {
            // Truncate the FPS to below 10,000
            if (fps > 9999)
                fps = 9999;

            // Convert the FPS integer to string format.
            string fpsString = string.Format("FPS: {0}", fps.ToString().PadLeft(4));

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
            if (!UpdateSentece(ref sentences[2], fpsString, 20, 70, red, green, blue, deviceContext))
                return false;

            return true;
        }
        public bool SetCpu(int cpu, DeviceContext deviceContext)
        {
            // Format string for this sentance to report CPU Usage percetange
            string formattedCpuUsage = string.Format("CPU: {0}%", cpu.ToString().PadLeft(3));

            // Update the sentence vertex buffer with the new string information.
            if (!UpdateSentece(ref sentences[3], formattedCpuUsage, 20, 90, 0, 1, 0, deviceContext))
                return false;

            return true;
        }
        public bool SetCameraPosition(float posX, float posY, float posZ, DeviceContext deviceContext)
        {
            // Convert the position from floating point to integer.
            float positionX = posX;
            float positionY = posY;
            float positionZ = posZ;

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
            string CamerXPosition = String.Format("X: {0}", positionX.ToString("F2").PadLeft(9));

            // Update the sentence vertex buffer with the new string information.
            if (!UpdateSentece(ref sentences[4], CamerXPosition, 20, 120, 0, 1, 0, deviceContext))
                return false;

            string CamerYPosition = String.Format("Y: {0:n1}", positionY.ToString("F2").PadLeft(9));

            // Update the sentence vertex buffer with the new string information.
            if (!UpdateSentece(ref sentences[5], CamerYPosition, 20, 140, 0, 1, 0, deviceContext))
                return false;

            string CamerZPosition = String.Format("Z: {0:n1}", positionZ.ToString("F2").PadLeft(9));

            // Update the sentence vertex buffer with the new string information.
            if (!UpdateSentece(ref sentences[6], CamerZPosition, 20, 160, 0, 1, 0, deviceContext))
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
            string CamerYRotation = String.Format("rY: {0}", rotationY);
            string CamerZRotation = String.Format("rZ: {0}", rotationZ);

            if (!UpdateSentece(ref sentences[7], CamerXRotation, 20, 190, 0, 1, 0, deviceContext))
                return false;
            if (!UpdateSentece(ref sentences[8], CamerYRotation, 20, 210, 0, 1, 0, deviceContext))
                return false;
            if (!UpdateSentece(ref sentences[9], CamerZRotation, 20, 230, 0, 1, 0, deviceContext))
                return false;

            return true;
        }
        public bool SetRenderCount(int renderCount, DeviceContext deviceContext)
        {
            // Format string for this sentance to report CPU Usage percetange
            string formattedRenderCount = string.Format("Render Count: {0}", renderCount);

            // Update the sentence vertex buffer with the new string information.
            if (!UpdateSentence2(ref sentences[10], formattedRenderCount, 20, 260, 0, 1, 0, deviceContext))
                return false;

            return true;
        }
        public bool SetSentenceByIndex(int sentenceIndex, string givenString, DeviceContext deviceContext)
        {
            // Format string for this sentance to report CPU Usage percetange
            string formattedText = string.Format("New Text: {0}", givenString);

            // Update the sentence vertex buffer with the new string information.
            if (!UpdateSentece(ref sentences[sentenceIndex], formattedText, 20, sentences[sentenceIndex].yPos, 1, 1, 1, deviceContext))
                return false;

            return true;
        }
        private bool InitializeSentence(out DSentence sentence, int maxLength, int yPosition, SharpDX.Direct3D11.Device device)
        {
            // Create a new sentence object.
            sentence = new DSentence();
            sentence.yPos = yPosition;

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
        private bool UpdateSentence2(ref DSentence sentence, string text, int positionX, int positionY, float red, float green, float blue, DeviceContext deviceContext)
        {
            // Store the color of the sentence.
            sentence.red = red;
            sentence.green = green;
            sentence.blue = blue;

            // Get the number of the letter in the sentence.
            var numLetters = sentence.MaxLength;

            // Check for possible buffer overflow.
            if (numLetters > sentence.MaxLength)
                return false;

            // Calculate the X and Y pixel position on screen to start drawing to.
            var drawX = -(ScreenWidth >> 1) + positionX;
            var drawY = (ScreenHeight >> 1) - positionY;

            // Use the font class to build the vertex array from the sentence text and sentence draw location.
            List<DFont.DVertexType> vertices;
            Font.BuildVertexArray(out vertices, text, drawX, drawY);

            // Empty all remaining vertices
            for (int i = text.Length; i < sentence.MaxLength; i++)
            {
                var emptyVertex = new DFont.DVertexType();
                emptyVertex.position = Vector3.Zero;
                emptyVertex.texture = Vector2.Zero;
                vertices.Add(emptyVertex);
            }

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
        private bool RenderSentence(DeviceContext deviceContext, DSentence sentence, Matrix worldMatrix, Matrix orthoMatrix)
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
            if (!FontShader.Render(deviceContext, sentence.IndexCount, worldMatrix, BaseViewMatrix, orthoMatrix, Font.Texture.TextureResource, pixelColor))
                return false;

            return true;
        }
    }
}