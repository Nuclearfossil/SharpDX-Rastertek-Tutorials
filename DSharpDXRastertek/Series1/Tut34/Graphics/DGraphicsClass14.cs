using DSharpDXRastertek.Tut34.Graphics.Models;
using DSharpDXRastertek.Tut34.Graphics.Shaders;
using DSharpDXRastertek.Tut34.System;
using SharpDX;
using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut34.Graphics
{
    public class DGraphics                  // 167 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }
        
        #region Models
        private DModel FloorModel { get; set; }
        private DModel BillboardModel { get; set; }
        #endregion

        #region Shaders
        private DTextureShader TextureShader { get; set; }
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
                Camera.SetPosition(0.0f, 0.0f, -7.0f);
                #endregion

                #region Initialize Shaders
                // Create the texture shader object.
                TextureShader = new DTextureShader();

                // Create the texture shader object.
                if (!TextureShader.Initialize(D3D.Device, windowHandle))
                    return false;
                #endregion

                #region Initialize Models
                // Create the Flat Plane  model class.
                FloorModel = new DModel();

                // Create the floor model object.
                if (!FloorModel.Initialize(D3D.Device, "floor.txt", new[] { "grid01.dds"}))
                {
                    MessageBox.Show("Could not initialize the ground model object", "Error", MessageBoxButtons.OK);
                    return false;
                }

                // Create the billboard model object.
                BillboardModel = new DModel();

                // Initialize the billboard model object.
                if (!BillboardModel.Initialize(D3D.Device, "square.txt", new[]{ "seafloor.dds" } ))
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

            // Release the texture shader object.
            TextureShader?.ShutDown();
            TextureShader = null;
            // Release the Floor model object.
            FloorModel?.Shutdown();
            FloorModel = null;
            // Release the Billboard model object.
            BillboardModel?.Shutdown();
            BillboardModel = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Frame(float positionX, float positionY, float positionZ)
        {
            // Update the position of the camera.
            Camera.SetPosition(positionX, positionY, positionZ);

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

            // Put the floor model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            FloorModel.Render(D3D.DeviceContext);

            // Render the floor model using the texture shader.
            if (!TextureShader.Render(D3D.DeviceContext, FloorModel.IndexCount, worldMatrix, viewMatrix, projectionMatrix, FloorModel.GetTexture()))
                return false;

            // Get the position of the camera.
            Vector3 cameraPosition = Camera.GetPosition();

            // Set the position of the billboard model.
            Vector3 modelPosition = new Vector3();
            modelPosition.X = 0.0f;
            modelPosition.Y = 1.5f;
            modelPosition.Z = 0.0f;

            // Calculate the rotation that needs to be applied to the billboard model to face the current camera position using the arc tangent function.
            double angle = Math.Atan2(modelPosition.X - cameraPosition.X, modelPosition.Z - cameraPosition.Z) * (180.0f / Math.PI);
            // Convert rotation into radians.
            float rotation = (float)angle * 0.0174532925f;
            // Setup the rotation the billboard at the origin using the world matrix.
            Matrix.RotationY(rotation, out worldMatrix);
            // Setup the translation matrix from the billboard model.
            Matrix translationMatrix = Matrix.Translation(modelPosition.X, modelPosition.Y, modelPosition.Z);
            // Finally combine the rotation and translation matrices to create the final world matrix for the billboard model.
            Matrix.Multiply(ref worldMatrix, ref translationMatrix, out worldMatrix);

            // Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            BillboardModel.Render(D3D.DeviceContext);

            // Render the model using the texture shader.
            if (!TextureShader.Render(D3D.DeviceContext, BillboardModel.IndexCount, worldMatrix, viewMatrix, projectionMatrix, BillboardModel.GetTexture()))
                return false;

            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }
    }
}