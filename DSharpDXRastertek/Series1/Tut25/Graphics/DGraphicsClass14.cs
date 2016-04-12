using DSharpDXRastertek.Tut25.Graphics.Camera;
using DSharpDXRastertek.Tut25.Graphics.Models;
using DSharpDXRastertek.Tut25.Graphics.Shaders;
using DSharpDXRastertek.Tut25.System;
using SharpDX;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut25.Graphics
{
    public class DGraphics                  // 129 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }

        #region Models
        private DModel Model { get; set; }
        #endregion

        #region Shaders
        private DTranslateShader TranslateShader { get; set; }
        #endregion

        // Static properties
        static float TextureTranslation { get; set; }

        // Construtor
        public DGraphics() { }

        // Methods.
        public bool Initialize(DSystemConfiguration configuration, IntPtr windowHandle)
        {
            try
            {
                // Create the Direct3D object.
                D3D = new DDX11();
                
                // Initialize the Direct3D object.
                if (!D3D.Initialize(configuration, windowHandle))
                    return false;

                // Create the camera object
                Camera = new DCamera();
 
                // Create the model class.
                Model = new DModel();

                // Initialize the model object.
                if (!Model.Initialize(D3D.Device, "triangle.txt", new[] { "seafloor.bmp" }))
                {
                    MessageBox.Show("Could not initialize the model object", "Error", MessageBoxButtons.OK);
                    return false;
                }
               
                // Create the shader object.
                TranslateShader = new DTranslateShader();

                // Initialize the shader object.
                if (!TranslateShader.Initialize(D3D.Device, windowHandle))
                {
                    MessageBox.Show("Could not initialize the shader", "Error", MessageBoxButtons.OK);
                    return false;
                }
                
                // Set the position of the camera.
                Camera.SetPosition(0.0f, 0.0f, -10.0f);

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

            // Release the translate shader object.
            TranslateShader?.ShutDown();
            TranslateShader = null;
            // Release the model object.
            Model?.Shutdown();
            Model = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Render()
        {
            // Increment the texture translation position.
            TextureTranslate();

            // Clear the buffer to begin the scene as Black.
            D3D.BeginScene(0, 0, 0, 1f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world, view, and projection matrices from camera and d3d objects.
            Matrix viewMatrix = Camera.ViewMatrix;
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;

            // Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            Model.Render(D3D.DeviceContext);

            // Render the model with the texture translation shader.
            if (!TranslateShader.Render(D3D.DeviceContext, Model.IndexCount, worldMatrix, viewMatrix, projectionMatrix, Model.TextureCollection.Select(item => item.TextureResource).ToArray(), TextureTranslation))
                return false;

            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }
        // Static Methods.
        public static void TextureTranslate()
        {
            TextureTranslation += 0.01f;
            if (TextureTranslation > 1.0f)
                TextureTranslation -= 1.0f;
        }
    }
}