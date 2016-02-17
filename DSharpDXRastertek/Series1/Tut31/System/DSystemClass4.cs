using DSharpDXRastertek.Tut31.Graphics;
using DSharpDXRastertek.Tut31.Input;
using DSharpDXRastertek.Tut31.Sound;
using SharpDX.Windows;
using System.Drawing;
using System.Windows.Forms;
using TestConsole;

namespace DSharpDXRastertek.Tut31.System
{
    public class DSystem                    // 167 lines
    {
        // Properties
        private RenderForm RenderForm { get; set; }
        public DSystemConfiguration Configuration { get; private set; }
        public DInput Input { get; private set; }
        public DGraphics Graphics { get; private set; }
        public DTimer Timer { get; private set; }
        public DSound Sound { get; private set; }

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

            // Create and initialize Timer.
            Timer = new DTimer();
            if (!Timer.Initialize())
            {
                MessageBox.Show("Could not initialize Timer object", "Error", MessageBoxButtons.OK);
                return false;
            }

            // Create the sound object   sound01.wav  sound02.wav
            Sound = new DWaveSound("sound02.wav");

            // Initialize the sound object.
            if (!Sound.Initialize(RenderForm.Handle))
            {
                MessageBox.Show("Could not initialize Direct Sound", "Error", MessageBoxButtons.OK);
                return false;
            }

            // This seperates out the creation of one DirectSound object and one PrimaryBuffer nad loads as many SecondaryBuffers as there are Sound.LoadAudio Calls. We pass in only the first instanceances DirectSound to play from both secondaryBuffers from the one primaryBuffer.
            Sound.LoadAudio(Sound._DirectSound);
            Sound.Play(0, new SharpDX.Vector3(-2.0f, 0, 0.0f)); // Front Center

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

            // Update the system stats.
            Timer.Frame2();
            if (DPerfLogger.IsTimedTest)
            {
                DPerfLogger.Frame(Timer.FrameTime);
                if (Timer.CumulativeFrameTime >= DPerfLogger.TestTimeInSeconds * 1000)
                    return false;
            }

            // The following Rotates the entire scenes around the speakers.
            // Get current positions of each secondarySound in 3dSpace.
            SharpDX.Vector3 SoundBufferPosition = Sound._3DSecondarySoundBuffer.Position;     
            // Create a reset Matreix in the o,o,o home position.
            SharpDX.Matrix rotationMatrix = SharpDX.Matrix.Identity;
            // Rotate it slightly.
            rotationMatrix = SharpDX.Matrix.RotationY(0.0002f);
            // And apply that rotation towards each coordinate for each secondarySoundBuffer being played in 3D space.
            SharpDX.Vector3 rotatedCoordinates = SharpDX.Vector3.TransformCoordinate(SoundBufferPosition, rotationMatrix);
            // And assign back the rotated coordinates to each Secondary Soundbuffer that will now been roated.
            Sound._3DSecondarySoundBuffer.Position = rotatedCoordinates;
            
            // Finally render the graphics to the screen.
            if (!Graphics.Render())
                return false;

            return true;
        }
        public void ShutDown()
        {
            ShutdownWindows();
            DPerfLogger.ShutDown();

            // Release the Timer object
            Timer = null;

            // Release the sound object
            Sound?.Shutdown();
            Sound = null;
            // Release graphics and related objects.
            Graphics?.Shutdown();
            Graphics = null;
            // Release DriectInput related object.
            Input?.Shutdown();
            Input = null;
        }
        private void ShutdownWindows()
        {
            RenderForm?.Dispose();
            RenderForm = null;
        }
    }
}