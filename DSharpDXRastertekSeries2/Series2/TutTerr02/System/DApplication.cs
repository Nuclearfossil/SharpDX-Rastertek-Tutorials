using DSharpDXRastertek.Series2.TutTerr02.Graphics;
using DSharpDXRastertek.Series2.TutTerr02.Graphics.Input;
using DSharpDXRastertek.Series2.TutTerr02.Graphics.Shaders;
using System;

namespace DSharpDXRastertek.Series2.TutTerr02.System
{
    public class DApplication
    {
        // Properties
        public DInput Input { get; private set; }
        private DDX11 D3D { get; set; }
        public DTimer Timer { get; set; }
        public DFPS FPS { get; set; }
        public DShaderManager ShaderManager { get; set; }
        public DZone Zone { get; set; }

        public bool Initialize(DSystemConfiguration configuration, IntPtr windowHandle)
        {
            // Create the input object.  The input object will be used to handle reading the keyboard and mouse input from the user.
            Input = new DInput();
            // Initialize the input object.
            if (!Input.Initialize(configuration, windowHandle))
                return false;

            // Create the Direct3D object.
            D3D = new DDX11();
            // Initialize the Direct3D object.
            if (!D3D.Initialize(configuration, windowHandle))
                return false;

            // Create the shader manager object.
            ShaderManager = new DShaderManager();
            // Initialize the shader manager object.
            if (!ShaderManager.Initilize(D3D, windowHandle))
                return false;

            // Create and initialize Timer.
            Timer = new DTimer();
            if (!Timer.Initialize())
                return false;

            // Create the fps object.
            FPS = new DFPS();
            FPS.Initialize();

            // Create and Initialize the Zone object.
            Zone = new DZone();
            if (!Zone.Initialze(D3D, windowHandle, configuration))
                return false;

            return true;
        }
        public void Shutdown()
        {
            // Release the zone object.
            Zone?.ShutDown();
            Zone = null;
            // Release the fps object.
            FPS = null;
            // Release the timer object.
            Timer = null;
            // Release the shader manager object.
            ShaderManager?.ShutDown();
            ShaderManager = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
            // Release the input object.
            Input?.Shutdown();
            Input = null;
        }
        public bool Frame()
        {
            // Update the system stats.
            FPS.Frame();

            // Do the zone frame processing.
            if (!Zone.Frame(D3D, Input, ShaderManager, Timer.FrameTime, FPS.FPS))
                return false;

            return true;
        }
    }
}
