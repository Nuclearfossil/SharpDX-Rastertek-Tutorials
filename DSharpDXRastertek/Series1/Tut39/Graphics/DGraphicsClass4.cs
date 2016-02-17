using DSharpDXRastertek.Tut39.System;
using SharpDX;
using System;

namespace DSharpDXRastertek.Tut39.Graphics
{
    public class DGraphics                  // 112 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        private DCamera Camera { get; set; }
        public DParticleShader ParticleShader { get; set; }
        public DParticleSystem ParticleSystem { get; set; }

        // Constructor
        public DGraphics() { }

        // Methods
        public bool Initialize(DSystemConfiguration configuration, IntPtr windowsHandle)
        {
            try
            {
                // Create the Direct3D object.
                D3D = new DDX11();

                // Initialize the Direct3D object.
                if (!D3D.Initialize(configuration, windowsHandle))
                    return false;

                // Create the camera object
                Camera = new DCamera();

                // Set the initial position of the camera.
                Camera.SetPosition(0.0f, -1.6f, -5.0f);

                // Create the particle shader object.
                ParticleShader = new DParticleShader();

                // Initialize the particle shader object.
                if (!ParticleShader.Initialize(D3D.Device, windowsHandle))
                    return false;

                // Create the particle system object.
                ParticleSystem = new DParticleSystem();

                // Initialize the particle system object.
                if (!ParticleSystem.Initialize(D3D.Device, "star.dds"))
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
        public void ShutDown()
        {
            // Release the camera object.
            Camera = null;

            // Release the particle system object.
            ParticleSystem?.ShutDown();
            ParticleSystem = null;
            // Release the particle shader object.
            ParticleShader?.ShutDown();
            ParticleShader = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Frame(float frameTime)
        {
            // Run the frame processing for the particle system.
            ParticleSystem.Frame(frameTime, D3D.DeviceContext);
            
            // Render the graphics scene.
            return Render();
        }
        private bool Render()
        {
            // Clear the buffer to begin the scene.
            D3D.BeginScene(0f, 0f, 0f, 1f);

            // Generate the view matrix based on the camera position.
            Camera.Render();

            // Get the world, view, and projection matrices from camera and d3d objects.
			Matrix viewMatrix = Camera.ViewMatrix;
			Matrix worldMatrix = D3D.WorldMatrix;
			Matrix projectionMatrix = D3D.ProjectionMatrix;

            // Turn on alpha blending.
            D3D.TurnOnAlphaBlending();

            // Put the particle system vertex and index buffers on the graphics pipeline to prepare them for drawing.
            ParticleSystem.Render(D3D.DeviceContext);
            
            // Render the model using the texture shader.
            if (!ParticleShader.Render(D3D.DeviceContext, ParticleSystem.IndexCount, worldMatrix, viewMatrix, projectionMatrix, ParticleSystem.Texture.TextureResource))
                return false;

            // Turn off alpha blending.
            D3D.TurnOffAlphaBlending();

            // Present the rendered scene to the screen.
			D3D.EndScene();

            return true;
        }
    }
}