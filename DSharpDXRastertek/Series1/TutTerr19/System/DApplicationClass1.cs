using DSharpDXRastertek.TutTerr19.Graphics;
using DSharpDXRastertek.TutTerr19.Graphics.Camera;
using DSharpDXRastertek.TutTerr19.Graphics.Data;
using DSharpDXRastertek.TutTerr19.Graphics.Input;
using DSharpDXRastertek.TutTerr19.Graphics.Models;
using DSharpDXRastertek.TutTerr19.Graphics.Shaders;
using SharpDX;
using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.TutTerr19.System
{
    public class DApplication                   // 234 lines
    {
        // Properties
        public DInput Input { get; private set; }
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }
        public DPosition Position { get; set; }

        #region Models
        public DFoliage Foliage { get; set; }
        public DModel GroundModel { get; set; }
        #endregion

        #region Shaders
        public DShaderManager ShaderManager { get; set; }
        #endregion     

        #region Text
        public DUserInterface UserInterface { get; set; }
        public DFPS FPS { get; set; }
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

                // Create the shader manager object.
                ShaderManager = new DShaderManager();

                // Initialize the shader manager object.
                if (!ShaderManager.Initilize(D3D, windowHandle))
                    return false;

                // Create the position object.
                Position = new DPosition();

                // Set the initial position and rotation of the viewer.
                Position.SetPosition(0.0f, 1.5f, -4.0f);
                Position.SetRotation(15.0f, 0.0f, 0.0f);

                // Create the camera object
                Camera = new DCamera();

                // Initialize a base view matrix with the camera for 2D user interface rendering.
                Camera.SetPosition(0.0f, 0.0f, -10.0f);
                Camera.Render();
                Camera.RenderBaseViewMatrix();

                // Create the fps object.
                FPS = new DFPS();

                //// Initialize the fps object.
                FPS.Initialize();

                // Create the user interface object.
                UserInterface = new DUserInterface();

                // Initialize the user interface object.
                if (!UserInterface.Initialize(D3D, configuration))
                    return false;

                // Create the ground model object.
                GroundModel = new DModel();

                // Initialize the ground model object.
                if (!GroundModel.Initialize(D3D.Device, "plane01.txt", "rock015.dds"))
                    return false;

                // Create the foliage object.
                Foliage = new DFoliage();

                // Initialize the foliage object.
                if (!Foliage.Initialize(D3D.Device, "grass01.dds", 500))
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
            // Release the FPS object.
            FPS = null;
            // Release the camera object.
            Camera = null;

            // Release the foliage object.
            Foliage?.ShutDown();
            Foliage = null;
            // Release the ground model object.
            GroundModel?.Shutdown();
            GroundModel = null;
            // Release the user interface object.
            UserInterface?.ShutDown();
            UserInterface = null;
            // Release the shader manager object.
            ShaderManager?.ShutDown();
            ShaderManager = null;
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

            return true;
        }
        public bool Frame(float frameTime)
        {
            // Update the system stats.
            FPS.Frame();
            
            // Do the frame input processing.
            if (!HandleInput(frameTime))
                return false;

            // Do the frame processing for the user interface.
            if (!UserInterface.Frame(FPS.FPS, Position.PositionX, Position.PositionY, Position.PositionZ, Position.RotationX, Position.RotationY, Position.RotationZ, D3D.DeviceContext))
                return false;

            // Do the frame processing for the foliage.
            if (!Foliage.Frame(Camera.GetPosition(), D3D.DeviceContext))
                return false;

            // Render the graphics.
            if (!Render())
                return false;

            return true;
        }
        private bool Render()
        {
            // Clear the scene.
            D3D.BeginScene(0.0f, 0.0f, 1.0f, 1.0f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world, view, projection, ortho, and base view matrices from the camera and Direct3D objects.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix viewCameraMatrix = Camera.ViewMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;
            Matrix orthoMatrix = D3D.OrthoMatrix;
            Matrix baseViewMatrix = Camera.BaseViewMatrix;

            // Render the ground model.
            GroundModel.Render(D3D.DeviceContext);
            ShaderManager.RenderTextureShader(D3D.DeviceContext, GroundModel.IndexCount, worldMatrix, viewCameraMatrix, projectionMatrix, GroundModel.ColourTexture.TextureResource);

            // Turn on the alpha-to-coverage blending.
            D3D.EnableSecondBlendState();

            // Render the foliage.
            Foliage.Render(D3D.DeviceContext);
            if (!ShaderManager.RenderFoliageShader(D3D.DeviceContext, Foliage.VertexCount, Foliage.InstanceCount, viewCameraMatrix, projectionMatrix, Foliage.Texture.TextureResource))
                return false;

            // Turn off the alpha blending.
            D3D.TurnOffAlphaBlending();

            // Render the user interface.
            UserInterface.Render(D3D, ShaderManager,worldMatrix, baseViewMatrix, orthoMatrix);

            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }
    }
}