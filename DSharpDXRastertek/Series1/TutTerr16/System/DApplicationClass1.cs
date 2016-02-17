using DSharpDXRastertek.TutTerr16.Graphics;
using DSharpDXRastertek.TutTerr16.Graphics.Camera;
using DSharpDXRastertek.TutTerr16.Graphics.Data;
using DSharpDXRastertek.TutTerr16.Graphics.Input;
using DSharpDXRastertek.TutTerr16.Graphics.Models;
using DSharpDXRastertek.TutTerr16.Graphics.Shaders;
using SharpDX;
using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.TutTerr16.System
{
    public class DApplication                   // 515 lines
    {
        // Properties
        public DInput Input { get; private set; }
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }
        public DPosition Position { get; set; }
        public DLight Light { get; set; }
        public DRenderTexture RefractionTexture { get; set; }
        public DRenderTexture ReflectionTexture { get; set; }

        #region Models
        public DTerrainHeightMap TerrainModel { get; set; }
        public DSkyDome SkyDome { get; set; }
        public DSkyPlane SkyPlane { get; set; }
        public DWater WaterModel { get; set; }
        #endregion

        #region Shaders
        public DTerrainShader TerrainShader { get; set; }
        public DSkyDomeShader SkyDomeShader { get; set; }
        public DSkyPlaneShader SkyPlaneShader { get; set; }
        public DReflectionShader ReflectionShader { get; set; }
        public DWaterShader WaterShader { get; set; }
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

                // Create the position object.
                Position = new DPosition();

                // Set the initial position and rotation of the viewer.
                Position.SetPosition(280.379f, 24.5225f, 367.018f);
                Position.SetRotation(19.6834f, 222.013f, 0.0f);

                // Create the camera object
                Camera = new DCamera();

                // Initialize a base view matrix with the camera for 2D user interface rendering.
                Camera.SetPosition(0.0f, 0.0f, -10.0f);
                Camera.RenderBaseViewMatrix();
                Matrix baseViewMatrix = Camera.BaseViewMatrix;

                // Create the light object.
                Light = new DLight();

                // Initialize the light object.
                Light.SetDiffuseColor(1.0f, 1.0f, 1.0f, 1.0f);
                Light.Direction = new Vector3(0.5f, -0.75f, 0.25f);

                // Create the model object.
                TerrainModel = new DTerrainHeightMap();

                // Initialize the terrain object.
                if (!TerrainModel.Initialize(D3D.Device, "hm.bmp", "cm.bmp", 20.0f, "dirt04.dds", "normal01.dds"))
                    return false;

                // Create the color shader object.
                TerrainShader = new DTerrainShader();

                //// Initialize the color shader object.
                if (!TerrainShader.Initialize(D3D.Device, windowHandle))
                    return false;

                // Create the sky dome object.
                SkyDome = new DSkyDome();

                // Initialize the sky dome object.
                if (!SkyDome.Initialize(D3D.Device))
                    return false;

                // Create the sky dome shader object.
                SkyDomeShader = new DSkyDomeShader();

                // Initialize the sky dome shader object.
                if (!SkyDomeShader.Initialize(D3D.Device, windowHandle))
                    return false;

                // Create the sky plane object.
                SkyPlane = new DSkyPlane();

                // Initialize the sky plane object.
                if (!SkyPlane.Initialze(D3D.Device, "cloud001.dds", "perturb001.dds"))
                    return false;

                // Create the sky plane shader object.
                SkyPlaneShader = new DSkyPlaneShader();

                // Initialize the sky plane shader object.
                if (!SkyPlaneShader.Initialize(D3D.Device, windowHandle))
                    return false;

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

                // Create the refraction render to texture object.
                RefractionTexture = new DRenderTexture();

                // Initialize the refraction render to texture object.
                if (!RefractionTexture.Initialize(D3D.Device, configuration))
                    return false;

                // Create the reflection render to texture object.
                ReflectionTexture = new DRenderTexture();

                // Initialize the reflection render to texture object.
                if (!ReflectionTexture.Initialize(D3D.Device, configuration))
                    return false;

                // Create the reflection shader object.
                ReflectionShader = new DReflectionShader();

                // Initialize the reflection shader object.
                if (!ReflectionShader.Initialize(D3D.Device, windowHandle))
                    return false;

                // Create the water object.
                WaterModel = new DWater();

                // Initialize the water object.
                if (!WaterModel.Initilize(D3D.Device, "waternormal.dds", 3.75f, 110.0f))
                    return false;

                // Create the water shader object.
                WaterShader = new DWaterShader();

                // Initialize the water shader object.
                if (!WaterShader.Initialize(D3D.Device, windowHandle))
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

            // Release the text object.
            Text?.Shutdown();
            Text = null;
            // Release the cpu object.
            CPU?.Shutdown();
            CPU = null;
            // Release the water shader object.
            WaterShader?.ShutDown();
            WaterShader = null;
            // Release the water object.
            WaterModel?.ShutDown();
            WaterModel = null;
            // Release the reflection shader object.
            ReflectionShader?.ShutDown();
            ReflectionShader = null;
            // Release the reflection render to texture object.
            ReflectionTexture?.Shutdown();
            ReflectionTexture = null;
            // Release the refraction render to texture object.
            RefractionTexture?.Shutdown();
            RefractionTexture = null;
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

            // Do the water frame processing.
            WaterModel.Frame(); 

            // Do the sky plane frame processing.
            SkyPlane.Frame();  

            // Render the refraction of the scene to a texture.
            if (!RenderRefractionToTexture())
                return false;

            // Render the reflection of the scene to a texture.
            if (!RenderReflectionToTexture())
                return false;

            // Render the graphics.
            if (!Render())
                return false;

            return true;
        }
        private bool RenderRefractionToTexture()
        {
            // Setup a clipping plane based on the height of the water to clip everything above it to create a refraction.
            Vector4 clipPlane = new Vector4(0.0f, -1.0f, 0.0f, WaterModel.WaterHeight + 0.1f);

            // Set the render target to be the refraction render to texture.
            RefractionTexture.SetRenderTarget(D3D.DeviceContext);

            // Clear the refraction render to texture.
            RefractionTexture.ClearRenderTarget(D3D.DeviceContext, 0.0f, 0.0f, 0.0f, 1.0f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the matrices from the camera and d3d objects.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix viewMatrix = Camera.ViewMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;

            // Render the terrain using the reflection shader and the refraction clip plane to produce the refraction effect.
            TerrainModel.Render(D3D.DeviceContext);
            if (!ReflectionShader.Render(D3D.DeviceContext, TerrainModel.IndexCount, worldMatrix, viewMatrix, projectionMatrix, TerrainModel.ColorTexture.TextureResource, TerrainModel.NormalMapTexture.TextureResource, Light.DiffuseColour, Light.Direction, 2.0f, clipPlane))
                return false;

            // Reset the render target back to the original back buffer and not the render to texture anymore.
            D3D.SetBackBufferRenderTarget();

            // Reset the viewport back to the original.
            D3D.ResetViewPort();

            return true;
        }
        private bool RenderReflectionToTexture()
        {
            // Setup a clipping plane based on the height of the water to clip everything below it.
            Vector4 clipPlane = new Vector4(0.0f, 1.0f, 0.0f, -WaterModel.WaterHeight);

            // Set the render target to be the reflection render to texture.
            ReflectionTexture.SetRenderTarget(D3D.DeviceContext);

            // Clear the reflection render to texture.
            ReflectionTexture.ClearRenderTarget(D3D.DeviceContext, 0.0f, 0.0f, 0.0f, 1.0f);

            // Use the camera to render the reflection and create a reflection view matrix.
            Camera.RenderReflection(WaterModel.WaterHeight);

            // Get the camera reflection view matrix instead of the normal view matrix.
            Matrix reflectionViewMatrix = Camera.ReflectionViewMatrix;

            // Get the world and projection matrices from the d3d object.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;

            // Get the position of the camera.
            Vector3 cameraPosition = Camera.GetPosition();

            // Invert the Y coordinate of the camera around the water plane height for the reflected camera position.
            cameraPosition.Y = -cameraPosition.Y + (WaterModel.WaterHeight * 2.0f);

            // Translate the sky dome and sky plane to be centered around the reflected camera position.
            Matrix.Translation(cameraPosition.X, cameraPosition.Y, cameraPosition.Z, out worldMatrix);

            // Turn off back face culling and the Z buffer.
            D3D.TurnOffCulling();
            D3D.TurnZBufferOff();

            // Render the sky dome using the reflection view matrix.
            SkyDome.Render(D3D.DeviceContext);
            if (!SkyDomeShader.Render(D3D.DeviceContext, SkyDome.IndexCount, worldMatrix, reflectionViewMatrix, projectionMatrix, SkyDome.ApexColour, SkyDome.CenterColour))
                return false;

            // Enable back face culling.
            D3D.TurnOnCulling();

            // Enable additive blending so the clouds blend with the sky dome color.
            D3D.EnableSecondBlendState();

            // Render the sky plane using the sky plane shader.
            SkyPlane.Render(D3D.DeviceContext);
            if(!SkyPlaneShader.Render(D3D.DeviceContext, SkyPlane.IndexCount, worldMatrix, reflectionViewMatrix, projectionMatrix, SkyPlane.CloudTexture.TextureResource, SkyPlane.PerturbTexture.TextureResource, SkyPlane.m_Translation, SkyPlane.m_Scale, SkyPlane.m_Brightness ))
                return false;

            // Turn off blending and enable the Z buffer again.
            D3D.TurnOffAlphaBlending();
            D3D.TurnZBufferOn();

            // Reset the world matrix.
            worldMatrix = D3D.WorldMatrix;

            // Render the terrain using the reflection view matrix and reflection clip plane.
            TerrainModel.Render(D3D.DeviceContext);
            if (!ReflectionShader.Render(D3D.DeviceContext, TerrainModel.IndexCount, worldMatrix, reflectionViewMatrix, projectionMatrix, TerrainModel.ColorTexture.TextureResource, TerrainModel.NormalMapTexture.TextureResource, Light.DiffuseColour, Light.Direction, 2.0f, clipPlane))
                return false;

            // Reset the render target back to the original back buffer and not the render to texture anymore.
            D3D.SetBackBufferRenderTarget();

            // Reset the viewport back to the original.
            D3D.ResetViewPort();

            return true;
        }
        private bool Render()
        {
            // Clear the scene.
            D3D.BeginScene( 0.0f, 0.0f, 0.0f, 1.0f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world, view, projection, ortho, base view and reflection matrices from the camera and Direct3D objects.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix viewCameraMatrix = Camera.ViewMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;
            Matrix orthoMatrix = D3D.OrthoMatrix;
            Matrix baseViewMatrix = Camera.BaseViewMatrix;
            Matrix reflectionMatrix = Camera.ReflectionViewMatrix;

            // Get the position of the camera.
            Vector3 cameraPosition = Camera.GetPosition();

            // Translate the sky dome to be centered around the camera position.
            Matrix.Translation(cameraPosition.X, cameraPosition.Y, cameraPosition.Z, out worldMatrix);

            // Turn off back face culling and the Z buffer.
            D3D.TurnOffCulling();
            D3D.TurnZBufferOff();

            // Render the sky dome using the sky dome shader.
            SkyDome.Render(D3D.DeviceContext);
            if (!SkyDomeShader.Render(D3D.DeviceContext, SkyDome.IndexCount, worldMatrix, viewCameraMatrix, projectionMatrix, SkyDome.ApexColour, SkyDome.CenterColour))
                return false;

            // Turn back face culling back on.
            D3D.TurnOnCulling();

            // Enable additive blending so the clouds blend with the sky dome color.
            D3D.EnableSecondBlendState();

            // Render the sky plane using the sky plane shader.
            SkyPlane.Render(D3D.DeviceContext);
            if (!SkyPlaneShader.Render(D3D.DeviceContext, SkyPlane.IndexCount, worldMatrix, viewCameraMatrix, projectionMatrix, SkyPlane.CloudTexture.TextureResource, SkyPlane.PerturbTexture.TextureResource, SkyPlane.m_Translation, SkyPlane.m_Scale, SkyPlane.m_Brightness))
                return false;

            // Turn off blending.
            D3D.TurnOffAlphaBlending();

            // Turn the Z buffer back on.
            D3D.TurnZBufferOn();

            // Reset the world matrix.
            worldMatrix = D3D.WorldMatrix;

            // Render the terrain using the terrain shader.
            TerrainModel.Render(D3D.DeviceContext);
            if (!TerrainShader.Render(D3D.DeviceContext, TerrainModel.IndexCount, worldMatrix, viewCameraMatrix, projectionMatrix, TerrainModel.ColorTexture.TextureResource, TerrainModel.NormalMapTexture.TextureResource, Light.DiffuseColour, Light.Direction, 2.0f))
                return false;

            // Translate to the location of the water and render it.
            Matrix.Translation(240.0f, WaterModel.WaterHeight, 250.0f, out worldMatrix);
            WaterModel.Render(D3D.DeviceContext);
            if (!WaterShader.Render(D3D.DeviceContext, WaterModel.IndexCount, worldMatrix, viewCameraMatrix, projectionMatrix, reflectionMatrix, ReflectionTexture.ShaderResourceView, RefractionTexture.ShaderResourceView, WaterModel.Texture.TextureResource, Camera.GetPosition(), WaterModel.NormalMapTiling, WaterModel.WaterTranslation, WaterModel.ReflectRefractScale, WaterModel.RefractionTint, Light.Direction, WaterModel.SpecularShininess))
                return false;

            // Reset the world matrix.
            worldMatrix = D3D.WorldMatrix;

            // Turn off the Z buffer to begin all 2D rendering.
            D3D.TurnZBufferOff();

            // Turn on the alpha blending before rendering the text.
            D3D.TurnOnAlphaBlending();

            // Render the text user interface elements.
            Text.Render(D3D.DeviceContext, worldMatrix, orthoMatrix);

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