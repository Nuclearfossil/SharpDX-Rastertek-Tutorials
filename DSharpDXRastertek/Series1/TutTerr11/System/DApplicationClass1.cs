using DSharpDXRastertek.TutTerr11.Graphics;
using DSharpDXRastertek.TutTerr11.Graphics.Camera;
using DSharpDXRastertek.TutTerr11.Graphics.Data;
using DSharpDXRastertek.TutTerr11.Graphics.Input;
using DSharpDXRastertek.TutTerr11.Graphics.Models;
using DSharpDXRastertek.TutTerr11.Graphics.Shaders;
using SharpDX;
using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.TutTerr11.System
{
    public class DApplication                   // 343 lines
    {
        // Properties
        public DInput Input { get; private set; }
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }
        public DPosition Position { get; set; }
        public DLight Light { get; set; }

        #region Models
        public DTerrain TerrainModel { get; set; }
        public DSkyDome SkyDome { get; set; }
        public DSkyPlane SkyPlane { get; set; }
        #endregion

        #region Shaders
        public DTerrainShader TerrainShader { get; set; }
        public DSkyDomwShader SkyDomeShader { get; set; }
        public DSkyPlaneShader SkyPlaneShader { get; set; }
        #endregion     

        #region Text
        public DFPS FPS { get; set; }
        public DCPU CPU { get; set; }
        public DText Text { get; set; }
        #endregion

        // Construtor
        public DApplication() { }

        // Methods.
        public bool Initialize(DSystemConfiguration configuration, IntPtr windowHandle)
        {
            try
            {
                // Create the input object.  The input object will be used to handle reading the keyboard and mouse input from the user.
                Input = new DInput();

                // Initialize the input object.
                if (!Input.Initialize(configuration, windowHandle))
                    return false;   

                // Create the Direct3D object.
                D3D = new DDX11();
                
                // Initialize the Direct3D object.
                if (!D3D.Initialize(configuration, windowHandle))
                    return false;

                // Create the camera object
                Camera = new DCamera();

                // Initialize a base view matrix with the camera for 2D user interface rendering.
                Camera.SetPosition(0.0f, 0.0f, -1.0f);
                Camera.Render();
                Matrix baseViewMatrix = Camera.ViewMatrix;

                // Set the initial position of the camera. (Since the ViewMatrix is already created from a base position.)
                Camera.SetPosition(40.0f, 2.0f, -7.0f);

                // Create the model object.
                TerrainModel = new DTerrain();

               // Initialize the terrain object.
                if (!TerrainModel.Initialize(D3D.Device, "heightmap01.bmp", "dirt02.bmp", "colorm01.bmp"))
                    return false;

                // Create the position object.
                Position = new DPosition();

                // Set the initial position of the viewer to the same as the initial camera position.
                Position.SetPosition(Camera.GetPosition().X, Camera.GetPosition().Y, Camera.GetPosition().Z);

                // Create the fps object.
                FPS = new DFPS();

                // Initialize the fps object.
                FPS.Initialize();

                // Create the cpu object.
                CPU = new DCPU();

                // Initialize the cpu object.
                CPU.Initialize();

                // Create the text object.
                Text = new DText();

                // Initialize the text object.
                if (!Text.Initialize(D3D.Device, D3D.DeviceContext, windowHandle, configuration.Width, configuration.Height, baseViewMatrix))
                    return false;

                // Set the video card information in the text object.
                if (!Text.SetVideoCard(D3D.VideoCardDescription, D3D.VideoCardMemory, D3D.DeviceContext))
                    return false;

                // Create the color shader object.
                TerrainShader = new DTerrainShader();

                // Initialize the color shader object.
                if (!TerrainShader.Initialize(D3D.Device, windowHandle))
                    return false;

                // Create the light object.
                Light = new DLight();

                // Initialize the light object.
                Light.SetAmbientColor(0.05f, 0.05f, 0.05f, 1.0f);
                Light.SetDiffuseColor(1.0f, 1.0f, 1.0f, 1.0f);
                Light.Direction = new Vector3(-0.5f, -1.0f, 0.0f);

                // Create the sky dome object.
                SkyDome = new DSkyDome();

                // Initialize the sky dome object.
                if (!SkyDome.Initialize(D3D.Device))
                    return false;

                // Create the sky dome shader object.
                SkyDomeShader = new DSkyDomwShader();

                // Initialize the sky dome shader object.
                if (!SkyDomeShader.Initialize(D3D.Device, windowHandle))
                    return false;

                // Create the sky plane object.
                SkyPlane = new DSkyPlane();

                // Initialize the sky plane object.
                if (!SkyPlane.Initialze(D3D.Device, "cloud001.bmp", "cloud002.bmp"))
                    return false;

                // Create the sky plane shader object.
                SkyPlaneShader = new DSkyPlaneShader();

                // Initialize the sky plane shader object.
                if (!SkyPlaneShader.Initialize(D3D.Device, windowHandle))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not initialize Direct3D\nError is '" + ex.Message + "'");
                return false;
            }
        }
        public void Shutdown()
        {
            // Release the position object.
            Position = null;
            // Release the light object.
            Light = null;
            // Release the fps object.
            FPS = null;
            // Release the camera object.
            Camera = null;

            // Release the sky plane shader object.
            SkyPlaneShader?.ShutDown();
            SkyPlaneShader = null;
            // Release the sky plane object.
            SkyPlane?.ShurDown();
            SkyPlane = null;
            // Release the sky dome shader object.
            SkyDomeShader?.ShutDown();
            SkyDomeShader = null;
            // Release the sky dome object.
            SkyDome?.ShutDown();
            SkyDome = null;
            // Release the text object.
            Text?.Shutdown();
            Text = null;
            // Release the cpu object.
            CPU?.Shutdown();
            CPU = null;
            // Release the terrain shader object.
            TerrainShader?.ShutDown();
            TerrainShader = null;
            // Release the tree object.
            TerrainModel?.ShutDown();
            TerrainModel = null;
            // Release the input object.
            Input?.Shutdown();
            Input = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        private bool HandleInput(float frameTime)
        {
            // Set the frame time for calculating the updated position.
            Position.SetFrameTime(frameTime);

            // Handle the input
            bool keydown = Input.IsLeftArrowPressed();
            Position.TurnLeft(keydown);
            keydown = Input.IsRightArrowPressed();
            Position.TurnRight(keydown);
            keydown = Input.IsUpArrowPressed();
            Position.MoveForward(keydown);
            keydown = Input.IsDownArrowPressed();
            Position.MoveBackward(keydown);
            keydown = Input.IsPageUpPressed();
            Position.LookUpward(keydown);
            keydown = Input.IsPageDownPressed();
            Position.LookDownward(keydown);
            keydown = Input.IsAPressed();
            Position.MoveUpward(keydown);
            keydown = Input.IsZPressed();
            Position.MoveDownward(keydown);

            // Set the position and rOTATION of the camera.
            Camera.SetPosition(Position.PositionX, Position.PositionY, Position.PositionZ);
            Camera.SetRotation(Position.RotationX, Position.RotationY, Position.RotationZ);

            // Update the position values in the text object.
            Text.SetCameraPosition(Position.PositionX, Position.PositionY, Position.PositionZ, D3D.DeviceContext);

            // Update the rotation values in the text object.
            Text.SetCameraRotation(Position.RotationX, Position.RotationY, Position.RotationZ, D3D.DeviceContext);
            return true;
        }
        public bool Frame(float frameTime)
        {
            // Update the system stats.
            FPS.Frame();
            CPU.Frame();

            // Update the FPS value in the text object.
            if (!Text.SetFps(FPS.FPS, D3D.DeviceContext))
                return false;

            // Update the CPU usage value in the text object.
            if (!Text.SetCpu(CPU.CPUUsage, D3D.DeviceContext))
                return false;

            // Do the frame input processing.
            if (!HandleInput(frameTime))
                return false;

            // Do the sky plane frame processing.
            SkyPlane.Frame();

            // Render the graphics.
            if (!RenderGraphics())
                return false;

            return true;
        }
        private bool RenderGraphics()
        {
            // Clear the scene.
            D3D.BeginScene(0.0f, 0.0f, 0.0f, 1.0f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world, view, projection, and ortho matrices from the camera and Direct3D objects.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix cameraViewMatrix = Camera.ViewMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;
            Matrix orthoD3DMatrix = D3D.OrthoMatrix;

            // Get the position of the camera.
            Vector3 cameraPostion = Camera.GetPosition();

            // Translate the sky dome to be centered around the camera position.
            Matrix.Translation(cameraPostion.X, cameraPostion.Y, cameraPostion.Z, out worldMatrix);

            // Turn off back face culling.
            D3D.TurnOffCulling();

            // Turn off the Z buffer.
            D3D.TurnZBufferOff();

            // Render the sky dome using the sky dome shader.
            SkyDome.Render(D3D.DeviceContext);
            if (!SkyDomeShader.Render(D3D.DeviceContext, SkyDome.IndexCount, worldMatrix, cameraViewMatrix, projectionMatrix, SkyDome.ApexColour, SkyDome.CenterColour))
                return false;

            // Turn back face culling back on.
            D3D.TurnOnCulling();

            // Enable additive blending so the clouds blend with the sky dome color.
            D3D.EnableSecondBlendState();

            // Render the sky plane using the sky plane shader.
            SkyPlane.Render(D3D.DeviceContext);
            if (!SkyPlaneShader.Render(D3D.DeviceContext, SkyPlane.IndexCount, worldMatrix, cameraViewMatrix, projectionMatrix, SkyPlane.CloudTexture1.TextureResource, SkyPlane.CloudTexture2.TextureResource, SkyPlane.m_TextureTranslation[0], SkyPlane.m_TextureTranslation[1], SkyPlane.m_TextureTranslation[2], SkyPlane.m_TextureTranslation[3], SkyPlane.m_Brightness))
                return false;

            // Turn off blending
            D3D.TurnOffAlphaBlending();

            // Turn the Z buffer back on.
            D3D.TurnZBufferOn();

            // Reset the world matrix.
            worldMatrix = D3D.WorldMatrix;

            // Render the terrain buffers.
            TerrainModel.Render(D3D.DeviceContext);

            // Render the model using the color shader.
            if (!TerrainShader.Render(D3D.DeviceContext, TerrainModel.IndexCount, worldMatrix, cameraViewMatrix, projectionMatrix, Light.AmbientColor, Light.DiffuseColour, Light.Direction, TerrainModel.Texture.TextureResource))
                return false;

            // Turn off the Z buffer to begin all 2D rendering.
            D3D.TurnZBufferOff();

            // Turn on the alpha blending before rendering the text.
            D3D.TurnOnAlphaBlending();

            // Render the text user interface elements.
            if (!Text.Render(D3D.DeviceContext, worldMatrix, orthoD3DMatrix))
                return false;

            // Turn off alpha blending after rendering the text.
            D3D.TurnOffAlphaBlending();

            // Turn the Z buffer back on now that all 2D rendering has completed.
            D3D.TurnZBufferOn();

            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }
    }
}