using DSharpDXRastertek.Tut24.Graphics.Camera;
using DSharpDXRastertek.Tut24.Graphics.Models;
using DSharpDXRastertek.Tut24.Graphics.Shaders;
using DSharpDXRastertek.Tut24.System;
using SharpDX;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut24.Graphics
{
    public class DGraphics                  // 130 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }

        #region Models
        private DModel Model { get; set; }
        #endregion

        #region Shaders
        private DClipPlaneShader ClipPlaneShader { get; set; }
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
               

                // Create the model class.
                Model = new DModel();

                // Initialize the model object.
                if (!Model.Initialize(D3D.Device, "triangle.txt", new[] { "seafloor.bmp" }))
                {
                    MessageBox.Show("Could not initialize the model object", "Error", MessageBoxButtons.OK);
                    return false;
                }
               
                // Create the shader object.
                ClipPlaneShader = new DClipPlaneShader();

                // Initialize the shader object.
                if (!ClipPlaneShader.Initialize(D3D.Device, windowHandle))
                {
                    MessageBox.Show("Could not initialize the shader", "Error", MessageBoxButtons.OK);
                    return false;
                }

                Camera.SetPosition(0, 0, -5);

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

            // Release the clip plane shader object.
            ClipPlaneShader?.ShutDown();
            ClipPlaneShader = null;
            // Release the model object.
            Model?.Shutdown();
            Model = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
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

            // Setup a clipping plane.
            Vector4 clipPlane = new Vector4(0, -1.0f, 0, 0);

            // Render the model using the color shader.
            if (!ClipPlaneShader.Render(D3D.DeviceContext, Model.IndexCount, worldMatrix, viewMatrix, projectionMatrix, Model.TextureCollection.Select(item => item.TextureResource).ToArray(), clipPlane))
                return false;

            return true;
        }
    }
}