using DSharpDXRastertek.TutTerr17.Graphics;
using DSharpDXRastertek.TutTerr17.Graphics.Camera;
using DSharpDXRastertek.TutTerr17.Graphics.Data;
using DSharpDXRastertek.TutTerr17.Graphics.Input;
using DSharpDXRastertek.TutTerr17.Graphics.Models;
using DSharpDXRastertek.TutTerr17.Graphics.Shaders;
using SharpDX;
using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.TutTerr17.System
{
    public class DApplication                   // 253 lines
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
        public DTerrainShader TerrainShader { get; set; }
        #endregion     

        #region Textures
        public DTexture ColourTexture1 { get; set; }
        public DTexture ColourTexture2 { get; set; }
        public DTexture ColourTexture3 { get; set; }
        public DTexture ColourTexture4 { get; set; }
        public DTexture AlphaTexture1 { get; set; }
        public DTexture NormalTexture1 { get; set; }
        public DTexture NormalTexture2 { get; set; }
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

                // Create the position object.
                Position = new DPosition();

                // Set the initial position and rotation of the viewer.
                Position.SetPosition(15.0f, 13.0f, 20.0f);
                Position.SetRotation(25.0f, 180.0f, 0.0f);

                // Create the camera object
                Camera = new DCamera();

                // Create the light object.
                Light = new DLight();

                // Initialize the light object.
                Light.Direction = new Vector3(0.5f, -0.75f, 0.0f);

                // Create the model object.
                TerrainModel = new DTerrainHeightMap();

               // Initialize the terrain object.
                if (!TerrainModel.Initialize(D3D.Device, "hm01.bmp", 10.0f))
                    return false;

                // Create the color shader object.
                TerrainShader = new DTerrainShader();

                //// Initialize the color shader object.
                if (!TerrainShader.Initialize(D3D.Device, windowHandle))
                    return false;

                // Create the first color texture object.
                ColourTexture1 = new DTexture();

                // Load the first color texture object.
                if (!ColourTexture1.Initialize(D3D.Device, DSystemConfiguration.DataFilePath + "dirt001.dds"))
                    return false;

                // Create the second color texture object.
                ColourTexture2 = new DTexture();

                // Load the second color texture object.
                if (!ColourTexture2.Initialize(D3D.Device, DSystemConfiguration.DataFilePath + "dirt004.dds"))
                    return false;

                // Create the third color texture object.
                ColourTexture3 = new DTexture();

                // Load the third color texture object.
                if (!ColourTexture3.Initialize(D3D.Device, DSystemConfiguration.DataFilePath + "dirt002.dds"))
                    return false;

                // Create the fourth color texture object.
                ColourTexture4 = new DTexture();

                // Load the forth color texture object.
                if (!ColourTexture4.Initialize(D3D.Device, DSystemConfiguration.DataFilePath + "stone001.dds"))
                    return false;

                // Create the first alpha texture object.
                AlphaTexture1 = new DTexture();

                // Load the first alpha texture object.
                if (!AlphaTexture1.Initialize(D3D.Device, DSystemConfiguration.DataFilePath + "alphaRoad001.dds"))
                    return false;

                // Create the first normal texture object.
                NormalTexture1 = new DTexture();

                // Load the first alpha/Normal texture object.
                if (!NormalTexture1.Initialize(D3D.Device, DSystemConfiguration.DataFilePath + "normal001.dds"))
                    return false;

                // Create the second normal texture object.
                NormalTexture2 = new DTexture();

                // Load the second alpha/Normal texture object.
                if (!NormalTexture2.Initialize(D3D.Device, DSystemConfiguration.DataFilePath + "normal002.dds"))
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
            // Release the camera object.
            Camera = null;

            // Release the texture objects.
            ColourTexture1?.ShutDown();
            ColourTexture1 = null;
            ColourTexture2?.ShutDown();
            ColourTexture2 = null;
            ColourTexture3?.ShutDown();
            ColourTexture3 = null;
            ColourTexture4?.ShutDown();
            ColourTexture4 = null;
            AlphaTexture1?.ShutDown();
            AlphaTexture1 = null;
            NormalTexture1?.ShutDown();
            NormalTexture1 = null;
            NormalTexture2?.ShutDown();
            NormalTexture2 = null;
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

            return true;
        }
        public bool Frame(float frameTime)
        {
            // Do the frame input processing.
            if (!HandleInput(frameTime))
                return false;

            // Render the graphics.
            if (!Render())
                return false;

            return true;
        }
        private bool Render()
        {
            // Clear the scene.
            D3D.BeginScene(0.0f, 0.0f, 0.0f, 1.0f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world, view, projection, ortho, and base view matrices from the camera and Direct3D objects.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix viewCameraMatrix = Camera.ViewMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;
            Matrix orthoMatrix = D3D.OrthoMatrix;

            // Render the terrain using the terrain shader.
            TerrainModel.Render(D3D.DeviceContext);
            if (!TerrainShader.Render(D3D.DeviceContext, TerrainModel.IndexCount, worldMatrix, viewCameraMatrix, projectionMatrix, Light.Direction, ColourTexture1.TextureResource, ColourTexture2.TextureResource, ColourTexture3.TextureResource, ColourTexture4.TextureResource, AlphaTexture1.TextureResource, NormalTexture1.TextureResource, NormalTexture2.TextureResource))
                return false;

            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }
    }
}