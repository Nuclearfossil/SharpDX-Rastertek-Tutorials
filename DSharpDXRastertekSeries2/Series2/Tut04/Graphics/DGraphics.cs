using DSharpDXRastertek.Series2.Tut04.System;
using System;

namespace DSharpDXRastertek.Series2.Tut04.Graphics
{
    public class DGraphics
    {
        private DDX11 D3D { get; set; }
        private DCamera Camera { get; set; }
        private DModel Model { get; set; }
        private DColorShader ColorShader { get; set; }
        public DTimer Timer { get; set; }

        public DGraphics() { }

        public bool Initialize(DSystemConfiguration consifguration, IntPtr windowsHandle)
        {
            bool result = false;

            D3D = new DDX11();
            result = D3D.Initialize(consifguration, windowsHandle);
            Camera = new DCamera();
            Camera.SetPosition(0, 0, -10);
            Model = new DModel();
            result = Model.Initialize(D3D.Device);
            ColorShader = new DColorShader();
            result = ColorShader.Initialize(D3D.Device, windowsHandle);
            Timer = new DTimer();
            result = Timer.Initialize();

            return result;
        }
        public void ShutDown()
        {
            Camera = null;
            Timer = null;
            ColorShader?.ShutDown();
            ColorShader = null;
            Model?.ShutDown();
            Model = null;
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Frame()
        {
            return Render();
        }
        private bool Render()
        {
            D3D.BeginScene(0.1f, 0f, 0f, 1f);

            Camera.Render();
            Model.Render(D3D.DeviceContext);
            if (!ColorShader.Render(D3D.DeviceContext, Model.IndexCount, D3D.WorldMatrix, Camera.ViewMatrix, D3D.ProjectionMatrix))
                return false;

            D3D.EndScene();
            return true;
        }
    }
}
