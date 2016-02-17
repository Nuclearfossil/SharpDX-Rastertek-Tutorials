using DSharpDXRastertek.Tut03.System;
using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut03.Graphics
{
    public class DGraphics                  // 65 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        public DTimer Timer { get; set; }

        // Constructor
        public DGraphics() { }

        public bool Initialize(DSystemConfiguration consifguration, IntPtr windowsHandle)
        {
            try
            {
                // Create the Direct3D object.
                D3D = new DDX11();

                // Initialize the Direct3D object.
                if (!D3D.Initialize(consifguration, windowsHandle))
                    return false;

                // Create the Timer
                Timer = new DTimer();

                // Initialize the Timer
                if (!Timer.Initialize())
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
            D3D.BeginScene(0.5f, 0.5f, 0.5f, 1.0f);
           
            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }
    }
}