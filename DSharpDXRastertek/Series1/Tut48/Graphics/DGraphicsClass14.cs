using DSharpDXRastertek.Tut48.Graphics.Camera;
using DSharpDXRastertek.Tut48.Graphics.Data;
using DSharpDXRastertek.Tut48.Graphics.Models;
using DSharpDXRastertek.Tut48.Graphics.Shaders;
using DSharpDXRastertek.Tut48.System;
using SharpDX;
using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut48.Graphics
{
    public class DGraphics                  // 327 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }

        #region Data
        private DLight Light { get; set; }
        private DRenderTexture RenderTexture { get; set; }
        #endregion

        #region Models
        private DModel CubeModel { get; set; }
        private DModel GroundModel { get; set; }
        private DModel SphereModel { get; set; }
        #endregion

        #region Shaders
        public DDepthShader DepthShader { get; set; }
        public DShadowShader ShadowShader { get; set; }
        #endregion     

        #region Variables 
        private float lightPositionX = 9.0f;
        private float lightAngle = 270.0f;
        private int SHADOWMAP_WIDTH = 1024;
        private int SHADOWMAP_HEIGHT = 1024;
        private float SHADOWMAP_DEPTH = 50.0f;
        private float SHADOWMAP_NEAR = 1.0f;
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

                // Set the initial position of the camera.
                Camera.SetPosition(0.0f, 2.0f, -10.0f);
                #endregion

                #region Initialize Models
                // Create the cube model object.
                CubeModel = new DModel();

                // Initialize the cube model object.
                if (!CubeModel.Initialize(D3D.Device, "cube.txt", "wall01.bmp"))
                    return false;

                // Set the position for the cube model.
                CubeModel.SetPosition(-2.0f, 2.0f, 0.0f);

                // Create the sphere model object.
                SphereModel = new DModel();

                // Initialize the sphere model object.
                if (!SphereModel.Initialize(D3D.Device, "sphere.txt", "ice01.bmp"))
                    return false;

                // Set the position for the sphere model.
                SphereModel.SetPosition(2.0f, 2.0f, 0.0f);

                // Create the ground model object.
                GroundModel = new DModel();

                // Initialize the ground model object.
                if (!GroundModel.Initialize(D3D.Device, "plane01.txt", "metal001.bmp"))
                    return false;

                // Set the position for the ground model.
                GroundModel.SetPosition(0.0f, 1.0f, 0.0f);
                #endregion

                #region Data variables.
                // Create the light object.
                Light = new DLight();

                // Initialize the light object.
                Light.SetAmbientColor(0.15f, 0.15f, 0.15f, 1.0f);
                Light.SetDiffuseColor(1.0f, 1.0f, 1.0f, 1.0f);
                Light.GenerateOrthoMatrix(20.0f, SHADOWMAP_DEPTH, SHADOWMAP_NEAR);

                // Create the render to texture object.
                RenderTexture = new DRenderTexture();

                // Initialize the render to texture object.
                if (!RenderTexture.Initialize(D3D.Device, SHADOWMAP_WIDTH, SHADOWMAP_HEIGHT, SHADOWMAP_DEPTH, SHADOWMAP_NEAR))
                    return false;

                // Create the depth shader object.
                DepthShader = new DDepthShader();

                // Initialize the depth shader object.
                if (!DepthShader.Initialize(D3D.Device, windowHandle))
                    return false;
                #endregion

                #region Initialize Shaders
                // Create the shadow shader object.
                ShadowShader = new DShadowShader();

                // Initialize the shadow shader object.
                if (!ShadowShader.Initialize(D3D.Device, windowHandle))
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

            // Release the shadow shader object.
            ShadowShader?.ShutDown();
            ShadowShader = null;
            // Release the depth shader object.
            DepthShader?.ShutDown();
            DepthShader = null;
            /// Release the render to texture object.
            RenderTexture?.Shutdown();
            RenderTexture = null;
            // Release the ground model object.
            GroundModel?.Shutdown();
            GroundModel = null;
            // Release the sphere model object.
            SphereModel?.Shutdown();
            SphereModel = null;
            // Release the cube model object.
            CubeModel?.Shutdown();
            CubeModel = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Frame(float positionX, float positionY, float positionZ, float rotationX, float rotationY, float rotationZ, float frameTime)
        {
            // Set the position of the camera.
            Camera.SetPosition(positionX, positionY, positionZ);
            Camera.SetRotation(rotationX, rotationY, rotationZ);

            // Update the position of the light each frame.
            lightPositionX -= 0.003f * frameTime;

            // Update the angle of the light each frame.
            lightAngle -= 0.03f * frameTime;
            if (lightAngle < 90.0f)
            {   // cycling code
                lightAngle = 270.0f;

                // Reset the light position also.
                lightPositionX = 9.0f;
            }
            float radians = lightAngle * 0.0174532925f;

            // Update the direction of the light.
            Light.Direction = new Vector3((float)Math.Sin(radians), (float)Math.Cos(radians), 0.0f);

            // Set the position and lookat for the light.
            Light.Position = new Vector3(lightPositionX, 8.0f, -0.1f);
            Light.SetLookAt(-lightPositionX, 0.0f, 0.0f);

            // Render the graphics scene.
            if (!Render())
                return false;

            return true;
        }
        public bool Render()
        {
            // First render the scene to a texture.
            if (!RenderSceneToTexture())
                return false;

            // Clear the buffers to begin the scene.
            D3D.BeginScene(0.0f, 0.0f, 0.0f, 1.0f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Generate the light view matrix based on the light's position.
            Light.GenerateViewMatrix();

            // Get the world, view, and projection matrices from the camera and d3d objects.
            Matrix viewMatrix = Camera.ViewMatrix;
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;

            // Get the light's view and projection matrices from the light object.
            Matrix lightViewMatrix = Light.ViewMatrix;
            Matrix lightOrthoMatrix = Light.OrthoMatrix;

            // Setup the translation matrix for the cube model.
            Vector3 cubePosition = CubeModel.GetPosition();
            Matrix.Translation(cubePosition.X, cubePosition.Y, cubePosition.Z, out worldMatrix);

            // Put the cube model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            CubeModel.Render(D3D.DeviceContext);

            // Render the model using the shadow shader.
            if (!ShadowShader.Render(D3D.DeviceContext, CubeModel.IndexCount, worldMatrix, viewMatrix, projectionMatrix, lightViewMatrix, lightOrthoMatrix, CubeModel.Texture.TextureResource, RenderTexture.ShaderResourceView, Light.Direction, Light.AmbientColor, Light.DiffuseColour))
                return false;

            // Reset the world matrix.
            worldMatrix = D3D.WorldMatrix;

            // Setup the translation matrix for the sphere model.
            Vector3 spherePosition = SphereModel.GetPosition();
            Matrix.Translation(spherePosition.X, spherePosition.Y, spherePosition.Z, out worldMatrix);

            // Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            SphereModel.Render(D3D.DeviceContext);
            if (!ShadowShader.Render(D3D.DeviceContext, SphereModel.IndexCount, worldMatrix, viewMatrix, projectionMatrix, lightViewMatrix, lightOrthoMatrix, SphereModel.Texture.TextureResource, RenderTexture.ShaderResourceView, Light.Direction, Light.AmbientColor, Light.DiffuseColour))
                return false;

            // Reset the world matrix.
            worldMatrix = D3D.WorldMatrix;

            // Setup the translation matrix for the ground model.
            Vector3 groundPosition = GroundModel.GetPosition();
            Matrix.Translation(groundPosition.X, groundPosition.Y, groundPosition.Z, out worldMatrix);

            // Render the ground model using the shadow shader.
            GroundModel.Render(D3D.DeviceContext);
            if (!ShadowShader.Render(D3D.DeviceContext, GroundModel.IndexCount, worldMatrix, viewMatrix, projectionMatrix, lightViewMatrix, lightOrthoMatrix, GroundModel.Texture.TextureResource, RenderTexture.ShaderResourceView, Light.Direction, Light.AmbientColor, Light.DiffuseColour))
                return false;

            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }
        private bool RenderSceneToTexture()
        {
            // Set the render target to be the render to texture.
            RenderTexture.SetRenderTarget(D3D.DeviceContext);

            // Clear the render to texture.
            RenderTexture.ClearRenderTarget(D3D.DeviceContext, 0.0f, 0.0f, 0.0f, 1.0f);

            // Generate the light view matrix based on the light's position.
            Light.GenerateViewMatrix();

            // Get the world matrix from the d3d object.
            Matrix worldMareix = D3D.WorldMatrix;

            // Get the view and orthographic matrices from the light object.
            Matrix lightViewMatrix = Light.ViewMatrix;
            Matrix lightOrthoMatrix = Light.OrthoMatrix;

            // Setup the translation matrix for the cube model.
            Vector3 cubePosition = CubeModel.GetPosition();
            Matrix.Translation(cubePosition.X, cubePosition.Y, cubePosition.Z, out worldMareix);

            // Render the cube model with the depth shader.
            CubeModel.Render(D3D.DeviceContext);
            if (!DepthShader.Render(D3D.DeviceContext, CubeModel.IndexCount, worldMareix, lightViewMatrix, lightOrthoMatrix))
                return false;

            // Reset the world matrix.
            worldMareix = D3D.WorldMatrix;

            // Setup the translation matrix for the sphere model.
            Vector3 spherePosition = SphereModel.GetPosition();
            Matrix.Translation(spherePosition.X, spherePosition.Y, spherePosition.Z, out worldMareix);

            // Render the sphere model with the depth shader.
            SphereModel.Render(D3D.DeviceContext);
            if (!DepthShader.Render(D3D.DeviceContext, SphereModel.IndexCount, worldMareix, lightViewMatrix, lightOrthoMatrix))
                return false;

            // Reset the world matrix.
            worldMareix = D3D.WorldMatrix;

            // Setup the translation matrix for the ground model.
            Vector3 groundPosition = GroundModel.GetPosition();
            Matrix.Translation(groundPosition.X, groundPosition.Y, groundPosition.Z, out worldMareix);

            // Render the ground model with the depth shader.
            GroundModel.Render(D3D.DeviceContext);
            if (!DepthShader.Render(D3D.DeviceContext, GroundModel.IndexCount, worldMareix, lightViewMatrix, lightOrthoMatrix))
                return false;

            // Reset the render target back to the original back buffer and not the render to texture anymore.
            D3D.SetBackBufferRenderTarget();

            // Reset the viewport back to the original.
            D3D.ResetViewPort();

            return true;
        }
    }
}