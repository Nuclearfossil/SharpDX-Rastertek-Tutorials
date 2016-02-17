using DSharpDXRastertek.Tut23.Graphics.Camera;
using DSharpDXRastertek.Tut23.Graphics.Models;
using DSharpDXRastertek.Tut23.System;
using SharpDX;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut23.Graphics
{
    public class DGraphics                  // 142 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }

        #region Models
        private DModel Model { get; set; }
        #endregion

        #region Shaders
        private DFogShader FogShader { get; set; }
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
                // Create the Direct3D object.
                D3D = new DDX11();
                
                // Initialize the Direct3D object.
                if (!D3D.Initialize(configuration, windowHandle))
                    return false;

                // Create the camera object
                Camera = new DCamera();

                // Initialize a base view matrix the camera for 2D user interface rendering.
                Camera.SetPosition(0, 0, -1);
                Camera.Render();

                // Create the model class.
                Model = new DModel();

                // Initialize the model object.
                if (!Model.Initialize(D3D.Device, "cube.txt", new[] { "seafloor.dds" }))
                {
                    MessageBox.Show("Could not initialize the model object", "Error", MessageBoxButtons.OK);
                    return false;
                }
   
                // Create the shader object.
                FogShader = new DFogShader();

                // Initialize the shader object.
                if (!FogShader.Initialize(D3D.Device, windowHandle))
                {
                    MessageBox.Show("Could not initialize the fog shader", "Error", MessageBoxButtons.OK);
                    return false;
                }

                Camera.SetPosition(0, 0, -5.0f);

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

            // Release the fog shader object.
            FogShader?.ShutDown();
            FogShader = null;
            //// Release the model object.
            Model?.Shutdown();
            Model = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Render()
        {
            // Set the color of the fog to grey.
            var fogColor = 0.5f;

            // Set the start and end of the fog.
            var fogStart = 4.0f; 
            var fogEnd = 4.5f;

            // Clear the buffer to begin the scene.
            D3D.BeginScene(fogColor, fogColor, fogColor, 1f);

            // Generate the view matrix based on the camera position.
            Camera.Render();

            // Get the world, view, and projection matrices from the camera and d3d objects.
            Matrix viewMatrix = Camera.ViewMatrix;
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;

            // Update the rotation variable each frame.
            Rotate();

            // Multiply the world matrix by the rotation.
            Matrix.RotationY(Rotation, out worldMatrix);

            // Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            Model.Render(D3D.DeviceContext);

            // Render the model with the fog shader.
            if (!FogShader.Render(D3D.DeviceContext, Model.IndexCount, worldMatrix, viewMatrix, projectionMatrix, Model.TextureCollection.Select(item => item.TextureResource).ToArray(), fogStart, fogEnd))
                return false;

            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }

        // Static Methods.
        static void Rotate()
        {
            Rotation += (float)Math.PI * 0.0002f;
            if (Rotation > 360)
                Rotation -= 360;
        }
    }
}