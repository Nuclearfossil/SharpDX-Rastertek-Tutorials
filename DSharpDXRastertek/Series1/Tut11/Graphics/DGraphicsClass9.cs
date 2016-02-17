using DSharpDXRastertek.Tut11.System;
using SharpDX;
using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut11.Graphics
{
    public class DGraphics                  // 124 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        private DCamera Camera { get; set; }
        private DBitmap Bitmap { get; set; }
        private DTextureShader TextureShader { get; set; }
        public DTimer Timer { get; set; }

        // Construtor
        public DGraphics() { }

        // Methods
        public bool Initialize(DSystemConfiguration configuration, IntPtr windowsHandle)
        {
            try
            {
                // Create the Direct3D object.
                D3D = new DDX11();

                // Initialize the Direct3D object.
                if (!D3D.Initialize(configuration, windowsHandle))
                    return false;

                // Create the Timer
                Timer = new DTimer();

                // Initialize the Timer
                if (!Timer.Initialize())
                    return false;

                // Create the camera object
                Camera = new DCamera();

                // Set the initial position of the camera.  moved closer inTutorial 7
                Camera.SetPosition(0, 0, -10);

                // Create the texture shader object.
                TextureShader = new DTextureShader();

                // Initialize the texture shader object.
                if (!TextureShader.Initialize(D3D.Device, windowsHandle))
                {
                    MessageBox.Show("Could not initialize the texture shader object.");
                    return false;
                }

                // Create the bitmap object.
                Bitmap = new DBitmap();

                // Initialize the bitmap object.
                if (!Bitmap.Initialize(D3D.Device, configuration.Width, configuration.Height, "seafloor.dds", 256, 256))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not initialize Direct3D\nError is '" + ex.Message + "'");
                return false;
            }
        }
        public void ShutDown()
        {
            Timer = null;
            Camera = null;

            // Release the color shader object.
            TextureShader?.ShutDown();
            TextureShader = null;
            // Release the model object.
            Bitmap?.Shutdown();
            Bitmap = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Frame()
        {
            // Render the graphics scene.
            return Render();
        }
        private bool Render()
        {
            // Clear the buffer to begin the scene.
            D3D.BeginScene(0f, 0f, 0f, 1f);

            // Generate the view matrix based on the camera position.
            Camera.Render();

            // Get the world, view, and projection matrices from camera and d3d objects.
            Matrix viewMatrix = Camera.ViewMatrix;
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;
            Matrix orthoMatrix = D3D.OrthoMatrix;

            // Turn off the Z buffer to begin all 2D rendering.
            D3D.TurnZBufferOff();

            // Put the bitmap vertex and index buffers on the graphics pipeline to prepare them for drawing.
            if (!Bitmap.Render(D3D.DeviceContext, 100, 100))
                return false;

            // Render the bitmap with the texture shader.
            if (!TextureShader.Render(D3D.DeviceContext, Bitmap.IndexCount, worldMatrix, viewMatrix, orthoMatrix, Bitmap.Texture.TextureResource))
                return false;

            // Turn the Z buffer back on now that all 2D rendering has completed.
            D3D.TurnZBufferOn();

            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }
    }
}