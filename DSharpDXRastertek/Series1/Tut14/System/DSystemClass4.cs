using DSharpDXRastertek.Tut14.Graphics;
using DSharpDXRastertek.Tut14.Input;
using DSharpDXRastertek.Tut14.Sound;
using SharpDX.Windows;
using System.Drawing;
using System.Windows.Forms;
using TestConsole;

namespace DSharpDXRastertek.Tut14.System
{
    public class DSystem                    // 152 lines
    {
        // Properties
        private RenderForm RenderForm { get; set; }
        public DSystemConfiguration Configuration { get; private set; }
        public DInput Input { get; private set; }
        public DGraphics Graphics { get; private set; }
        public DSound Sound { get; private set; }

        // Statuc Properties
        public static bool IsMouseOffScreen { get; set; }

        // Constructor
        public DSystem() { }

        // Static Methods
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

            if (Input == null)
            {
                Input = new DInput();
                if (!Input.Initialize(Configuration, RenderForm.Handle))
                    return false;
            }
            if (Graphics == null)
            {
                Graphics = new DGraphics();
                result = Graphics.Initialize(Configuration, RenderForm.Handle);
            }

            DPerfLogger.Initialize("RenderForm C# SharpDX: " + Configuration.Width + "x" + Configuration.Height + " VSync:" + DSystemConfiguration.VerticalSyncEnabled + " FullScreen:" + DSystemConfiguration.FullScreen + "   " + RenderForm.Text, testTimeSeconds, Configuration.Width, Configuration.Height);;

            // Create the sound object   sound01.wav
            Sound = new DWaveSound("sound01.wav");

            // Initialize the sound object.
            if (!Sound.Initialize(RenderForm.Handle))
            {
                MessageBox.Show("Could not initialize Direct Sound", "Error", MessageBoxButtons.OK);
                return false;
            }

            // This seperates out the creation of one DirectSound object and one PrimaryBuffer nad loads as many SecondaryBuffers as there are Sound.LoadAudio Calls. We pass in only the first instanceances DirectSound to play from both secondaryBuffers from the one primaryBuffer.
            Sound.LoadAudio(Sound._DirectSound);
            Sound.Play(0);

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
                FormBorderStyle = DSystemConfiguration.BorderStyle
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
            // Check if the user pressed escape and wants to exit the application.
            if (!Input.Frame() || Input.IsEscapePressed())
                return false;

            // Performance Logging.
            Graphics.Timer.Frame2();
            if (DPerfLogger.IsTimedTest)
            {
                DPerfLogger.Frame(Graphics.Timer.FrameTime);
                if (Graphics.Timer.CumulativeFrameTime >= DPerfLogger.TestTimeInSeconds * 1000)
                    return false;
            }

            // Do the frame processing for the graphics object.mouseX, mouseY, Input.PressedKeyss
            if (!Graphics.Frame())
                return false;

            // Finally render the graphics to the screen.   mouseX, mouseY
            if (!Graphics.Render())
                return false;

            return true;
        }
        public void ShutDown()
        {
            ShutdownWindows();
            DPerfLogger.ShutDown();

            // Release the sound object
            Sound?.Shutdown();
            Sound = null;
            // Release graphics and related objects.
            Graphics?.Shutdown();
            Graphics = null;
            // Release DriectInput related object.
            Input?.Shutdown();
            Input = null;
            Configuration = null;
        }
        private void ShutdownWindows()
        {
            if (RenderForm != null)
                RenderForm.Dispose();

            RenderForm = null;
        }
    }
}