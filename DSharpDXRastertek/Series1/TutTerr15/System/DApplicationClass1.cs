using DSharpDXRastertek.TutTerr15.Graphics;
using DSharpDXRastertek.TutTerr15.Graphics.Camera;
using DSharpDXRastertek.TutTerr15.Graphics.Data;
using DSharpDXRastertek.TutTerr15.Graphics.Input;
using DSharpDXRastertek.TutTerr15.Graphics.Models;
using DSharpDXRastertek.TutTerr15.Graphics.Shaders;
using SharpDX;
using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.TutTerr15.System
{
    public class DApplication                   // 260 lines
    {
        // Properties
        public DInput Input { get; private set; }
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }
        public DPosition Position { get; set; }
        public DLight Light { get; set; }

        #region Models
        public DTerrainHeightMap TerrainModel { get; set; }
        #endregion

        #region Shaders
        public DBumpMapShader TerrainBumpMapShader { get; set; }
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

                // #region Initialize System
                // Create the Direct3D object.
                D3D = new DDX11();
                
                // Initialize the Direct3D object.
                if (!D3D.Initialize(configuration, windowHandle))
                    return false;

                // Create the camera object
                Camera = new DCamera();

                // Initialize a base view matrix with the camera for 2D user interface rendering.
                Camera.SetPosition(0.0f, 0.0f, -10.0f);
                Camera.RenderBaseViewMatrix();
                Matrix baseViewMatrix = Camera.BaseViewMatrix;

                // Set the initial position of the camera. (Since the ViewMatrix is already created from a base position.)              250.0f
                Camera.SetPosition(100.0f, 2.0f, 250.0f);
                Camera.SetRotation(0, 180.0f, 0);

                // Create the model object.
                TerrainModel = new DTerrainHeightMap();

               // Initialize the terrain object.
                if (!TerrainModel.Initialize(D3D.Device, "heightmap01.bmp", "dirt02.bmp", "bump.bmp"))
                    return false;

                // Create the position object.
                Position = new DPosition();

                // Set the initial position of the viewer to the same as the initial camera position.
                Position.SetPosition(Camera.GetPosition().X, Camera.GetPosition().Y, Camera.GetPosition().Z);
                Position.SetRotation(Camera.GetRotation().X, Camera.GetRotation().Y, Camera.GetRotation().Z);

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
                TerrainBumpMapShader = new DBumpMapShader();

                // Initialize the color shader object.
                if (!TerrainBumpMapShader.Initialize(D3D.Device, windowHandle))
                    return false;

                // Create the light object.
                Light = new DLight();

                // Initialize the light object.
                Light.SetDiffuseColor(1.0f, 1.0f, 1.0f, 1.0f);
                Light.Direction = new Vector3(0.75f, -0.5f, 0.0f);

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

            // Release the text object.
            Text?.Shutdown();
            Text = null;
            // Release the cpu object.
            CPU?.Shutdown();
            CPU = null;
            // Release the terrain shader object.
            TerrainBumpMapShader?.ShutDown();
            TerrainBumpMapShader = null;
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

            // Get the world, view, projection, ortho, and base view matrices from the camera and Direct3D objects.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix cameraViewMatrix = Camera.ViewMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;
            Matrix orthoD3DMatrix = D3D.OrthoMatrix;
           //  Matrix baseViewCameraMatrix = Camera.BaseViewMatrix;

            // Render the terrain using the terrain shader (BumpMapShader).
            TerrainModel.Render(D3D.DeviceContext);
            if (!TerrainBumpMapShader.Render(D3D.DeviceContext, TerrainModel.IndexCount, worldMatrix, cameraViewMatrix, projectionMatrix, Light.DiffuseColour, Light.Direction, TerrainModel.ColorTexture.TextureResource, TerrainModel.NormalMapTexture.TextureResource))
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