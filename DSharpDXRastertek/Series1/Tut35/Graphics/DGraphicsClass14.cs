using DSharpDXRastertek.Tut35.Graphics.Models;
using DSharpDXRastertek.Tut35.System;
using SharpDX;
using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut35.Graphics
{
    public class DGraphics                  // 124 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }

        #region Models
        private DModel FloorModel { get; set; }
        private DModel BillboardModel { get; set; }
        #endregion

        #region Shaders
        public DDepthShader DepthShader { get; set; }
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

                // Set the initial position of the camera.
                Camera.SetPosition(0.0f, 2.0f, -10.0f);
               #endregion

                #region Initialize Models
                // Create the Flat Plane  model class.
                FloorModel = new DModel();

                // Create the floor model object.
                if (!FloorModel.Initialize(D3D.Device, "floor.txt"))
                {
                    MessageBox.Show("Could not initialize the ground model object", "Error", MessageBoxButtons.OK);
                    return false;
                }

                // Create the depth shader object.
                DepthShader = new DDepthShader();

                if (!DepthShader.Initialize(D3D.Device, windowHandle))
                    return false;
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

            // Release the depth shader object.
            DepthShader?.ShutDown();
            DepthShader = null;
            // Release the Floor model object.
            FloorModel?.Shutdown();
            FloorModel = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Frame()
        {
            // Render the scene.
            if (!Render())
                return false;

            return true;
        }
        public bool Render()
        {
            // Clear the buffers to begin the scene.
            D3D.BeginScene(0, 0, 0, 1f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world, view, and projection matrices from the camera and d3d objects.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix viewMatrix = Camera.ViewMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;

            // Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            FloorModel.Render(D3D.DeviceContext);

            // Render the model using the depth shader.
            if (!DepthShader.Render(D3D.DeviceContext, FloorModel.IndexCount, worldMatrix, viewMatrix, projectionMatrix))
                return false;
            
            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }
    }
}