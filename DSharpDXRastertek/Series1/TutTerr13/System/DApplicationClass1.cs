using DSharpDXRastertek.TutTerr13.Graphics;
using DSharpDXRastertek.TutTerr13.Graphics.Camera;
using DSharpDXRastertek.TutTerr13.Graphics.Data;
using DSharpDXRastertek.TutTerr13.Graphics.Input;
using DSharpDXRastertek.TutTerr13.Graphics.Models;
using DSharpDXRastertek.TutTerr13.Graphics.Shaders;
using SharpDX;
using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.TutTerr13.System
{
    public class DApplication                   // 345 lines
    {
        // Properties
        public DInput Input { get; private set; }
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }
        public DPosition Position { get; set; }
        public DLight Light { get; set; }

        #region Models
        public DTerrain TerrainModel { get; set; }
        public DDebugWindow DebugWindow { get; set; }
        public DRenderTexture RenderTexture { get; set; }
        #endregion

        #region Shaders
        public DTerrainShader TerrainShader { get; set; }
        public DTextureShader TextureShader { get; set; }
        public DDepthShader DepthShader { get; set; }
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
                Camera.SetPosition(0.0f, 0.0f, -10.0f);
                Camera.RenderBaseViewMatrix();
                Matrix baseViewMatrix = Camera.BaseViewMatrix;

                // Set the initial position of the camera.
                Camera.SetPosition(100.0f, 2.0f, 5.0f);

                // Create the model object.
                TerrainModel = new DTerrain();

               // Initialize the terrain object.
                if (!TerrainModel.Initialize(D3D.Device, "heightmap01.bmp", "dirt02.dds", "colorm01.bmp", "detail001.dds"))
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

                // Create the terrain shader object.
                TerrainShader = new DTerrainShader();

                // Initialize the terrain shader object.
                if (!TerrainShader.Initialize(D3D.Device, windowHandle))
                    return false;

                // Create the light object.
                Light = new DLight();

                // Initialize the light object.
                Light.SetAmbientColor(0.05f, 0.05f, 0.05f, 1.0f);
                Light.SetDiffuseColor(1.0f, 1.0f, 1.0f, 1.0f);
                Light.Direction = new Vector3(-0.5f, -1.0f, 0.0f);

                // Create the debug window bitmap object.
                DebugWindow = new DDebugWindow();

                // Initialize the debug window bitmap object.
                if (!DebugWindow.Initialize(D3D.Device, configuration.Width, configuration.Height, 256, 256))
                    return false;

                // Create the texture shader object.
                TextureShader = new DTextureShader();

                // Initialize the texture shader object.
                if (!TextureShader.Initialize(D3D.Device, windowHandle))
                    return false;

                // Create the render to texture object.
                RenderTexture = new DRenderTexture();

                // Initialize the render to texture object.
                if (!RenderTexture.Initialize(D3D.Device, configuration))
                    return false;

                // Create the depth shader object.
                DepthShader = new DDepthShader();

                // Initialize the depth shader object.
                if (!DepthShader.Initialize(D3D.Device, windowHandle))
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

            // Release the depth shader object.
            DepthShader?.ShutDown();
            DepthShader = null;
            // Release the render to texture object.
            RenderTexture?.Shutdown();
            RenderTexture = null;
            // Release the texture shader object.
            TextureShader?.ShutDown();
            TextureShader = null;
            // Release the debug window bitmap object.
            DebugWindow?.Shutdown();
            DebugWindow = null;
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

            // Render the graphics.
            if (!RenderGraphics())
                return false;

            return true;
        }
        private bool RenderGraphics()
        {
            // First render the scene to a texture.
            if (!RenderSceneToTexture())
                return false;

            // Clear the scene.
            D3D.BeginScene(0.0f, 0.0f, 0.0f, 1.0f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            //// Get the world, view, projection, ortho, and base view matrices from the camera and Direct3D objects.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix cameraViewMatrix = Camera.ViewMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;
            Matrix orthoD3DMatrix = D3D.OrthoMatrix;
            Matrix baseViewMatrix = Camera.BaseViewMatrix;

            // Render the terrain buffers.
            TerrainModel.Render(D3D.DeviceContext);

            // Render the terrain using the terrain shader.
            if (!TerrainShader.Render(D3D.DeviceContext, TerrainModel.IndexCount, worldMatrix, cameraViewMatrix, projectionMatrix, Light.AmbientColor, Light.DiffuseColour, Light.Direction, TerrainModel.Texture.TextureResource, TerrainModel.DetailTexture.TextureResource))
                return false;

           ///// Turn off the Z buffer to begin all 2D rendering.
           D3D.TurnZBufferOff();

           // Put the debug window on the graphics pipeline to prepare it for drawing.
           if (!DebugWindow.Render(D3D.DeviceContext, 100, 60))
               return false;

           // Render the bitmap model using the texture shader and the render to texture resource.
           if (!TextureShader.Render(D3D.DeviceContext, DebugWindow.IndexCount, worldMatrix, baseViewMatrix, orthoD3DMatrix, RenderTexture.ShaderResourceView))
               return false;

            // Turn on the alpha blending before rendering the text.
            D3D.TurnOnAlphaBlending();

            // Render the text user interface elements.
            if (!Text.Render(D3D.DeviceContext, worldMatrix, orthoD3DMatrix))
                return false;

            // Turn off alpha blending after rendering the text.
            D3D.TurnOffAlphaBlending();

            // Turn the Z buffer back on now that all 2D rendering has completed.
            D3D.TurnZBufferOn();

           /// Present the rendered scene to the screen.
           D3D.EndScene();

            return true;
        }
        private bool RenderSceneToTexture()
        {
            // Set the render target to be the render to texture.
            RenderTexture.SetRenderTarget(D3D.DeviceContext);

            // Clear the render to texture.
            RenderTexture.ClearRenderTarget(D3D.DeviceContext, 0.0f, 0.0f, 0.0f, 1.0f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world, view, projection, ortho, and base view matrices from the camera and Direct3D objects.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix viewMatrix = Camera.ViewMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;

            // Render the terrain using the depth shader.
            TerrainModel.Render(D3D.DeviceContext);
            if (!DepthShader.Render(D3D.DeviceContext, TerrainModel.IndexCount, worldMatrix, viewMatrix, projectionMatrix))
                return false;

            // Reset the render target back to the original back buffer and not the render to texture anymore.
            D3D.SetBackBufferRenderTarget();

            // Reset the viewport back to the original.
            D3D.ResetViewPort();

            return true;
        }
    }
}