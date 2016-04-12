using DSharpDXRastertek.Series2.Tut03.System;
using System;

namespace DSharpDXRastertek.Series2.Tut03.Graphics
{
    public class DGraphics
    {
        private DDX11 D3D { get; set; }
        public DTimer Timer { get; set; }

        public DGraphics() { }

        public bool Initialize(DSystemConfiguration consifguration, IntPtr windowsHandle)
        {
            bool result = false;

            D3D = new DDX11();
            result = D3D.Initialize(consifguration, windowsHandle);
            Timer = new DTimer();
            result = Timer.Initialize();

            return result;
        }
        public void ShutDown()
        {
            Timer = null;
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Frame()
        {
            return Render();
        }
        private bool Render()
        {
            D3D.BeginScene(0.5f, 0.5f, 0.5f, 1.0f);
            D3D.EndScene();
            return true;
        }
    }
}
