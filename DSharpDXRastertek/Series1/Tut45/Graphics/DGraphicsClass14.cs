using DSharpDXRastertek.Tut45.Graphics.Camera;
using DSharpDXRastertek.Tut45.Graphics.Data;
using DSharpDXRastertek.Tut45.Graphics.Models;
using DSharpDXRastertek.Tut45.Graphics.Shaders;
using DSharpDXRastertek.Tut45.System;
using SharpDX;
using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut45.Graphics
{
    public class DGraphics                  // 203 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }

        #region Data
        private DLight Light { get; set; }
        #endregion

        #region Models
        private DModel CubeModel1 { get; set; }
        private DModel CubeModel2 { get; set; }
        private DBumpMapModel CubeBumpMapModel3 { get; set; }
        #endregion

        #region Shaders
        public DShaderManager ShaderManager { get; set; }
        #endregion

        // Static properties
        public static float Rotation { get; set; }

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

                // Create the shader manager object.
                ShaderManager = new DShaderManager();

                // Initialize the shader manager object.
                if (!ShaderManager.Initialize(D3D.Device, windowHandle))
                    return false;
                #endregion

                #region Initialize Camera
                // Create the camera object
                Camera = new DCamera();

                // Set the initial position and rotation of the camera.
                Camera.SetPosition(0.0f, 0.0f, -10.0f);
                #endregion

                #region Data variables.
                // Create the light object.
                Light = new DLight();

                // Initialize the light object.
                Light.SetAmbientColor(0.15f, 0.15f, 0.15f, 1.0f);
                Light.SetDiffuseColor(1.0f, 1.0f, 1.0f, 1.0f);
                Light.Direction = new Vector3(0.0f, 0.0f, 1.0f);
                Light.SetSpecularColor(1.0f, 1.0f, 1.0f, 1.0f);
                Light.SetSpecularPower(64.0f);
                #endregion

                #region Initialize Models
                // Create the ground model object.
                CubeModel1 = new DModel();

                // Initialize the cube model object.
                if (!CubeModel1.Initialize(D3D.Device, "cube.txt", "marble.dds"))
                    return false;

                // Create the second model object.
                CubeModel2 = new DModel();

                // Initialize the cube model object.
                if (!CubeModel2.Initialize(D3D.Device, "cube.txt", "metal.dds"))
                    return false;

                // Create the third bump model object for models with normal maps and related vectors.
                CubeBumpMapModel3 = new DBumpMapModel();

                // Initialize the bump model object.
                if (!CubeBumpMapModel3.Initialize(D3D.Device, "cube.txt", "stone01.dds", "normal.dds"))
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
            // Release the light object.
            Light = null;
            // Release the camera object.
            Camera = null;

            // Release the Second Cube model object.
            CubeModel2?.Shutdown();
            CubeModel2 = null;
            // Release the Third Cube model object.
            CubeBumpMapModel3?.Shutdown();
            CubeBumpMapModel3 = null;
            // Release the First cube model object.
            CubeModel1?.Shutdown();
            CubeModel1 = null;
            // Release the shader manager object.
            ShaderManager?.ShutDown();
            ShaderManager = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Frame()
        {
            Rotate();

            // Render the graphics scene.
            if (!Render())
                return false;

            return true;
        }
        private bool Render()
        {
            // Clear the buffers to begin the scene.
            D3D.BeginScene(0.0f, 0.0f, 0.0f, 1.0f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world, view, and projection matrices from the camera and d3d objects.
            Matrix translationMatrix;
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix viewMatrix = Camera.ViewMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;

            // Setup the rotation and translation of the first model.
            Matrix.RotationY(Rotation, out worldMatrix);
            Matrix.Translation(-3.5f, 0.0f, 0.0f, out translationMatrix);
            Matrix.Multiply(ref worldMatrix, ref translationMatrix, out worldMatrix);

            // Render the first model using the texture shader.
            CubeModel1.Render(D3D.DeviceContext);
            if (!ShaderManager.RenderTextureShader(D3D.DeviceContext, CubeModel1.IndexCount, worldMatrix, viewMatrix, projectionMatrix, CubeModel1.Texture.TextureResource))
                return false;

            // Setup the rotation and translation of the second model by resetting the worldMatrix too.
            worldMatrix = D3D.WorldMatrix;
            Matrix.RotationY(Rotation, out worldMatrix);
            Matrix.Translation(0.0f, 0.0f, 0.0f, out translationMatrix);
            Matrix.Multiply(ref worldMatrix, ref translationMatrix, out worldMatrix);

            // Render the second model using the light shader.
            CubeModel2.Render(D3D.DeviceContext);
            if (!ShaderManager.RenderLightShader(D3D.DeviceContext, CubeModel2.IndexCount, worldMatrix, viewMatrix, projectionMatrix, CubeModel2.Texture.TextureResource, Light.Direction, Light.AmbientColor, Light.DiffuseColour, Camera.GetPosition(), Light.SpecularColor, Light.SpecularPower))
                return false;

            // Setup the rotation and translation of the third model.
            worldMatrix = D3D.WorldMatrix;
            Matrix.RotationY(Rotation, out worldMatrix);
            Matrix.Translation(3.5f, 0.0f, 0.0f, out translationMatrix);
            Matrix.Multiply(ref worldMatrix, ref translationMatrix, out worldMatrix);

            // Render the third model using the bump map shader.
            CubeBumpMapModel3.Render(D3D.DeviceContext);
            if (!ShaderManager.RenderBumpMapShader(D3D.DeviceContext, CubeBumpMapModel3.IndexCount, worldMatrix, viewMatrix, projectionMatrix, CubeBumpMapModel3.ColorTexture.TextureResource, CubeBumpMapModel3.NormalMapTexture.TextureResource, Light.Direction, Light.DiffuseColour))
                return false;
            
            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }

        // Static Methods.
        static void Rotate()
        {
            Rotation += (float)Math.PI * 0.0005f;
            if (Rotation > 360)
                Rotation -= 360;
        }
    }
}