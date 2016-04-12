using DSharpDXRastertek.Tut49.Graphics.Camera;
using DSharpDXRastertek.Tut49.Graphics.Data;
using DSharpDXRastertek.Tut49.Graphics.Models;
using DSharpDXRastertek.Tut49.Graphics.Shaders;
using DSharpDXRastertek.Tut49.Input;
using DSharpDXRastertek.Tut49.System;
using SharpDX;
using System;
using System.Windows.Forms;
using TestConsole;

namespace DSharpDXRastertek.Tut49.Graphics
{
    public class DApplication                   // 389 lines
    {
        // Variables
        float SHADOWMAP_DEPTH = 50.0f;
        float SHADOWMAP_NEAR = 1.0f;
        int SHADOWMAP_WIDTH = 1024;
        int SHADOWMAP_HEIGHT = 1024;
        float sunAngle = 270.0f;
        float offsetX = 9.0f;

        // Properties
        public DInput Input { get; private set; }
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }
        public DPosition Position { get; set; }

        #region Models
        public DTimer Timer { get; set; }
        public DModel GroundModel { get; set; }
        public DTreeModel TreeModel { get; set; }
        public DLight Light { get; set; }
        #endregion

        #region Shaders
        public DRenderTexture RenderTexture { get; set; }
        public DDepthShader DepthShader { get; set; }
        public DTransparentDepthShader TransparentDepthShader { get; set; }
        public DShadowShader ShadowShader { get; set; }
        #endregion     

        // Construtor
        public DApplication() { }

        // Methods.
        public bool Initialize(DSystemConfiguration configuration, IntPtr windowHandle, string appTitle, int testTimeSeconds)
        {
            try
            {
                // Create the input object.
                Input = new DInput();

                // Initialize the input object.
                if (!Input.Initialize(configuration, windowHandle))
                    return false;   

                #region Initialize System
                // Create the Direct3D object.
                D3D = new DDX11();
                
                // Initialize the Direct3D object.
                if (!D3D.Initialize(configuration, windowHandle))
                    return false;

                // Create the timer object.
                Timer = new DTimer();

                // Initialize the timer object.
                if (!Timer.Initialize())
                    return false;

                // Create the position object.
                Position = new DPosition();

                // Set the initial position.
                Position.SetPosition(0.0f, 7.0f, -11.0f);
                Position.SetRotation(20.0f, 0.0f, 0.0f);
                #endregion

                #region Initialize Camera
                // Create the camera object
                Camera = new DCamera();

                // Create the light object.
                Light = new DLight();

                // Initialize the light object.
                Light.GenerateOrthoMatrix(15.0f, 15.0f, SHADOWMAP_DEPTH, SHADOWMAP_NEAR);
                #endregion

                // Create the ground model object.
                GroundModel = new DModel();

                // Initialize the ground model object.
                if (!GroundModel.Initialize(D3D.Device, "plane01.txt", "dirt.bmp", 2.0f))
                    return false;

                // Set the position for the ground model.
                GroundModel.SetPosition(0.0f, 1.0f, 0.0f);

                // Create the tree object.
                TreeModel = new DTreeModel();

                // Initialize the shadow shader object.
                if (!TreeModel.Initialize(D3D.Device, "trunk001.txt", "trunk001.bmp", "leaf001.txt", "leaf001.bmp", 0.1f))
                    return false;

                // Set the position for the tree model.
                TreeModel.SetPosition(0.0f, 1.0f, 0.0f);

                // Create the render to texture object.
                RenderTexture = new DRenderTexture();

                // Initialize the render to texture object.
                if (!RenderTexture.Initialize(D3D.Device, SHADOWMAP_WIDTH, SHADOWMAP_HEIGHT, SHADOWMAP_DEPTH, SHADOWMAP_NEAR))
                    return false;

                // Create the depth shader object.
                DepthShader = new DDepthShader();

                // Initialize the depth shader object.
                if (!DepthShader.Initialize(D3D.Device, windowHandle))
                    return false;

                // Create the transparent depth shader object.
                TransparentDepthShader = new DTransparentDepthShader();

                // Initialize the transparent depth shader object.
                if (!TransparentDepthShader.Initialize(D3D.Device, windowHandle))
                    return false;

                // Create the shadow shader object.
                ShadowShader = new DShadowShader();

                // Initialize the shadow shader object.
                if (!ShadowShader.Initialize(D3D.Device, windowHandle))
                    return false;

                DPerfLogger.Initialize("WinForms C# SharpDX: " + configuration.Width + "x" + configuration.Height + " VSync:" + DSystemConfiguration.VerticalSyncEnabled + " FullScreen:" + DSystemConfiguration.FullScreen + "   " + appTitle, testTimeSeconds, configuration.Width, configuration.Height);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not initialize Direct3D\nError is '" + ex.Message + "'");
                return false;
            }
        }
        public void ShutDown()
        {
            DPerfLogger.ShutDown();

            // Release the light object.
            Light = null;
            // Release the position object.
            Position = null;
            // Release the timer object.
            Timer = null;
            // Release the camera object.
            Camera = null;

            // Release the shadow shader object.
            ShadowShader?.ShutDown();
            ShadowShader = null;
            // Release the transparent depth shader object.
            TransparentDepthShader?.ShutDown();
            TransparentDepthShader = null;
            // Release the depth shader object.
            DepthShader?.ShutDown();
            DepthShader = null;
            // Release the render to texture object.
            RenderTexture?.Shutdown();
            RenderTexture = null;
            // Release the tree object.
            TreeModel?.Shutdown();
            TreeModel = null;
            // Release the model object.
            GroundModel?.Shutdown();
            GroundModel = null;
            // Release the input object.
            Input?.Shutdown();
            Input = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Frame()
        {
            // Read the user input.
            if (!Input.Frame() || Input.IsEscapePressed())
                return false;

            // Update the system stats.
            Timer.Frame2();
            if (DPerfLogger.IsTimedTest)
            {
                DPerfLogger.Frame(Timer.FrameTime);
                if (Timer.CumulativeFrameTime >= DPerfLogger.TestTimeInSeconds * 1000)
                    return false;
            }

            // Do the frame input processing.
            if (!HandleMovementInput(Timer.FrameTime))
                return false;

            // Update the scene lighting.
            UpdateLighting();

            // Render the graphics.
            if (!Render())
                return false;

            return true;
        }
        private void UpdateLighting()
        {
            // Update direction of the light (Sum).
            sunAngle -= 0.03f * Timer.FrameTime;

            // Cycle the day
            if (sunAngle < 90.0f)
            {
                sunAngle = 270.0f;
                offsetX = 9.0f;
            }
            float radians = sunAngle * 0.0174532925f;
            
            // Update the direction the Sunlight is facing.
            Light.Direction = new Vector3((float)Math.Sin(radians), (float)Math.Cos (radians), 0.0f);

            // Update the lookat and position of Sunlight.
            offsetX -= 0.003f * Timer.FrameTime;
            Light.Position = new Vector3(0.0f + offsetX, 10.0f, 1.0f);
            Light.SetLookAt(0.0f - offsetX, 0.0f, 2.0f);
        }
        private bool HandleMovementInput(float time)
        {
            // Set the frame time for calculating the updated position.
            Position.FrameTime = time;

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
            Position.LookUp(keydown);
            keydown = Input.IsPageDownPressed();
            Position.LookDown(keydown);
            keydown = Input.IsAPressed();
            Position.MoveUpward(keydown);
            keydown = Input.IsZPressed();
            Position.MoveDownward(keydown);

            // Get the view point position/rotation.
            Vector3 currentPosition = new Vector3(Position.PositionX, Position.PositionY, Position.PositionZ);
            Vector3 currentRotation = new Vector3(Position.RotationX, Position.RotationY, Position.RotationZ);

            // Set the position of the camera.
            Camera.SetPosition(currentPosition.X, currentPosition.Y, currentPosition.Z);
            Camera.SetRotation(currentRotation.X, currentRotation.Y, currentRotation.Z);

            return true;
        }
        private bool Render()
        {
            // Render the depth of the scene to a texture.
            if (!RenderSceneToTexture())
                return false;

            // Clear the scene to Grey
            D3D.BeginScene(0.0f, 0.5f, 0.8f, 1.0f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Generate the light view matrix based on the light's position.
            Light.GenerateViewMatrix();

            // Get the matrices from the camera and d3d objects.
            Matrix viewCameraMatrix = Camera.ViewMatrix;
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;

            // Get the light's view and projection matrices from the light object.
            Matrix lightViewMatrix = Light.ViewMatrix;
            Matrix lightOrthoMatrix = Light.OrthoMatrix;

            // Set the light color attributes.
            Light.SetDiffuseColor(1.0f, 1.0f, 1.0f, 1.0f);
            Light.SetAmbientColor(0.15f, 0.15f, 0.15f, 1.0f);

            // Translate to the position of the ground model.
            Vector3 groundPosition = GroundModel.GetPosition();
            Matrix.Translation(groundPosition.X, groundPosition.Y, groundPosition.Z, out worldMatrix);

            // Render the ground model using the shadow shader.
            GroundModel.Render(D3D.DeviceContext);
            if (!ShadowShader.Render(D3D.DeviceContext, GroundModel.IndexCount, worldMatrix, viewCameraMatrix, projectionMatrix, lightViewMatrix, lightOrthoMatrix, GroundModel.Texture.TextureResource, RenderTexture.ShaderResourceView, Light.Direction, Light.AmbientColor, Light.DiffuseColour))
                return false;

            // Reset the world matrix.
            worldMatrix = D3D.WorldMatrix;

            // Translate to the position of the tree model.
            Vector3 treePosition = TreeModel.GetPosition();
            Matrix.Translation(treePosition.X, treePosition.Y, treePosition.Z, out worldMatrix);

            // Render the tree trunk.
            TreeModel.RenderTrunk(D3D.DeviceContext);
            if (!ShadowShader.Render(D3D.DeviceContext, TreeModel.TrunkIndexCount, worldMatrix, viewCameraMatrix, projectionMatrix, lightViewMatrix, lightOrthoMatrix, TreeModel.TrunkTexture.TextureResource, RenderTexture.ShaderResourceView, Light.Direction, Light.AmbientColor, Light.DiffuseColour))
                return false;

            // Enable blending and 
            D3D.TurnOnAlphaBlending();

            // Render the tree leaves.
            TreeModel.RenderLeaves(D3D.DeviceContext);
            if (!ShadowShader.Render(D3D.DeviceContext, TreeModel.LeafIndexCount, worldMatrix, viewCameraMatrix, projectionMatrix, lightViewMatrix, lightOrthoMatrix, TreeModel.LeafTexture.TextureResource, RenderTexture.ShaderResourceView, Light.Direction, Light.AmbientColor, Light.DiffuseColour))
                return false;
            
            // Turn off Alpha Blending.
            D3D.TurnOffAlphaBlending();

            // Reset the world matrix.
            worldMatrix = D3D.WorldMatrix;

            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }
        private bool RenderSceneToTexture()
        {
            // Set the render target to be the render to texture.
            RenderTexture.SetRenderTarget(D3D.DeviceContext);

            // Clear the render to texture.
            RenderTexture.ClearRenderTarget(D3D.DeviceContext, 0.0f, 0.0f, 0.0f, 1.0f);

            // Get the world matrix from the d3d object.
            Matrix worldMatrix = D3D.WorldMatrix;

            // Generate the light view matrix based on the light's position.
            Light.GenerateViewMatrix();

            // Get the view and orthographic matrices from the light object.
            Matrix lightViewMatrix = Light.ViewMatrix;
            Matrix lightOrthoMatrix = Light.OrthoMatrix;

            // Translate to the position of the tree.
            Vector3 treePosition = TreeModel.GetPosition();
            Matrix.Translation(treePosition.X, treePosition.Y, treePosition.Z, out worldMatrix);

            // Render the tree trunk with the depth shader.
            TreeModel.RenderTrunk(D3D.DeviceContext);
            if (!DepthShader.Render(D3D.DeviceContext, TreeModel.TrunkIndexCount, worldMatrix, lightViewMatrix, lightOrthoMatrix))
                return false;

            // Render the tree leaves using the depth transparency shader.
            TreeModel.RenderLeaves(D3D.DeviceContext);
            if (!TransparentDepthShader.Render(D3D.DeviceContext, TreeModel.LeafIndexCount, worldMatrix, lightViewMatrix, lightOrthoMatrix, TreeModel.LeafTexture.TextureResource))
                return false;

            // Reset the world matrix.
            worldMatrix = D3D.WorldMatrix;

            // Translate to the position of the ground model.
            Vector3 groundPosition = GroundModel.GetPosition();
            Matrix.Translation(groundPosition.X, groundPosition.Y, groundPosition.Z, out worldMatrix);

            // Render the ground model with the depth shader.
            GroundModel.Render(D3D.DeviceContext);
            if (!DepthShader.Render(D3D.DeviceContext, GroundModel.IndexCount, worldMatrix, lightViewMatrix, lightOrthoMatrix))
                return false;

            // Reset the render target back to the original back buffer and not the render to texture anymore.
            D3D.SetBackBufferRenderTarget();

            // Reset the viewport back to the original.
            D3D.ResetViewPort();

            return true;
        }
    }
}