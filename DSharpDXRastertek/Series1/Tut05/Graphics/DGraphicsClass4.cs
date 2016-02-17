using DSharpDXRastertek.Tut05.System;
using SharpDX;
using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut05.Graphics
{
    public class DGraphics                  // 120 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        private DCamera Camera { get; set; }
        private DModel Model { get; set; }
        private DTextureShader TextureShader { get; set; }
        public DTimer Timer { get; set; }

        // Constructor
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
         
                // Set the initial position of the camera.
                Camera.SetPosition(0, 0, -10);

                // Create the model object.
                Model = new DModel();

                // Initialize the model object.
                if (!Model.Initialize(D3D.Device, DSystemConfiguration.DataFilePath + "seafloor.dds"))
                {
                    MessageBox.Show("Could not initialize the model object.");
					return false;
                }

                // Create the texture shader object.
                TextureShader = new DTextureShader();

                // Initialize the texture shader object.
                if (!TextureShader.Initialize(D3D.Device, windowsHandle))
                {
                    MessageBox.Show("Could not initialize the texture shader object.");
					return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        public void ShutDown()
        {
            // Release the Timer object.
            Timer = null;
            // Release the camera object.
            Camera = null;

            // Release the TextureShader Object.
            TextureShader?.ShutDown();
            TextureShader = null;
            // Release the model object.
            Model?.ShutDown();
            Model = null;
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

            // Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            Model.Render(D3D.DeviceContext);

            // Render the model using the color shader.
            if (!TextureShader.Render(D3D.DeviceContext, Model.IndexCount, worldMatrix, viewMatrix, projectionMatrix, Model.Texture.TextureResource))
				return false;

            // Present the rendered scene to the screen.
			D3D.EndScene();

            return true;
        }
    }
}