using SharpDX.Windows;
using System.Drawing;
using System.Windows.Forms;
using TestConsole;

namespace DSharpDXRastertek.Series2.TutTerr12.System
{
    public class DSystem
    {
        // Properties
        private RenderForm RenderForm { get; set; }
        public DSystemConfiguration Configuration { get; private set; }
        public DApplication DApplication { get; set; }

        // Constructor
        public DSystem() { }

        public static void StartRenderForm(string title, int width, int height, bool vSync, bool fullScreen = true, int testTimeSeconds = 0)
        {
            DSystem system = new DSystem();
            system.Initialize(title, width, height, vSync, fullScreen, testTimeSeconds);
            system.RunRenderForm();
        }
        // Methods
        public virtual bool Initialize(string title, int width, int height, bool vSync, bool fullScreen, int testTimeSeconds)
        {
            bool result = false;

            if (Configuration == null)
                Configuration = new DSystemConfiguration(title, width, height, fullScreen, vSync);

            // Initialize Window.
            InitializeWindows(title);

            // Create the application wrapper object.
            DApplication = new DApplication();

            // Initialize the application wrapper object.
            if (!DApplication.Initialize(Configuration, RenderForm.Handle))
                return false;

            DPerfLogger.Initialize("RenderForm C# SharpDX: " + Configuration.Width + "x" + Configuration.Height + " VSync:" + DSystemConfiguration.VerticalSyncEnabled + " FullScreen:" + DSystemConfiguration.FullScreen + "   " + RenderForm.Text, testTimeSeconds, Configuration.Width, Configuration.Height); ;

            return result;
        }
        private void InitializeWindows(string title)
        {
            int width = Screen.PrimaryScreen.Bounds.Width;
            int height = Screen.PrimaryScreen.Bounds.Height;

            // Initialize Window.
            RenderForm = new RenderForm(title)
            {
                ClientSize = new Size(Configuration.Width, Configuration.Height),
                FormBorderStyle = FormBorderStyle.None
            };

            // The form must be showing in order for the handle to be used in Input and Graphics objects.
            RenderForm.Show();
            RenderForm.Location = new Point((width / 2) - (Configuration.Width / 2), (height / 2) - (Configuration.Height / 2));
        }
        private void RunRenderForm()
        {
            RenderLoop.Run(RenderForm, () =>
            {
                if (!Frame())
                    ShutDown();
            });
        }
        public bool Frame()
        {
            // Read the user input.
            if (!DApplication.Input.Frame() || DApplication.Input.IsEscapePressed())
                return false;

            // Update the system stats.
            DApplication.Timer.Frame2();
            if (DPerfLogger.IsTimedTest)
            {
                DPerfLogger.Frame(DApplication.Timer.FrameTime);
                if (DApplication.Timer.CumulativeFrameTime >= DPerfLogger.TestTimeInSeconds * 1000)
                    return false;
            }

            // Do the frame processing for the application object.
            if (!DApplication.Frame())
                return false;

            return true;
        }
        public void ShutDown()
        {
            ShutdownWindows();
            DPerfLogger.ShutDown();

            // Release the graphics object.
            DApplication?.Shutdown();
            DApplication = null;
            Configuration = null;
        }
        private void ShutdownWindows()
        {
            RenderForm?.Dispose();
            RenderForm = null;
        }
    }
}
