using DSharpDXRastertek.Tut07.System;
using SharpDX;
using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut07.Graphics
{
    public class DGraphics                  // 147 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        private DCamera Camera { get; set; }
        private DModel Model { get; set; }
        private DLightShader LightShader { get; set; }
        private DLight Light { get; set; }
        public DTimer Timer { get; set; }

        // Static properties
        private static float D3DX_PI = 3.14159265358979323846f;
        public static float Rotation { get; set; }

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
                Camera.SetPosition(0, 0, -10.0f);

                // Create the model object.
                Model = new DModel();

                // Initialize the model object.
                if (!Model.Initialize(D3D.Device, "Cube.txt", "seafloor.dds"))
                {
                    MessageBox.Show("Could not initialize the model object.");
                    return false;
                } 

                // Create the texture shader object.
                LightShader = new DLightShader();

                // Initialize the texture shader object.
                if (!LightShader.Initialize(D3D.Device, windowsHandle))
                {
                    MessageBox.Show("Could not initialize the texture shader object.");
                    return false;
                }

                // Create the light object.
                Light = new DLight();

                // Iniialize the light object.  Changed to white in Tutorial 7
                Light.SetDiffuseColour(1, 1, 1, 1);
                Light.SetDirection(0, 0, 1);

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
            Light = null;
            Camera = null;

            // Release the LightShader and Light objects.
            LightShader?.ShutDown();
            LightShader = null;
            // Release the model object.
            Model?.Shutdown();
            Model = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Frame()
        {
            // Update the rotation variables each frame.
            Rotate();

            // Render the graphics scene.
            return Render(Rotation);
        }
        private bool Render(float rotation)
        {
            // Clear the buffer to begin the scene.
            D3D.BeginScene(0f, 0f, 0f, 1f);

            // Generate the view matrix based on the camera position.
            Camera.Render();

            // Get the world, view, and projection matrices from camera and d3d objects.
            Matrix viewMatrix = Camera.ViewMatrix;
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;

            // Rotate the world matrix by the rotation value so that the triangle will spin.
            Matrix.RotationY(rotation, out worldMatrix);

            // Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            Model.Render(D3D.DeviceContext);

            // Render the model using the color shader.
            if (!LightShader.Render(D3D.DeviceContext, Model.IndexCount, worldMatrix, viewMatrix, projectionMatrix, Model.Texture.TextureResource, Light.Direction, Light.DiffuseColour))
                return false;

            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }

        // Static Methods
        public static void Rotate()
        {
            Rotation += D3DX_PI * 0.01f;

            if (Rotation > 360)
                Rotation -= 360;
        }
    }
}