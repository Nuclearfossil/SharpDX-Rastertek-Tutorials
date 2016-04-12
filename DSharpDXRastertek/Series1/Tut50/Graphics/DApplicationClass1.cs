using DSharpDXRastertek.Tut50.Graphics.Camera;
using DSharpDXRastertek.Tut50.Graphics.Data;
using DSharpDXRastertek.Tut50.Graphics.Models;
using DSharpDXRastertek.Tut50.Input;
using DSharpDXRastertek.Tut50.System;
using SharpDX;
using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut50.Graphics
{
    public class DApplication                   // 227 lines
    {
        // Properties
        public DInput Input { get; private set; }
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }

        #region Models
        public DModel Model { get; set; }
        public DLight Light { get; set; }
        #endregion

        #region Shaders
        public DOrthoWindow FullScreenWindow { get; set; }
        public DDeferredBuffers DeferredBuffers { get; set; }
        public DDeferredShader DeferredShader { get; set; }
        public DLightShader LightShader { get; set; }
        #endregion     

        // Static properties
        public static float Rotation { get; set; }

        // Construtor
        public DApplication() { }

        // Methods.
        public bool Initialize(DSystemConfiguration configuration, IntPtr windowHandle)
        {
            try
            {
                // Create the input object.
                Input = new DInput();

                // Initialize the input object.
                if (!Input.Initialize(configuration, windowHandle))
                    return false;   

                // #region Initialize System
                // Create the Direct3D object.
                D3D = new DDX11();
                
                // Initialize the Direct3D object.
                if (!D3D.Initialize(configuration, windowHandle))
                    return false;

                // Create the camera object
                Camera = new DCamera();

                // Set the initial position of the camera and build the matrices needed for rendering.
                Camera.SetPosition(0.0f, 0.0f, -10.0f);
                Camera.Render();
                Camera.RenderBaseViewMatrix();  // This might be mis-implemented.  CHECK THIS !!!

                // Create the light object.
                Light = new DLight();

                // Initialize the light object.
                Light.Direction = new Vector3(0.0f, 0.0f, 1.0f);

                // Create the model object.
                Model = new DModel();

                // Initialize the ground model object.
                if (!Model.Initialize(D3D.Device, "cube.txt", "seafloor.bmp"))
                    return false;

                // Create the full screen ortho window object.
                FullScreenWindow = new DOrthoWindow();

                // Initialize the full screen ortho window object.
                if (!FullScreenWindow.Initialize(D3D.Device, configuration.Width, configuration.Height))
                    return false;

                // Create the deferred buffers object.
                DeferredBuffers = new DDeferredBuffers();

                // Initialize the deferred buffers object.
                if(!DeferredBuffers.Initialize(D3D.Device, configuration.Width, configuration.Height))
                    return false;

                // Create the deferred shader object.
                DeferredShader = new DDeferredShader();

                // Initialize the deferred shader object.
                if (!DeferredShader.Initialize(D3D.Device, windowHandle))
                    return false;

                // Create the light shader object.
                LightShader = new DLightShader();

                // Initialize the light shader object.
                if (!LightShader.Initialize(D3D.Device, windowHandle))
                    return false;

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

            // Release the light shader object.
            LightShader?.ShutDown();
            LightShader = null;
            // Release the deferred shader object.
            DeferredShader?.ShutDown();
            DeferredShader = null;
            // Release the deferred buffers object.
            DeferredBuffers?.Shutdown();
            DeferredBuffers = null;
            // Release the full screen ortho window object.
            FullScreenWindow?.Shutdown();
            FullScreenWindow = null;
            // Release the tree object.
            Model?.Shutdown();
            Model = null;
            // Release the input object.
            Input?.Shutdown();
            Input = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Frame()
        {
            // Render the graphics.
            if (!Render())
                return false;

            return true;
        }
        private bool Render()
        {
            // Render the depth of the scene to a texture.
            if (!RenderSceneToTexture())
                return false;

            // Clear the scene.
            D3D.BeginScene(0.0f, 0.5f, 0.8f, 1.0f);

            // Get the matrices.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix cameraBaseViewMatrix = Camera.BaseViewMatrix;
            Matrix orthoMatrix = D3D.OrthoMatrix;

            // Turn off the Z buffer to begin all 2D rendering.
            D3D.TurnZBufferOff();

            // Put the full screen ortho window vertex and index buffers on the graphics pipeline to prepare them for drawing.
            if (!FullScreenWindow.Render(D3D.DeviceContext))
                return false;

            // Render the full screen ortho window using the deferred light shader and the render buffers.
            if (!LightShader.Render(D3D.DeviceContext, FullScreenWindow.IndexCount, worldMatrix, cameraBaseViewMatrix, orthoMatrix, DeferredBuffers.ShaderResourceViewArray[0], DeferredBuffers.ShaderResourceViewArray[1], Light.Direction))
                return false;

            // Turn the Z buffer back on now that all 2D rendering has completed.
            D3D.TurnZBufferOn();

            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }
        private bool RenderSceneToTexture()
        {
            // Set the render buffers to be the render target.
            DeferredBuffers.SetRenderTargets(D3D.DeviceContext);

            // Clear the render buffers.
            DeferredBuffers.ClearRenderTargets(D3D.DeviceContext, 0.0f, 0.0f, 0.0f, 1.0f);

            // Get the matrices from the camera and d3d objects.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix cameraViewMatrix = Camera.ViewMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;

            // Update the rotation variable each frame.
            Rotate();

            // Rotate the world matrix by the rotation value so that the cube will spin.
            Matrix.RotationY(Rotation, out worldMatrix);

            // Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            Model.Render(D3D.DeviceContext);

            // Render the model using the deferred shader.
            if (!DeferredShader.Render(D3D.DeviceContext, Model.IndexCount, worldMatrix, cameraViewMatrix, projectionMatrix, Model.Texture.TextureResource))
                return false;

            // Reset the render target back to the original back buffer and not the render buffers anymore.
            D3D.SetBackBufferRenderTarget();

            // Reset the viewport back to the original.
            D3D.ResetViewPort();

            return true;
        }

        // Static Methods.
        static void Rotate()
        {
            Rotation += (float)Math.PI * 0.001f;
            if (Rotation > 360)
                Rotation -= 360;
        }
    }
}