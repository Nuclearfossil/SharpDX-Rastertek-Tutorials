using DSharpDXRastertek.Tut14.System;
using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut14.Graphics
{
    public class DGraphics                  // 81 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        private DCamera Camera { get; set; }
        public DTimer Timer { get; set; }

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

                // Create the Timer
                Timer = new DTimer();

                // Initialize the Timer
                if (!Timer.Initialize())
                    return false;

                // Create the camera object
                Camera = new DCamera();

                // Initialize a base view matrix the camera for 2D user interface rendering.
                Camera.SetPosition(0, 0, -1);
                Camera.Render();
                var baseViewMatrix = Camera.ViewMatrix;
                
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
            Timer = null;
            Camera = null;

            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Frame()
        {
            bool resultMouse = true, resultKeyboard = true;

            // Set the position of the camera.
            Camera.SetPosition(0, 0, -10f);

            return (resultMouse | resultKeyboard);
        }
        public bool Render()
        {
            // Clear the buffer to begin the scene.
            D3D.BeginScene(0f, 0f, 0f, 1f);
            
            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }
    }
}