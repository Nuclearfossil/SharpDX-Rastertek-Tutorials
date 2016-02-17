using DSharpDXRastertek.Tut22.Graphics.Camera;
using DSharpDXRastertek.Tut22.Graphics.Data;
using DSharpDXRastertek.Tut22.Graphics.Models;
using DSharpDXRastertek.Tut22.Graphics.Shaders;
using DSharpDXRastertek.Tut22.System;
using SharpDX;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut22.Graphics
{
    public class DGraphics                  // 232 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }
        private DModel Model { get; set; }
        private DLight Light { get; set; }
        private DDebugWindow DebugWindow { get; set; }
        private DLightShader LightShader { get; set; }
        private DTextureShader TextureShader { get; set; }

        #region Resources
        private DRenderTexture RenderTexture { get; set; }
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
                var baseViewMatrix = Camera.ViewMatrix;

                // Create the model class.
                Model = new DModel();

                // Initialize the model object.
                if (!Model.Initialize(D3D.Device, "cube.txt", new[] { "seafloor.dds" }))
                {
                    MessageBox.Show("Could not initialize the model object", "Error", MessageBoxButtons.OK);
                    return false;
                }

                // Create the light shader object.
                LightShader = new DLightShader();

                // Initialize the light shader object.
                if (!LightShader.Initialize(D3D.Device, windowHandle))
                {
                    MessageBox.Show("Could not initialize the light shader", "Error", MessageBoxButtons.OK);
                    return false;
                }

                // Create the light object.
                Light = new DLight();

                // Initialize the light object.
                Light.SetDiffuseColor(1, 1, 1, 1f);
                Light.SetDirection(0, 0, 1);

                // Create the render to texture object.
                RenderTexture = new DRenderTexture();

                // Initialize the render to texture object.
                if (!RenderTexture.Initialize(D3D.Device, configuration))
                    return false;

                // Create the debug window object.
                DebugWindow = new DDebugWindow();

                // Initialize the debug window object.  * configuration.Height / configuration.Width
                if (!DebugWindow.Initialize(D3D.Device, configuration.Width, configuration.Height, 100, 100))
                {
                    MessageBox.Show("Could not initialize the debug window object.", "Error", MessageBoxButtons.OK);
                    return false;
                }

                // Create the texture shader object.
                TextureShader = new DTextureShader();

                // Initialize the texture shader object.
                if (!TextureShader.Initialize(D3D.Device, windowHandle))
                {
                    MessageBox.Show("Could not initialize the texture shader object.", "Error", MessageBoxButtons.OK);
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
            // Release the light object.
            Light = null;
            // Release the camera object.
            Camera = null;

            // Release the texture shader object.
            TextureShader?.ShutDown();
            TextureShader = null;
            // Release the debug window object.
            DebugWindow?.Shutdown();
            DebugWindow = null;
            // Release the render to texture object.
            RenderTexture?.Shutdown();
            RenderTexture = null;
            // Release the light shader object.
            LightShader?.ShutDown();
            LightShader = null;
            // Release the model object.
            Model?.Shutdown();
            Model = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Render()
        {
            // Render the entire scene to the texture first.
            if (!RenderToTexture())
                return false;

            // Clear the buffer to begin the scene.
            D3D.BeginScene(0f, 0f, 0f, 1f);

            //// Render the scene as normal to the back buffer.
            if (!RenderScene())
                return false;

            // Turn off the Z buffer to begin all 2D rendering.
            D3D.TurnZBufferOff();

            // Get the world, view, and projection matrices from camera and d3d objects.
            var viewMatrix = Camera.ViewMatrix;
            var worldMatrix = D3D.WorldMatrix;
            var orthoMatrix = D3D.OrthoMatrix;

            // Put the debug window vertex and index buffer on the graphics pipeline them for drawing.
            if (!DebugWindow.Render(D3D.DeviceContext, 50, 50))
                return false;

            // Render the debug window using the texture shader.
            if (!TextureShader.Render(D3D.DeviceContext, DebugWindow.IndexCount, worldMatrix, viewMatrix, orthoMatrix, RenderTexture.ShaderResourceView))
                return false;

            // Turn the Z buffer back on now that all 2D rendering has completed.
            D3D.TurnZBufferOn();

            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }
        private bool RenderToTexture()
        {
            // Set the render to be the render to the texture.
            RenderTexture.SetRenderTarget(D3D.DeviceContext, D3D.DepthStencilView);

            // Clear the render to texture.
            RenderTexture.ClearRenderTarget(D3D.DeviceContext, D3D.DepthStencilView, 0, 0, 1, 1);

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
            Rotate();

            // Rotate the world matrix by the rotation value so that the triangle will spin.
            Matrix.RotationY(Rotation, out worldMatrix);

            // Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            Model.Render(D3D.DeviceContext);

            // Render the model using the color shader.
            if (!LightShader.Render(D3D.DeviceContext, Model.IndexCount, worldMatrix, viewMatrix, projectionMatrix, Model.TextureCollection.Select(item => item.TextureResource).First(), Light.Direction, Light.DiffuseColour))
                return false;

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