using DSharpDXRastertek.Series2.TutTerr05.Graphics.Models;
using DSharpDXRastertek.Series2.TutTerr05.Graphics.Shaders;
using DSharpDXRastertek.Series2.TutTerr05.System;
using SharpDX;
using SharpDX.Direct3D11;
using System;

namespace DSharpDXRastertek.Series2.TutTerr05.Graphics.Data
{
    public class DUserInterface
    {
        // properties
        public DFont Font1 { get; set; }
        public DText FpsString { get; set; }
        public int PreviousFPS { get; set; }
        public DText[] PositionStrings { get; set; }
        public DText[] VideoStrings { get; set; }
        public int[] PreviousPositions { get; set; }

        // Constructor
        public DUserInterface() { }

        // Methods
        public bool Initialize(DDX11 D3Ddevice, DSystemConfiguration configuration)
        {
            // Create the first font object.
            Font1 = new DFont();

            // Initialize the first font object.
            if (!Font1.Initialize(D3Ddevice.Device, "font01.txt", "font01.bmp", 32.0f, 3))
                return false;

            // Create the text object for the fps string.
            FpsString = new DText();

            // Initialize the fps text string.
            if (!FpsString.Initialize(D3Ddevice.Device, Font1, configuration, 16, "FPS: 0", 10, 50, 0.0f, 1.0f, 0.0f, false, D3Ddevice.DeviceContext))
                return false;

            // Initial the previous frame fps.
            PreviousFPS = -1;

            // Create the text objects for the position strings.
            PositionStrings = new DText[6];
            for (int i = 0; i < PositionStrings.Length; i++)
                PositionStrings[i] = new DText();

            // Initialize the position text strings.
            if (!PositionStrings[0].Initialize(D3Ddevice.Device, Font1, configuration, 16, "X: 0", 10, 90, 1.0f, 1.0f, 1.0f, false, D3Ddevice.DeviceContext))
                return false;
            if (!PositionStrings[1].Initialize(D3Ddevice.Device, Font1, configuration, 16, "Y: 0", 10, 110, 1.0f, 1.0f, 1.0f, false, D3Ddevice.DeviceContext))
                return false;
            if (!PositionStrings[2].Initialize(D3Ddevice.Device, Font1, configuration, 16, "Z: 0", 10, 130, 1.0f, 1.0f, 1.0f, false, D3Ddevice.DeviceContext))
                return false;
            if (!PositionStrings[3].Initialize(D3Ddevice.Device, Font1, configuration, 16, "rX: 0", 10, 170, 1.0f, 1.0f, 1.0f, false, D3Ddevice.DeviceContext))
                return false;
            if (!PositionStrings[4].Initialize(D3Ddevice.Device, Font1, configuration, 16, "rY: 0", 10, 190, 1.0f, 1.0f, 1.0f, false, D3Ddevice.DeviceContext))
                return false;
            if (!PositionStrings[5].Initialize(D3Ddevice.Device, Font1, configuration, 16, "rZ: 0", 10, 210, 1.0f, 1.0f, 1.0f, false, D3Ddevice.DeviceContext))
                return false;

            // Initialize the previous frame position.
            PreviousPositions = new int[6];
            VideoStrings = new DText[2];
            for (int i = 0; i < VideoStrings.Length; i++)
                VideoStrings[i] = new DText();

            // Initialize the position text strings.
            if (!VideoStrings[0].Initialize(D3Ddevice.Device, Font1, configuration, 256, D3Ddevice.VideoCardDescription, 10, 10, 1.0f, 1.0f, 1.0f, false, D3Ddevice.DeviceContext))
                return false;
            if (!VideoStrings[1].Initialize(D3Ddevice.Device, Font1, configuration, 64, D3Ddevice.VideoCardMemory.ToString(), 10, 30, 1.0f, 1.0f, 1.0f, false, D3Ddevice.DeviceContext))
                return false;

            return true;
        }
        public void ShutDown()
        {
            PreviousPositions = null;
            // Release the position text strings.
            foreach (DText sent in VideoStrings)
                sent?.Shutdown();
            VideoStrings = null;

            // Release the position text strings.
            foreach (DText sentence in PositionStrings)
                sentence?.Shutdown();
            PositionStrings = null;
            // Release the fps text string.
            FpsString?.Shutdown();
            FpsString = null;
            // Release the font object.
            Font1?.Shutdown();
            Font1 = null;
        }
        public bool Frame(DeviceContext deviceContext, int fps, float posX, float posY, float posZ, float rotX, float rotY, float rotZ)
        {
            // Update the fps string.
            if (!UpdateFPSString(fps, deviceContext))
                return false;

            // Update the position strings.
            if (!UpdatePositionStrings(posX, posY, posZ, rotX, rotY, rotZ, deviceContext))
                return false;

            return true;
        }
        private bool UpdatePositionStrings(float posX, float posY, float posZ, float rotX, float rotY, float rotZ, DeviceContext deviceContext)
        {
            // Update the position strings if the value has changed since the last frame.
            if (posX != PreviousPositions[0])
            {
                PreviousPositions[0] = (int)posX;
                string finalString = String.Format("X: {0}", PreviousPositions[0]);
                if (!PositionStrings[0].UpdateSentence2(Font1, finalString, 10, 90, 1.0f, 1.0f, 1.0f, deviceContext))
                    return false;
            }
            if (posY != PreviousPositions[1])
            {
                PreviousPositions[1] = (int)posY;
                string finalString = String.Format("Y: {0}", PreviousPositions[1]);
                if (!PositionStrings[1].UpdateSentence2(Font1, finalString, 10, 110, 1.0f, 1.0f, 1.0f, deviceContext))
                    return false;
            }
            if (posZ != PreviousPositions[2])
            {
                PreviousPositions[2] = (int)posZ;
                string finalString = String.Format("Z: {0}", PreviousPositions[2]);
                if (!PositionStrings[2].UpdateSentence2(Font1, finalString, 10, 130, 1.0f, 1.0f, 1.0f, deviceContext))
                    return false;
            }
            if (rotX != PreviousPositions[3])
            {
                PreviousPositions[3] = (int)rotX;
                string finalString = String.Format("rX: {0}", PreviousPositions[3]);
                if (!PositionStrings[3].UpdateSentence2(Font1, finalString, 10, 170, 1.0f, 1.0f, 1.0f, deviceContext))
                    return false;
            }
            if (rotY != PreviousPositions[4])
            {
                PreviousPositions[4] = (int)rotY;
                string finalString = String.Format("rY: {0}", PreviousPositions[4]);
                if (!PositionStrings[4].UpdateSentence2(Font1, finalString, 10, 190, 1.0f, 1.0f, 1.0f, deviceContext))
                    return false;
            }
            if (rotZ != PreviousPositions[5])
            {
                PreviousPositions[5] = (int)rotZ;
                string finalString = String.Format("rZ: {0}", PreviousPositions[5]);
                if (!PositionStrings[5].UpdateSentence2(Font1, finalString, 10, 210, 1.0f, 1.0f, 1.0f, deviceContext))
                    return false;
            }

            return true;
        }
        private bool UpdateFPSString(int fps, DeviceContext deviceContext)
        {
            float red = 0.0f, green = 0.0f, blue = 0.0f;

            // Check if the fps from the previous frame was the same, if so don't need to update the text string.
            if (PreviousFPS == fps)
                return true;

            // Store the fps for checking next frame.
            PreviousFPS = fps;

            // Truncate the fps to below 100,000.
            if (fps > 99999)
                fps = 99999;

            // Setup the fps string.
            string finalString = String.Format("FPS: {0}", fps);

            // If fps is 60 or above set the fps color to green.
            if (fps >= 60)
            {
                red = 0.0f;
                green = 1.0f;
                blue = 0.0f;
            }
            // If fps is below 60 set the fps color to yellow.
            if (fps < 60)
            {
                red = 1.0f;
                green = 1.0f;
                blue = 0.0f;
            }
            // If fps is below 30 set the fps color to red.
            if (fps < 60)
            {
                red = 1.0f;
                green = 0.0f;
                blue = 0.0f;
            }

            // Update the sentence vertex buffer with the new string information.
            if (!FpsString.UpdateSentence2(Font1, finalString, 10, 50, red, green, blue, deviceContext))
                return false;

            return true;
        }
        public bool Render(DDX11 D3DDevice, DShaderManager shaderManager, Matrix worldMatrix, Matrix baseViewMatrix, Matrix orthoMatrix)
        {
            // Turn off the Z buffer and enable alpha blending to begin 2D rendering.
            D3DDevice.TurnZBufferOff();
            D3DDevice.TurnOnAlphaBlending();

            // Render the fps string.
            FpsString.Render(D3DDevice.DeviceContext, shaderManager, worldMatrix, baseViewMatrix, orthoMatrix, Font1.Texture.TextureResource);

            // Render the position and rotation strings.
            foreach (DText position in PositionStrings)
                position.Render(D3DDevice.DeviceContext, shaderManager, worldMatrix, baseViewMatrix, orthoMatrix, Font1.Texture.TextureResource);

            // Render the video card strings.
            foreach (DText vidString in VideoStrings)
                vidString.Render(D3DDevice.DeviceContext, shaderManager, worldMatrix, baseViewMatrix, orthoMatrix, Font1.Texture.TextureResource);

            // Turn the Z buffer back on and disable alpha blending now that the 2D rendering has completed.
            D3DDevice.TurnZBufferOn();
            D3DDevice.TurnOffAlphaBlending();

            return true;
        }
    }
}
