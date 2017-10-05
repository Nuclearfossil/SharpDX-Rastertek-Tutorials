using DSharpDXRastertek.Tut28.Graphics.Camera;
using DSharpDXRastertek.Tut28.Graphics.Data;
using DSharpDXRastertek.Tut28.Graphics.Models;
using DSharpDXRastertek.Tut28.Graphics.Shaders;
using DSharpDXRastertek.Tut28.System;
using SharpDX;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut28.Graphics
{
    public class DGraphics                  // 279 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }
        private float FadeInTime { get; set; }
        private float AccumulatedTime { get; set; }
        private float FadePercentage { get; set; }
        private bool FadeDone { get; set; }

        #region Data
        private DRenderTexture RenderTexture { get; set; }
        #endregion

        #region Models
        private DModel Model { get; set; }
        private DBitmap Bitmap { get; set; }
        #endregion

        #region Shaders
        private DTextureShader TextureShader { get; set; }
        private DFadeShader FadeShader { get; set; }
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

                // Create the bitmap object.
                Bitmap = new DBitmap();

                // Initialize the model object.
                if (!Bitmap.Initialize(D3D.Device, configuration.Width, configuration.Height, configuration.Width, configuration.Height))
                {
                    MessageBox.Show("Could not initialize the bitmap object", "Error", MessageBoxButtons.OK);
                    return false;
                }
                #endregion

                #region Initialize Shader
                // Create the shader object.
                TextureShader = new DTextureShader();

                // Initialize the shader object.
                if (!TextureShader.Initialize(D3D.Device, windowHandle))
                {
                    MessageBox.Show("Could not initialize the shader", "Error", MessageBoxButtons.OK);
                    return false;
                }

                // Create the shader object.
                FadeShader = new DFadeShader();

                // Initialize the shader object.
                if (!FadeShader.Initialize(D3D.Device, windowHandle))
                {
                    MessageBox.Show("Could not initialize the fade shader", "Error", MessageBoxButtons.OK);
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

                #region Initialize Variables
                // Set the fade in time to 3000 milliseconds
                FadeInTime = 5000;
                // Initialize the accumulated time to zero milliseconds.
                AccumulatedTime = 0;
                // Initialize the fade percentage to zero at first so the scene is black.
                FadePercentage = 0;
                // Set the fading in effect to not done.
                FadeDone = false;
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

            // Release the model object.
            Bitmap?.Shutdown();
            Bitmap = null;
            // Release the fade shader object.
            FadeShader?.ShutDown();
            FadeShader = null;
            // Release the texture shader object.
            TextureShader?.ShutDown();
            TextureShader = null;
            // Release the render to texture object.
            RenderTexture?.Shutdown();
            RenderTexture = null;
            // Release the model object.
            Model?.Shutdown();
            Model = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Frame(float frameTime)
        {
            if (!FadeDone)
            {
                // Update the accumulated time with the extra frame time addition.
                AccumulatedTime += frameTime;

                // While the time goes on increase the fade in amount by the time is passing each frame.
                if (AccumulatedTime < FadeInTime)
                    // Calculate the percentage that the screen should be faded in based on the accumulated time.
                    FadePercentage = AccumulatedTime / FadeInTime;
                else
                {
                    // If the fade in time is complete then turn off effect and render the scene normally.
                    FadeDone = true;

                    // Set the percentage to 100%
                    FadePercentage = 1f;
                }
            }

            return true;
        }
        public bool Render()
        {
            Rotate();

            // Clear the buffer to begin the scene as Black.
            D3D.BeginScene(0, 0, 0, 1f);

            if (FadeDone)
            {
                // If fading in is complete the render the scene as normal to the back buffer.
                if (!RenderScene())
                    return false;
            }
            else
            {
                // If fading is not complete then render the scene to a texture and fade that texture in.
                if (!RenderToTexture() || !RenderFadingScene())
                    return false;
            }
           
            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }
        private bool RenderToTexture()
        {
            // Set the render to be the render to the texture.
			RenderTexture.SetRenderTarget(D3D.DeviceContext, D3D.DepthStencilView);

			// Clear the render to texture.
			RenderTexture.ClearRenderTarget(D3D.DeviceContext, D3D.DepthStencilView, 0, 0, 0, 1);

			// Render the scene now and it will draw to the render to texture instead of the back buffer.
			if (!RenderScene())
				return false;

			// Reset the render target back to the original back buffer and not to texture anymore.
			D3D.SetBackBufferRenderTarget();
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

            // Rotate the world matrix by the rotation value so that the triangle will spin.
            Matrix.RotationY(Rotation, out worldMatrix);

            // Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            Model.Render(D3D.DeviceContext);

            // Render the model using the color shader.
            if (!TextureShader.Render(D3D.DeviceContext, Model.IndexCount, worldMatrix, viewMatrix, projectionMatrix, Model.TextureCollection.Select(item => item.TextureResource).First()))
                return false;

            return true;
        }
        private bool RenderFadingScene()
        {
            // Turn off the Z buffer to begin all 2D rendering.
            D3D.TurnZBufferOff();

            // Get the world, view, and orthotic matrices from camera and d3d objects.
            var viewMatrix = Camera.ViewMatrix;
            var worldMatrix = D3D.WorldMatrix;
            var orthoMatrix = D3D.OrthoMatrix;

            // Put the bitmap vertex and index buffers on the graphics pipeline to prepare them for drawing.
            if (!Bitmap.Render(D3D.DeviceContext, 0, 0))
                return false;

            // Render the bitmap using the fade shader.
            if (!FadeShader.Render(D3D.DeviceContext, Bitmap.IndexCount, worldMatrix, viewMatrix, orthoMatrix, RenderTexture.ShaderResourceView, FadePercentage))
                return false;

            // Turn the Z buffer back on now that all 2D rendering has completed.
            D3D.TurnZBufferOn();

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