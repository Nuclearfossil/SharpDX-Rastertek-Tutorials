using DSharpDXRastertek.Tut29.Graphics.Camera;
using DSharpDXRastertek.Tut29.Graphics.Data;
using DSharpDXRastertek.Tut29.Graphics.Models;
using DSharpDXRastertek.Tut29.Graphics.Shaders;
using DSharpDXRastertek.Tut29.System;
using SharpDX;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut29.Graphics
{
    public class DGraphics                  // 416 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }

        #region Data
        private DLight Light { get; set; }
        private DRenderTexture RenderReflectionTexture { get; set; }
        private DRenderTexture RenderRefractionTexture { get; set; }
        #endregion

        #region Models
        private DModel GroundModel { get; set; }
        private DModel WallModel { get; set; }
        private DModel BathModel { get; set; }
        private DModel WaterModel { get; set; }
        #endregion

        #region Shaders
        private DRefractionShader RefractionShader { get; set; }
        private DWaterShader WaterShader { get; set; }
        private DLightShader LightShader { get; set; }
        #endregion     

        #region Variables
        private float FadeInTime { get; set; }
        private float AccumulatedTime { get; set; }
        private float FadePercentage { get; set; }
        private bool FadeDone { get; set; }
        private float WaterHeight { get; set; }
        private float WaterTranslation { get; set; }
        #endregion

        // Construtor
        public DGraphics() { }

        // Methods.
        public bool Initialize(DSystemConfiguration configuration, IntPtr windowHandle)
        {
            try
            {
                #region Initialize System
                // Create the Direct3D object.
                D3D = new DDX11();
                
                // Initialize the Direct3D object.
                if (!D3D.Initialize(configuration, windowHandle))
                    return false;
                #endregion

                #region Initialize Camera
                // Create the camera object
                Camera = new DCamera();

                // Set the position and rotation of the camera;
                Camera.SetPosition(-10.0f, 6.0f, -10.0f);
                Camera.SetRotation(0.0f, 45.0f, 0.0f);
                #endregion

                #region Initialize Models
                // Create the ground model class.
                GroundModel = new DModel();

                // Initialize the ground model object.
                if (!GroundModel.Initialize(D3D.Device, "ground.txt", new[] { "ground01.bmp" }))
                {
                    MessageBox.Show("Could not initialize the ground model object", "Error", MessageBoxButtons.OK);
                    return false;
                }

                // Create the wall model class.
                WallModel = new DModel();

                // Initialize the wall model object.
                if (!WallModel.Initialize(D3D.Device, "wall.txt", new[] { "wall01.bmp" }))
                {
                    MessageBox.Show("Could not initialize the wall model object", "Error", MessageBoxButtons.OK);
                    return false;
                }

                // Create the bath model class.
				BathModel = new DModel();

				// Initialize the bath model object.
				if (!BathModel.Initialize(D3D.Device, "bath.txt", new[] { "marble01.bmp" }))
				{
					MessageBox.Show("Could not initialize the bath model object", "Error", MessageBoxButtons.OK);
					return false;
				}

                // Create the water model class.
                WaterModel = new DModel();

                // Initialize the water model object.
                if (!WaterModel.Initialize(D3D.Device, "water.txt", new[] { "water01.bmp" }))
                {
                    MessageBox.Show("Could not initialize the bath model object", "Error", MessageBoxButtons.OK);
                    return false;
                }

                // Create the light object.
                Light = new DLight();

                // Initialize the light object.
                Light.SetAmbientColor(0.15f, 0.15f, 0.15f, 1.0f);
                Light.SetDiffuseColor(1.0f, 1.0f, 1.0f, 1.0f);
                Light.SetDirection(0.0f, -1.0f, 0.5f);
                Light.SetSpecularColor(0, 1, 1, 1);
                Light.SetSpecularPower(16);
                #endregion

                #region Initialize Data
                // Create the refraction render to texture object.
                RenderRefractionTexture = new DRenderTexture();

                // Initialize the refraction render to texture object.
                if (!RenderRefractionTexture.Initialize(D3D.Device, configuration))
                {
                    MessageBox.Show("Could not initialize the refraction render to texture object.", "Error", MessageBoxButtons.OK);
                    return false;
                }

                // Create the refraction render to texture object.
                RenderReflectionTexture = new DRenderTexture();

                // Initialize the refraction render to texture object.
                if (!RenderReflectionTexture.Initialize(D3D.Device, configuration))
                {
                    MessageBox.Show("Could not initialize the reflection render to texture object.", "Error", MessageBoxButtons.OK);
                    return false;
                }
                #endregion
          
                #region Initialize Shaders     
                // Create the light shader object.
                LightShader = new DLightShader();
                
                // Initialize the light shader object.
                if (!LightShader.Initialize(D3D.Device, windowHandle))
                {
                    MessageBox.Show("Could not initialize the light shader object.", "Error", MessageBoxButtons.OK);
                    return false;
                }

                // Create the refraction shader object.
                RefractionShader = new DRefractionShader();
                
                // Initialize the refraction shader object.
                if (!RefractionShader.Initialize(D3D.Device, windowHandle))
                {
                    MessageBox.Show("Could not initialize the refraction shader object.", "Error", MessageBoxButtons.OK);
                    return false;
                }

                // Create the water shader object.
                WaterShader = new DWaterShader();
                
                // Initialize the water shader object.
                if (!WaterShader.Initialize(D3D.Device, windowHandle))
                {
                    MessageBox.Show("Could not initialize the water shader object.", "Error", MessageBoxButtons.OK);
                    return false;
                }
                #endregion

                // Set the height of the water.
                WaterHeight = 2.75f;
                // Initialize the position of the water.
                WaterTranslation = 0f;
                
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
            // Release the light object.
            Light = null;
            // Release the camera object.
            Camera = null;

            // Release the water shader object.
            WaterShader?.ShutDown();
            WaterShader = null;
            // Release the refraction shader object.
            RefractionShader?.ShutDown();
            RefractionShader = null;
            /// Release the render to texture object.
            RenderReflectionTexture?.Shutdown();
            RenderReflectionTexture = null;
            // Release the render to texture object.
            RenderRefractionTexture?.Shutdown();
            RenderRefractionTexture = null;
            // Release the light shader object.
            LightShader?.ShutDown();
            LightShader = null;
            // Release the model object.
            GroundModel?.Shutdown();
            GroundModel = null;
            // Release the model object.
            WallModel?.Shutdown();
            WallModel = null;
            // Release the model object.
            BathModel?.Shutdown();
            BathModel = null;
            // Release the model object.
            WaterModel?.Shutdown();
            WaterModel = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Frame()
        {
            // Update the position of the water to simulate motion.
            WaterTranslation += 0.001f;
            if (WaterTranslation > 1.0f)
                WaterTranslation -= 1.0f;

            return true;
        }
        public bool Render()
        {
            // Clear the buffer to begin the scene.
            D3D.BeginScene(0, 0, 0, 1f);

            // Render the refraction of the scene to a texture.
            if (!RenderRefractionToTexture())
                return false;

            // Render the reflection of the scene to a texture.
            if (!RenderReflectionToTexture())
                return false;

            // Render the scene as normal to the back buffer.
            if (!RenderScene())
                return false;

            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }
        private bool RenderScene()
        {
            // Generate the view matrix based on the camera position.
            Camera.Render();

            // Get the world, view, and projection matrices from camera and d3d objects.
            var viewMatrix = Camera.ViewMatrix;
            var worldMatrix = D3D.WorldMatrix;
            var projectionMatrix = D3D.ProjectionMatrix;

            #region Render Ground Model
            // Translate to where the ground model will be rendered.
            Matrix.Translation(0f, 1f, 0f, out worldMatrix);

            // Put the ground model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            GroundModel.Render(D3D.DeviceContext);

            // Render the ground model using the light shader.
            if (!LightShader.Render(D3D.DeviceContext, GroundModel.IndexCount, worldMatrix, viewMatrix, projectionMatrix, GroundModel.TextureCollection.Select(item => item.TextureResource).First(), Light.Direction, Light.AmbientColor, Light.DiffuseColour, Camera.GetPosition(), Light.SpecularColor, Light.SpecularPower))
                return false;
            #endregion

            // Reset the world matrix.
            worldMatrix = D3D.WorldMatrix;

            #region Render Wall Model
            // Translate to where the ground model will be rendered.
            Matrix.Translation(0f, 6f, 8f, out worldMatrix);

            // Put the wall model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            WallModel.Render(D3D.DeviceContext);

            // Render the wall model using the light shader.
            if (!LightShader.Render(D3D.DeviceContext, WallModel.IndexCount, worldMatrix, viewMatrix, projectionMatrix, WallModel.TextureCollection.Select(item => item.TextureResource).First(), Light.Direction, Light.AmbientColor, Light.DiffuseColour, Camera.GetPosition(), Light.SpecularColor, Light.SpecularPower))
                return false;
            #endregion

            // Reset the world matrix.
            worldMatrix = D3D.WorldMatrix;

            #region Render Bath Model
            // Translate to where the bath model will be rendered.
            Matrix.Translation(0f, 2f, 0f, out worldMatrix);

            // Put the bath model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            BathModel.Render(D3D.DeviceContext);

            // Render the bath model using the light shader.
            if (!LightShader.Render(D3D.DeviceContext, BathModel.IndexCount, worldMatrix, viewMatrix, projectionMatrix, BathModel.TextureCollection.Select(item => item.TextureResource).First(), Light.Direction, Light.AmbientColor, Light.DiffuseColour, Camera.GetPosition(), Light.SpecularColor, Light.SpecularPower))
                return false;
            #endregion

            // Reset the world matrix.
            worldMatrix = D3D.WorldMatrix;

            #region Render Water Model
            // Get the camera reflection view matrix.
            var reflectionMatrix = Camera.ReflectionViewMatrix;

            // Translate to where the water model will be rendered.
            Matrix.Translation(0f, WaterHeight, 0f, out worldMatrix);

            // Put the water model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            WaterModel.Render(D3D.DeviceContext);

            // Render the bath model using the light shader.
            if (!WaterShader.Render(D3D.DeviceContext, WaterModel.IndexCount, worldMatrix, viewMatrix, projectionMatrix, reflectionMatrix, RenderReflectionTexture.ShaderResourceView, RenderRefractionTexture.ShaderResourceView, WaterModel.TextureCollection.Select(item => item.TextureResource).First(), WaterTranslation, 0.1f)) // was 0.01f for scale originally.
                return false;
            #endregion

            return true;
        }
        private bool RenderReflectionToTexture()
        {
            // Set the render target to be the refraction render to texture.
            RenderReflectionTexture.SetRenderTarget(D3D.DeviceContext, D3D.DepthStencilView);
            
            // Clear the render to texture.
            RenderReflectionTexture.ClearRenderTarget(D3D.DeviceContext, D3D.DepthStencilView, 0, 0, 0, 1);

            // Render the scene now and it will draw to the render to texture instead of the back buffer.
            if (!RenderReflectionScene())
                return false;

            // Reset the render target back to the original back buffer and not to texture anymore.
            D3D.SetBackBufferRenderTarget();

            return true;
        }
        private bool RenderReflectionScene()
        {
            // Use the camera to render the reflection and create a reflection view matrix.
            Camera.RenderReflection(WaterHeight);

            // Get the camera reflection view matrix instead of the normal view matrix.
            var viewMatrix = Camera.ReflectionViewMatrix;
            // Get the world and projection matrices from the d3d object.
            var worldMatrix = D3D.WorldMatrix;
            var projectionMatrix = D3D.ProjectionMatrix;

            // Translate to where the bath model will be rendered.
            Matrix.Translation(0f, 6f, 8f, out worldMatrix);

            // Put the wall model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            WallModel.Render(D3D.DeviceContext);

            // Render the wall model using the light shader and the reflection view matrix.
            if (!LightShader.Render(D3D.DeviceContext, WallModel.IndexCount, worldMatrix, viewMatrix, projectionMatrix, WallModel.TextureCollection.Select(item => item.TextureResource).First(), Light.Direction, Light.AmbientColor, Light.DiffuseColour, Camera.GetPosition(), Light.SpecularColor, Light.SpecularPower))
                return false;

            return true;
        }
        private bool RenderRefractionToTexture()
        {
            // Set the render target to be the refraction render to texture.
            RenderRefractionTexture.SetRenderTarget(D3D.DeviceContext, D3D.DepthStencilView);
            
            // Clear the render to texture.
            RenderRefractionTexture.ClearRenderTarget(D3D.DeviceContext, D3D.DepthStencilView, 0, 0, 0, 1);

            // Render the scene now and it will draw to the render to texture instead of the back buffer.
            if (!RenderRefractionScene())
                return false;

            // Reset the render target back to the original back buffer and not to texture anymore.
            D3D.SetBackBufferRenderTarget();

            return true;
        }
        private bool RenderRefractionScene()
        {
            // Setup a clipping plane based on the height of the water to clip everything above it.
            var clipPlane = new Vector4(0f, -1f, 0f, WaterHeight + 0.1f);

            // Generate the view matrix based on the camera position.
            Camera.Render();

            // Get the world, view, and projection matrices from camera and d3d objects.
            var viewMatrix = Camera.ViewMatrix;
            var worldMatrix = D3D.WorldMatrix;
            var projectionMatrix = D3D.ProjectionMatrix;

            // Translate to where the bath model will be rendered.
            Matrix.Translation(0f, 2f, 0f, out worldMatrix);

            // Put the bath model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            BathModel.Render(D3D.DeviceContext);

            // Render the bath model using the light shader.
            if (!RefractionShader.Render(D3D.DeviceContext, BathModel.IndexCount, worldMatrix, viewMatrix, projectionMatrix, BathModel.TextureCollection.Select(item => item.TextureResource).First(), Light.Direction, Light.AmbientColor, Light.DiffuseColour, clipPlane))
                return false;

            return true;
        }
    }
}