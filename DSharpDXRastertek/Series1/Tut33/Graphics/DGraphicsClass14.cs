using DSharpDXRastertek.Tut33.Graphics.Models;
using DSharpDXRastertek.Tut33.Graphics.Shaders;
using DSharpDXRastertek.Tut33.System;
using SharpDX;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut33.Graphics
{
    public class DGraphics                  // 161 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }

        #region Models
        private DModel Model { get; set; }
        #endregion

        #region Shaders
        public DFireShader FireShader { get; set; }
        #endregion     

        #region Variables
        public  float FrameTime = 0.0f;
        #endregion

        // Construtor
        public DGraphics() { }

        // Methods.
        public bool Initialize(DSystemConfiguration configuration, IntPtr windowHandle)
        {
            try
            {
                #region Initialize System
                // Create the Direct3D object.
                D3D = new DDX11();
                
                // Initialize the Direct3D object.
                if (!D3D.Initialize(configuration, windowHandle))
                    return false;
                #endregion

                #region Initialize Camera
                // Create the camera object
                Camera = new DCamera();

                // Set the position of the camera.
                Camera.SetPosition(0.0f, 0.0f, -5.0f);
                #endregion

                #region Initialize Models
                //// Create the Flat Plane  model class.
                Model = new DModel();

                //// Initialize the ground model object.
                if (!Model.Initialize(D3D.Device, "square.txt", new[] { "fire01.dds", "noise01.dds", "alpha02.dds" }))
                {
                    MessageBox.Show("Could not initialize the ground model object", "Error", MessageBoxButtons.OK);
                    return false;
                }
                #endregion

                #region Initialize Shaders
                 // Create the fire shader object.
                FireShader = new DFireShader();

                if (!FireShader.Initialize(D3D.Device, windowHandle))
                    return false;
                #endregion
  
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
            // Release the camera object.
            Camera = null;

            // Release the GlassShader object.
            FireShader?.ShutDown();
            FireShader = null;
            // Release the model object.
            Model?.Shutdown();
            Model = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Frame()
        {
            // Render the scene.
            if (!Render())
                return false;

            return true;
        }
        public bool Render()
        {
            float distortionScale, distortionBias;
            Vector3 scrollSpeeds, scales;
            Vector2 distortion1, distortion2, distortion3;
            
            // Increment the frame time counter.
             FrameTime +=  0.001f;
             if (FrameTime >= 1000.0f)
                 FrameTime = 0.0f;

            // Set the three scrolling speeds for the three different noise textures.
            // The x value is the scroll speed for the first noise texture. The y value is the scroll speed for the second noise texture. And the z value is the scroll speed for the third noise texture.
            scrollSpeeds = new Vector3(1.3f, 2.1f, 2.3f);

            // Set the three scales which will be used to create the three different noise octave textures.
            scales = new Vector3(1.0f, 2.0f, 3.0f);

            // Set the three different x and y distortion factors for the three different noise textures.
            distortion1 = new Vector2(0.1f, 0.2f);
            distortion2 = new Vector2(0.1f, 0.3f);
            distortion3 = new Vector2(0.1f, 0.1f);

            // The the scale and bias of the texture coordinate sampling perturbation.
            distortionScale = 0.8f;
            distortionBias = 0.5f;

            // Clear the buffers to begin the scene.
            D3D.BeginScene(0, 0, 0, 1f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world, view, and projection matrices from the camera and d3d objects.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix viewMatrix = Camera.ViewMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;

            // Turn on alpha blending for the fire transparency.
            D3D.TurnOnAlphaBlending();

            // Put the square model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            Model.Render(D3D.DeviceContext);

            // Render the square model using the fire shader.
            FireShader.Render(D3D.DeviceContext, Model.IndexCount, worldMatrix, viewMatrix, projectionMatrix, Model.TextureCollection.Select(item => item.TextureResource).ToArray()[0], Model.TextureCollection.Select(item => item.TextureResource).ToArray()[1], Model.TextureCollection.Select(item => item.TextureResource).ToArray()[2], FrameTime, scrollSpeeds, scales, distortion1, distortion2, distortion3, distortionScale, distortionBias);

            // Turn off alpha blending.
            D3D.TurnOffAlphaBlending();

            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }
    }
}