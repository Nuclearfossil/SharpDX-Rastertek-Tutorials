using DSharpDXRastertek.Tut27.Graphics.Camera;
using DSharpDXRastertek.Tut27.Graphics.Data;
using DSharpDXRastertek.Tut27.Graphics.Models;
using DSharpDXRastertek.Tut27.Graphics.Shaders;
using DSharpDXRastertek.Tut27.System;
using SharpDX;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut27.Graphics
{
    public class DGraphics                  // 249 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }

        #region Data
        private DRenderTexture RenderTexture { get; set; }
        #endregion

        #region Models
        private DModel Model { get; set; }
        private DModel FloorModel { get; set; }
        #endregion

        #region Shaders
        private DReflectionShader ReflectionShader { get; set; }
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
                Camera.SetPosition(0, 0, -10);
                #endregion

                #region Initialize Models
                // Create the model class.
                Model = new DModel();

                // Initialize the model object.
                if (!Model.Initialize(D3D.Device, "Cube.txt", new[] { "seafloor.bmp" }))
                {
                    MessageBox.Show("Could not initialize the model object", "Error", MessageBoxButtons.OK);
                    return false;
                }

                // Create the model class.
                FloorModel = new DModel();

                // Initialize the model object.
                if (!FloorModel.Initialize(D3D.Device, "floor.txt", new[] { "blue01.bmp" }))
                {
                    MessageBox.Show("Could not initialize the model object", "Error", MessageBoxButtons.OK);
                    return false;
                }
                #endregion

                #region Initialize Shaders
                // Create the shader object.
                TextureShader = new DTextureShader();

                // Initialize the shader object.
                if (!TextureShader.Initialize(D3D.Device, windowHandle))
                {
                    MessageBox.Show("Could not initialize the shader", "Error", MessageBoxButtons.OK);
                    return false;
                }

                // Create the Reflection shader object.
                ReflectionShader = new DReflectionShader();

                // Initialize the shader object.
                if (!ReflectionShader.Initialize(D3D.Device, windowHandle))
                {
                    MessageBox.Show("Could not initialize the shader", "Error", MessageBoxButtons.OK);
                    return false;
                }
                #endregion

                #region Initialize Data
                // Create the render to texture object.
                RenderTexture = new DRenderTexture();

                // Initialize the render to texture object.
                if (!RenderTexture.Initialize(D3D.Device, configuration))
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

            // Release the reflection shader object.
            ReflectionShader?.ShutDown();
            ReflectionShader = null;
            // Release the texture shader object.
            TextureShader?.ShutDown();
            TextureShader = null;
            // Release the render to texture object.
            RenderTexture?.Shutdown();
            RenderTexture = null;
            // Release the model object.
            Model?.Shutdown();
            Model = null;
            FloorModel?.Shutdown();
            FloorModel = null;
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
            // Render the entire scene as a reflection to the texture first.
            if (!RenderToReflectionTexture())
                return false;

            //// Render the scene as normal to the back buffer.
            if (!RenderScene())
                return false;

            return true;
        }
        private bool RenderToReflectionTexture()
        {
            // Set the render to be the render to the texture.
            RenderTexture.SetRenderTarget(D3D.DeviceContext, D3D.DepthStencilView);

            // Clear the render to texture.
            RenderTexture.ClearRenderTarget(D3D.DeviceContext, D3D.DepthStencilView, 0, 0, 0, 1);

            // Use the camera to calculate the reflection matrix.
            Camera.RenderReflection(-1.5f);

            // Get the camera reflection view matrix instead of the normal view matrix.
            var viewMatrix = Camera.ReflectionViewMatrix;

            // Get the world and projection matrices.
            var worldMatrix = D3D.WorldMatrix;
            var projectionMatrix = D3D.ProjectionMatrix;

            // Update the rotation variable each frame.
            Rotate();

            // Rotate the world matrix by the rotation value
            Matrix.RotationY(Rotation, out worldMatrix);

            // Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            Model.Render(D3D.DeviceContext);

            // Render the model using the texture shader and the reflection view matrix.
            if (!TextureShader.Render(D3D.DeviceContext, Model.IndexCount, worldMatrix, viewMatrix, projectionMatrix, Model.TextureCollection.Select(item => item.TextureResource).First()))
                return false;

            // Reset the render target back to the original back buffer and not to texture anymore.
            D3D.SetBackBufferRenderTarget();

            return true;
        }
        private bool RenderScene()
        {
            // Clear the buffer to begin the scene as Black.
            D3D.BeginScene(0, 0, 0, 1f);

            // Generate the view matrix based on the camera position.
            Camera.Render();

            // Get the world, view, and projection matrices from camera and d3d objects.
            var viewMatrix = Camera.ViewMatrix;
            var worldMatrix = D3D.WorldMatrix;
            var projectionMatrix = D3D.ProjectionMatrix;

            //// Rotate the world matrix by the rotation value so that the triangle will spin.
            Matrix.RotationY(Rotation, out worldMatrix);

            // Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            Model.Render(D3D.DeviceContext);

            // Render the model with the texture shader.
            if (!TextureShader.Render(D3D.DeviceContext, Model.IndexCount, worldMatrix, viewMatrix, projectionMatrix, Model.TextureCollection.Select(item => item.TextureResource).First()))
                return false;

            // Get the world matrix again and translate down for the floor model to render underneath the cube.
            worldMatrix = D3D.WorldMatrix;
            Matrix.Translation(0, -1.5f, 0, out worldMatrix);

            // Get the camera reflection view matrix.
            var reflectionMatrix = Camera.ReflectionViewMatrix;

            // Put the floor model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            FloorModel.Render(D3D.DeviceContext);

            // Render the floor model using the reflection shader, reflection texture, and reflection view matrix.
            if (!ReflectionShader.Render(D3D.DeviceContext, FloorModel.IndexCount, worldMatrix, viewMatrix, projectionMatrix, FloorModel.TextureCollection.Select(item => item.TextureResource).First(), RenderTexture.ShaderResourceView, reflectionMatrix))
                return false;

            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }

        // Static Methods.
        static void Rotate()
        {
            Rotation += (float)Math.PI * 0.005f;
            if (Rotation > 360)
                Rotation -= 360;
        }
    }
}