using DSharpDXRastertek.Tut43.Graphics.Camera;
using DSharpDXRastertek.Tut43.Graphics.Data;
using DSharpDXRastertek.Tut43.Graphics.Models;
using DSharpDXRastertek.Tut43.Graphics.Shaders;
using DSharpDXRastertek.Tut43.System;
using SharpDX;
using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut43.Graphics
{
    public class DGraphics                  // 192 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }

        #region Data
        private DLight Light { get; set; }
        public DTexture ProjectionTexture { get; set; }
        public DViewPoint ViewPoint { get; set; }
        #endregion

        #region Models
        private DModel CubeModel { get; set; }
        private DModel GroundModel { get; set; }
        #endregion

        #region Shaders
        public DProjectionShader ProjectionShader { get; set; }
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

                // Set the initial position and rotation of the camera.
                Camera.SetPosition(0.0f, 7.0f, -10.0f);
                Camera.SetRotation(35.0f, 0.0f, 0.0f);
                #endregion

                #region Initialize Models
                // Create the ground model object.
                GroundModel = new DModel();

                // Initialize the ground model object.
                if (!GroundModel.Initialize(D3D.Device, "floor.txt", "stone.dds"))
                    return false;

                // Create the ground model object.
                CubeModel = new DModel();

                // Initialize the cube model object.
                if (!CubeModel.Initialize(D3D.Device, "cube.txt", "seafloor.dds"))
                    return false;
                #endregion

                #region Data variables.
                // Create the light object.
                Light = new DLight();

                // Initialize the light object.
                Light.SetAmbientColor(0.15f, 0.15f, 0.15f, 1.0f);
                Light.SetDiffuseColor(1.0f, 1.0f, 1.0f, 1.0f);
                Light.SetDirection(0.0f, -0.75f, 0.5f);
                #endregion

                #region Initialize Shaders  
                // Create the projection shader object.
                ProjectionShader = new DProjectionShader();

                // Initialize the projection shader object.
                if (!ProjectionShader.Initialize(D3D.Device, windowHandle))
                    return false;

                // Create the projection texture object.
                ProjectionTexture = new DTexture();

                // Initialize the projection texture object.
                if (!ProjectionTexture.Initialize(D3D.Device, DSystemConfiguration.DataFilePath +"dx11.dds"))
                    return false;

                // Create the view point object.
                ViewPoint = new DViewPoint();

                // Initialize the view point object.
                ViewPoint.SetPosition(2.0f, 5.0f, -2.0f);
                ViewPoint.SetLookAt(0.0f, 0.0f, 0.0f);
                ViewPoint.SetProjectionParameters((float)(Math.PI / 2.0f), 1.0f, 0.1f, 100.0f);
                ViewPoint.GenerateViewMatrix();
                ViewPoint.GenerateProjectionMatrix();
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
            // Release the light object.
            Light = null;
            // Release the camera object.
            Camera = null;
            // Release the view point object.
            ViewPoint = null;

            // Release the projection texture object.
            ProjectionTexture?.ShutDown();
            ProjectionTexture = null;
            // Release the projection shader object.
            ProjectionShader?.ShutDown();
            ProjectionShader = null;
            // Release the ground model object.
            GroundModel?.Shutdown();
            GroundModel = null;
            // Release the cube model object.
            CubeModel?.Shutdown();
            CubeModel = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Frame()
        {
            // Render the graphics scene.
            if (!Render())
                return false;

            return true;
        }
        private bool Render()
        {
            // Clear the buffers to begin the scene.
            D3D.BeginScene(0.0f, 0.0f, 0.0f, 1.0f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world, view, and projection matrices from the camera and d3d objects.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix viewMatrix = Camera.ViewMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;

            // Get the view and projection matrices from the view point object.
            Matrix viewMatrix2 = ViewPoint.ViewMatrix;
            Matrix projectionMatrix2 = ViewPoint.ProjectionMatrix;

            // Setup the translation for the ground model.
            Matrix.Translation(0.0f, 1.0f, 0.0f, out worldMatrix);

            // Render the ground model using the projection shader.
            GroundModel.Render(D3D.DeviceContext);
            if (!ProjectionShader.Render(D3D.DeviceContext, GroundModel.IndexCount, worldMatrix, viewMatrix, projectionMatrix, GroundModel.Texture.TextureResource, Light.AmbientColor, Light.DiffuseColour, Light.Direction, viewMatrix2, projectionMatrix2, ProjectionTexture.TextureResource))
                return false; // 

            // Reset the world matrix and setup the translation for the cube model.
            worldMatrix = D3D.WorldMatrix;
            Matrix.Translation(0.0f, 2.0f, 0.0f, out worldMatrix);

            // Render the cube model using the projection shader.
            CubeModel.Render(D3D.DeviceContext);
            if (!ProjectionShader.Render(D3D.DeviceContext, CubeModel.IndexCount, worldMatrix, viewMatrix, projectionMatrix, CubeModel.Texture.TextureResource, Light.AmbientColor, Light.DiffuseColour, Light.Direction, viewMatrix2, projectionMatrix2, ProjectionTexture.TextureResource))
                return false; // 

            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }
    }
}