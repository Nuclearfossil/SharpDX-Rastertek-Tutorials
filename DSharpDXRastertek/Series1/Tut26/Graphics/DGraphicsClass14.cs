using DSharpDXRastertek.Tut26.Graphics.Camera;
using DSharpDXRastertek.Tut26.Graphics.Models;
using DSharpDXRastertek.Tut26.Graphics.Shaders;
using DSharpDXRastertek.Tut26.System;
using SharpDX;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut26.Graphics
{
    public class DGraphics                  // 189 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }

        #region Models
        private DModel Model { get; set; }
        private DModel Model2 { get; set; }
        #endregion

        #region Shaders
        private DTransparentShader TransparentShader { get; set; }
        private DTextureShader TextureShader { get; set; }
        #endregion

        // Static properties
        public static float Rotation { get; set; }
        static float TextureTranslation { get; set; }

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

                // Set the position of the camera;
                Camera.SetPosition(0, 0, -5.0f);
                #endregion

                #region Initialize Models
                // Create the model class.
                Model = new DModel();

                // Initialize the model object.
                if (!Model.Initialize(D3D.Device, "square.txt", new[] { "dirt01.bmp" }))
                {
                    MessageBox.Show("Could not initialize the model object", "Error", MessageBoxButtons.OK);
                    return false;
                }

                // Create the model class.
                Model2 = new DModel();

                // Initialize the model object.
                if (!Model2.Initialize(D3D.Device, "square.txt", new[] { "stone01.bmp" }))
                {
                    MessageBox.Show("Could not initialize the model object", "Error", MessageBoxButtons.OK);
                    return false;
                }
                #endregion

                #region Initialize Textures
                // Create the shader object.
                TextureShader = new DTextureShader();

                // Initialize the shader object.
                if (!TextureShader.Initialize(D3D.Device, windowHandle))
                {
                    MessageBox.Show("Could not initialize the shader", "Error", MessageBoxButtons.OK);
                    return false;
                }

                // Create the shader object.
                TransparentShader = new DTransparentShader();

                // Initialize the shader object.
                if (!TransparentShader.Initialize(D3D.Device, windowHandle))
                {
                    MessageBox.Show("Could not initialize the shader", "Error", MessageBoxButtons.OK);
                    return false;
                }
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

            // Release the transparent shader object.
            TransparentShader?.ShutDown();
            TransparentShader = null;
            // Release the texture shader object.
            TextureShader?.ShutDown();
            TextureShader = null;
            //// Release the model object.
            Model?.Shutdown();
            Model = null;
            Model2?.Shutdown();
            Model2 = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Frame()
        {
            return true;
        }
        public bool Render()
        {
            // Clear the buffer to begin the scene as Black.
            D3D.BeginScene(0, 0, 0, 1f);

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

            // Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            Model.Render(D3D.DeviceContext);

            // Render the model using the color shader.
            if (!TextureShader.Render(D3D.DeviceContext, Model.IndexCount, worldMatrix, viewMatrix, projectionMatrix, Model.TextureCollection.Select(item => item.TextureResource).First()))
                return false;

            // Translate to the right by one unit and towards the camera by one unit.
            Matrix.Translation(1, 0, -1, out worldMatrix);

            // Setup a BlendAmount for Transparency..
            var blendAmount = 0.5f;

            // Turn on alpha blending for the transparency to work.
            D3D.TurnOnAlphaBlending();

            // Put the second square model on the graphics pipeline.
            Model2.Render(D3D.DeviceContext);

            // Render the model using the color shader.
            if (!TransparentShader.Render(D3D.DeviceContext, Model2.IndexCount, worldMatrix, viewMatrix, projectionMatrix, Model2.TextureCollection.Select(item => item.TextureResource).ToArray(), blendAmount))
                return false;

            // Turn off alpha blending.
            D3D.TurnOffAlphaBlending();

            return true;
        }
    }
}