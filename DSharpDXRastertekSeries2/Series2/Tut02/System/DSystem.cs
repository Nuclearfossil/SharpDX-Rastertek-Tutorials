using DSharpDXRastertek.Series2.Tut02.Graphics;
using DSharpDXRastertek.Series2.Tut02.Input;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSharpDXRastertek.Series2.Tut02.System
{
    public class DSystem
    {
        private RenderForm RenderForm { get; set; }
        public DSystemConfiguration Configuration { get; private set; }
        public DInput Input { get; private set; }
        public DGraphics Graphics { get; private set; }
        public DTimer Timer { get; private set; }

        public DSystem() { }

        public static void StartRenderForm(string title, int width, int height, bool vSync, bool fullScreen = true, int testTimeSeconds = 0)
        {
            DSystem system = new DSystem();
            system.Initialize(title, width, height, vSync, fullScreen, testTimeSeconds);
            system.RunRenderForm();
        }
        public virtual bool Initialize(string title, int width, int height, bool vSync, bool fullScreen, int testTimeSeconds)
        {
            bool result = false;

            Configuration = new DSystemConfiguration(title, width, height, fullScreen, vSync);
            InitializeWindows(title);
            RenderForm.BackColor = Color.Black;
            Input = new DInput();
            result = Input.Initialize();
            Graphics = new DGraphics();
            result = Graphics.Initialize(Configuration);
            Timer = new DTimer();
            result = Timer.Initialize();

            return result;
        }
        private void InitializeWindows(string title)
        {
            int width = Screen.PrimaryScreen.Bounds.Width;
            int height = Screen.PrimaryScreen.Bounds.Height;

            RenderForm = new RenderForm(title)
            {
                ClientSize = new Size(Configuration.Width, Configuration.Height),
                FormBorderStyle = FormBorderStyle.None
            };

            RenderForm.Show();
            RenderForm.Location = new Point((width / 2) - (Configuration.Width / 2), (height / 2) - (Configuration.Height / 2));
        }
        private void RunRenderForm()
        {
            RenderForm.KeyDown += (s, e) => Input.KeyDown(e.KeyCode);
            RenderForm.KeyUp += (s, e) => Input.KeyUp(e.KeyCode);

            RenderLoop.Run(RenderForm, () =>
            {
                if (!Frame())
                    ShutDown();
            });
        }
        public bool Frame()
        {
            if (Input.IsKeyDown(Keys.Escape))
                return false;

            Timer.Frame2();
            if (Timer.CumulativeFrameTime >= (1 * 1000))
                return false;

            return Graphics.Frame();
        }
        public void ShutDown()
        {
            ShutdownWindows();
            Timer = null;
            Graphics?.ShutDown();
            Graphics = null;
            Input = null;
        }
        private void ShutdownWindows()
        {
            RenderForm?.Dispose();
            RenderForm = null;
        }
    }
}
