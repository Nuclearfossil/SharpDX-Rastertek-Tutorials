using DSharpDXRastertek.Tut31.System;
using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut31.Graphics
{
    public class DGraphics                  // 52 lines
    {
        // Properties
        private DDX11 D3D { get; set; }

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
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
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