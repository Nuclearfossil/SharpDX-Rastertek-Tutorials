using DSharpDXRastertek.Tut30.Graphics.Camera;
using DSharpDXRastertek.Tut30.Graphics.Data;
using DSharpDXRastertek.Tut30.Graphics.Models;
using DSharpDXRastertek.Tut30.Graphics.Shaders;
using DSharpDXRastertek.Tut30.System;
using SharpDX;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut30.Graphics
{
    public class DGraphics                  // 187 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }

        #region Data
        private DLight Light1 { get; set; }
        private DLight Light2 { get; set; }
        private DLight Light3 { get; set; }
        private DLight Light4 { get; set; }
        #endregion

        #region Models
        private DModel Model { get; set; }
        #endregion

        #region Shaders
        private DLightShader LightShader { get; set; }
        #endregion     

        #region Variables
        private Vector4[] lightDiffuseColors = null;
        private Vector4[] lightPositions = null;
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
 
                // Set the position and rotation of the camera;
                Camera.SetPosition(0.0f, 2.0f, -12.0f); // 0.0f, 4.0f, -12.0f
                #endregion

                #region Initialize Models
                // Create the Flat Plane  model class.
                Model = new DModel();

                //// Initialize the ground model object.
                if (!Model.Initialize(D3D.Device, "plane01.txt", new[] { "stone01.bmp" }))
                {
                    MessageBox.Show("Could not initialize the ground model object", "Error", MessageBoxButtons.OK);
                    return false;
                }
                #endregion

                #region Initialize Shaders
                // Create the light shader object.
                LightShader = new DLightShader();
                
                // Initialize the light shader object.
                if (!LightShader.Initialize(D3D.Device, windowHandle))
                {
                    MessageBox.Show("Could not initialize the light shader object.", "Error", MessageBoxButtons.OK);
                    return false;
                }
                #endregion

                #region Initialize Data
                // Create the first light object.
                Light1 = new DLight();
                // Initialize the first light as a red Light.
                Light1.SetDiffuseColor(1.0f, 0.0f, 0.0f, 1.0f);
                Light1.SetPosition(-3.0f, 1.0f, 3.0f);

                // Create the second light object.
                Light2 = new DLight();
                // Initialize the second light as a green Light.
                Light2.SetDiffuseColor(0.0f, 1.0f, 0.0f, 1.0f);
                Light2.SetPosition(3.0f, 1.0f, 3.0f);

                // Create the third light object.
                Light3 = new DLight();
                // Initialize the third light to a blue Light.
                Light3.SetDiffuseColor(0.0f, 0.0f, 1.0f, 1.0f);
                Light3.SetPosition(-3.0f, 1.0f, -3.0f);

                // Create the fourth light object.
                Light4 = new DLight();
                // Initialize the fourth light as a white Light.
                Light4.SetDiffuseColor(1.0f, 1.0f, 0.0f, 1.0f);
                Light4.SetPosition(3.0f, 1.0f, -3.0f);

                // Prep rendering variables here once instead of every frame.
                lightDiffuseColors = new Vector4[LightShader.NumLights];
                lightPositions = new Vector4[LightShader.NumLights];

                // Create the diffuse color array from the four light colors.
                lightDiffuseColors[0] = Light1.DiffuseColour;
                lightDiffuseColors[1] = Light2.DiffuseColour;
                lightDiffuseColors[2] = Light3.DiffuseColour;
                lightDiffuseColors[3] = Light4.DiffuseColour;

                // Create the light position array from the four light positions.
                lightPositions[0] = Light1.Position;
                lightPositions[1] = Light2.Position;
                lightPositions[2] = Light3.Position;
                lightPositions[3] = Light4.Position;
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
            // Release the light object.
            Light1 = null;
            Light2 = null;
            Light3 = null;
            Light4 = null;

            //// Release the light shader object.
            LightShader?.ShutDown();
            LightShader = null;
            // Release the model object.
            Model?.Shutdown();
            Model = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Frame()
        {
            return true;
        }
        public bool Render()
        {
            // Clear the buffer to begin the scene.
            D3D.BeginScene(0, 0, 0, 1f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world, view, and projection matrices from the camera and d3d objects.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix viewMatrix = Camera.ViewMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;

            // Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            Model.Render(D3D.DeviceContext);

            // Render the model using the light shader and the light arrays.
            LightShader.Render(D3D.DeviceContext, Model.IndexCount, worldMatrix, viewMatrix, projectionMatrix, Model.TextureCollection.Select(item => item.TextureResource).First(), lightDiffuseColors, lightPositions);

            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }
    }
}