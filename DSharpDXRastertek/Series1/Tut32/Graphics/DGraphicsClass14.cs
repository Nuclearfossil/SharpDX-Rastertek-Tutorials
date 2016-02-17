using DSharpDXRastertek.Tut32.Graphics.Data;
using DSharpDXRastertek.Tut32.Graphics.Models;
using DSharpDXRastertek.Tut32.Graphics.Shaders;
using DSharpDXRastertek.Tut32.System;
using SharpDX;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut32.Graphics
{
    public class DGraphics                  // 241 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }

        #region Data
        private DRenderTexture RenderTexture { get; set; }
        #endregion

        #region Models
        private DModel Model { get; set; }
        private DModel WindowModel { get; set; }
        #endregion

        #region Shaders
        public DGlassShader GlassShader { get; set; }
        private DTextureShader TextureShader { get; set; }
        #endregion     

        // Static properties
        public static float Rotation { get; set; }

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

                // Set the position of the camera.
                Camera.SetPosition(0.0f, 0.0f, -5.0f);
                #endregion

                #region Initialize Models
                // Create the Flat Plane  model class.
                Model = new DModel();

                // Initialize the ground model object.
                if (!Model.Initialize(D3D.Device, "cube.txt", new[] { "seafloor.dds", "bump03.dds" }))
                {
                    MessageBox.Show("Could not initialize the ground model object", "Error", MessageBoxButtons.OK);
                    return false;
                }

                // Create the wall model class.
                WindowModel = new DModel();

                // Initialize the wall model object.    "lens2.txt"   "triangle.txt"
                if (!WindowModel.Initialize(D3D.Device, "square.txt", new[] { "glass01.dds", "bump03.dds" }))
                {
                    MessageBox.Show("Could not initialize the wall model object", "Error", MessageBoxButtons.OK);
                    return false;
                }

                // Create the render to texture object.
                RenderTexture = new DRenderTexture();

                // Initialize the render to texture object.
                RenderTexture.Initialize(D3D.Device, configuration);
                #endregion

                #region Initialize Shaders
                // Create the texture shader object.
                TextureShader = new DTextureShader();

                // Initialize the texture shader object.
                TextureShader.Initialize(D3D.Device, windowHandle);

                // Create the glass shader object.
                GlassShader = new DGlassShader();

                // Initialize the glass shader object.
                GlassShader.Initialize(D3D.Device, windowHandle);
                #endregion

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
            // Release the camera object.
            Camera = null;

            // Release the GlassShader object.
            GlassShader?.ShutDown();
            GlassShader = null;
            // Release the texture shader object.
            TextureShader?.ShutDown();
            TextureShader = null;
            // Release the render to texture object.
            RenderTexture?.Shutdown();
            RenderTexture = null;
            // Release the model object.
            Model?.Shutdown();
            Model = null;
            // Release the model object.
            WindowModel?.Shutdown();
            WindowModel = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Frame()
        {
            Rotate();

            // First we render the 3D scene to a texture so the glass shader will have a refraction texture as input.
            // Render the scene to texture first.
            if (!RenderToTexture(Rotation))
                return false;

            // Then we render the scene again normally and render the glass over top of it with the perturbed and colored refraction texture rendered on the glass model.
            // Render the scene.
            if (!Render(Rotation))
                return false;

            return true;
        }

        private bool RenderToTexture(float rotation)
        {
            Matrix worldMatrix, viewMatrix, projectionMatrix;

            // Set the render target to be the render to texture.
            RenderTexture.SetRenderTarget(D3D.DeviceContext, D3D.DepthStencilView);

            // Clear the render to texture.
            RenderTexture.ClearRenderTarget(D3D.DeviceContext, D3D.DepthStencilView, 0.0f, 0.0f, 0.0f, 1.0f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world, view, and projection matrices from the camera and d3d objects.
            worldMatrix = D3D.WorldMatrix;
            viewMatrix = Camera.ViewMatrix;
            projectionMatrix = D3D.ProjectionMatrix;

            // Multiply the world matrix by the rotation.
            Matrix.RotationY(rotation, out worldMatrix);
            // worldMatrix *= Matrix.Translation(0.0f, 0.0f, 1.0f);

            // Put the cube model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            Model.Render(D3D.DeviceContext);

            // Render the cube model using the texture shader.
            if (!TextureShader.Render(D3D.DeviceContext, Model.IndexCount, worldMatrix, viewMatrix, projectionMatrix, Model.TextureCollection.Select(item => item.TextureResource).First()))
                return false;

            // Reset the render target back to the original back buffer and not the render to texture anymore.
            D3D.SetBackBufferRenderTarget();

            return true;
        }
        public bool Render(float rotation)
        {
            Matrix worldMatrix, viewMatrix, projectionMatrix;
            float refractionScale;

            // First set the refraction scale to modify how much perturbation occurs in the glass.
            // Set the refraction scale for the glass shader.
            refractionScale = 0.01f;

            // Clear the buffers to begin the scene.
            D3D.BeginScene(0.0f, 0.0f, 0.0f, 1.0f);

            // Get the world, view, and projection matrices from the camera and d3d objects.
            worldMatrix = D3D.WorldMatrix;
            viewMatrix = Camera.ViewMatrix;
            projectionMatrix = D3D.ProjectionMatrix;

            // Then render the 3D spinning cube scene as normal. 
            // Multiply the world matrix by the rotation.
            Matrix.RotationY(rotation, out worldMatrix);

            // Put the cube model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            Model.Render(D3D.DeviceContext);

            // Render the cube model using the texture shader.
            if (!TextureShader.Render(D3D.DeviceContext, Model.IndexCount, worldMatrix, viewMatrix, projectionMatrix, Model.TextureCollection.Select(item => item.TextureResource).First()))
                return false;

            // Reset the world matrix.
            worldMatrix = D3D.WorldMatrix;

            // Now render the window model using the glass shader with the color texture, normal map, refraction render to texture, and refraction scale as input.
            // Translate to back where the window model will be rendered.
            Matrix.Translation(0.0f, 0.0f, -1.5f, out worldMatrix);

            // // Put the window model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            WindowModel.Render(D3D.DeviceContext);

            // Render the window model using the glass shader.
            if (!GlassShader.Render(D3D.DeviceContext, WindowModel.IndexCount, worldMatrix, viewMatrix, projectionMatrix, WindowModel.TextureCollection.Select(item => item.TextureResource).First(), WindowModel.TextureCollection.Select(item => item.TextureResource).ToArray()[1], RenderTexture.ShaderResourceView, refractionScale))
                return false;

            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }

        // Static Methods.
        static void Rotate()
        {
            Rotation += (float)Math.PI * 0.002f; //  0.005f;
            if (Rotation > 360)
                Rotation -= 360;
        }
    }
}