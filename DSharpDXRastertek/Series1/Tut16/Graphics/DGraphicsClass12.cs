using DSharpDXRastertek.Tut16.Graphics.Camera;
using DSharpDXRastertek.Tut16.Graphics.Data;
using DSharpDXRastertek.Tut16.Graphics.Models;
using DSharpDXRastertek.Tut16.Graphics.Shaders;
using DSharpDXRastertek.Tut16.Input;
using DSharpDXRastertek.Tut16.System;
using SharpDX;
using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut16.Graphics
{
    public class DGraphics                  // 222 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        private DCamera Camera { get; set; }
        private DModel Model { get; set; }
        private DLightShader LightShader { get; set; }
        private DLight Light { get; set; }
        public DTextClass Text { get; set; }
        private DModelList ModelList { get; set; }
        private DFrustum Frustum { get; set; }

        // Static properties
        public static float Rotation { get; set; }

        // Construtor
        public DGraphics() { }

        // Methods.
        public bool Initialize(DSystemConfiguration configuration, IntPtr windowHandle)
        {
            try
            {
                // Create the Direct3D object.
                D3D = new DDX11();
                
                // Initialize the Direct3D object.
                if (!D3D.Initialize(configuration, windowHandle))
                    return false;

                // Create the camera object
                Camera = new DCamera();

                // Initialize a base view matrix the camera for 2D user interface rendering.
                Camera.SetPosition(0, 0, -1);
                Camera.Render();
                var baseViewMatrix = Camera.ViewMatrix;

                // Create the text object.
                Text = new DTextClass();
                if (!Text.Initialize(D3D.Device, D3D.DeviceContext, windowHandle, configuration.Width, configuration.Height, baseViewMatrix))
                    return false;

                // Create the model class.
                Model = new DModel();

                // Initialize the model object.
                if (!Model.Initialize(D3D.Device, "sphere.txt", "seafloor.bmp"))
                {
                    MessageBox.Show("Could not initialize the model object", "Error", MessageBoxButtons.OK);
                    return false;
                }

                // Create the light shader object.
                LightShader = new DLightShader();

                // Initialize the light shader object.
                if (!LightShader.Initialize(D3D.Device, windowHandle))
                {
                    MessageBox.Show("Could not initialize the light shader", "Error", MessageBoxButtons.OK);
                    return false;
                }

                // Create the light object.
                Light = new DLight();

                // Initialize the light object.
                Light.SetDirection(0.0f, 0.0f, 1.0f);
   
                // Create the model list object.
                ModelList = new DModelList();

                // Initialize the model list object.
                if (!ModelList.Initialize(25))
                {
                    MessageBox.Show("Could not initialize the model list object", "Error", MessageBoxButtons.OK);
                    return false;
                }

                // Create the frustum object.
                Frustum = new DFrustum();

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
            // Release the frustum object.
            Frustum = null;
            // Release the light object.
            Light = null;
            Camera = null;

            // Release the model list object.
            ModelList?.Shutdown();
            ModelList = null;
            // Release the light shader object.
            LightShader?.ShutDown();
            LightShader = null;
            // Release the model object.
            Model?.Shutdown();
            Model = null;
            // Release the text object.
            Text?.Shutdown();
            Text = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        internal bool Frame(DPosition Position)
        {
            // Set the position of the camera.
            Camera.SetPosition(0, 0, -10f);

            //// Set the rotation of the camera.
            Camera.SetRotation(Position.RotationX, Position.RotationY, 0);

            return true;
        }
        public bool Render()
        {
            // Clear the buffer to begin the scene.
            D3D.BeginScene(0f, 0f, 0f, 1f);

            // Generate the view matrix based on the camera position.
            Camera.Render();

            // Get the world, view, and projection matrices from camera and d3d objects.
            var viewMatrix = Camera.ViewMatrix;
            var worldMatrix = D3D.WorldMatrix;
            var projectionMatrix = D3D.ProjectionMatrix;
            var orthoMatrix = D3D.OrthoMatrix;

            // Construct the frustum.
            Frustum.ConstructFrustum(DSystemConfiguration.ScreenDepth, projectionMatrix, viewMatrix);

            // Initialize the count of the models that have been rendered.
            var renderCount = 0;

            Vector3 position;
            Vector4 color;
            
            // Go through all models and render them only if they can seen by the camera view.
            for (int index = 0; index < ModelList.ModelCount; index++)
            {
                // Get the position and color of the sphere model at this index.
                ModelList.GetData(index, out position, out color);

                // Before checking whether this model is in the view to render, adjust the position of the model to the newly rotated camera view to see if it needs to be rendered this frame or not.
                position = Vector3.TransformCoordinate(position, worldMatrix);

                // Set the radius of the sphere to 1.0 since this is already known.
                var radius = 1.0f;

                // Check if the sphere model is in the view frustum.
                bool renderModel = Frustum.CheckSphere(position, radius);

                // If it can be seen then render it, if not skip this model and check the next sphere.
                if (renderModel)
                {
                    // Move the model to the location it should be rendered at.
                    worldMatrix *= Matrix.Translation(position);

                    // Put the model vertex and index buffer on the graphics pipeline to prepare them for drawing.
                    Model.Render(D3D.DeviceContext);

                    // Render the model using the color shader.
                    if (!LightShader.Render(D3D.DeviceContext, Model.IndexCount, worldMatrix, viewMatrix, projectionMatrix, Model.Texture.TextureResource, Light.Direction, color))
                        return false;

                    // Reset to the original world matrix.
                    worldMatrix = D3D.WorldMatrix * Matrix.RotationY(Rotation);

                    // Since this model was rendered then increase the count for this frame.
                    renderCount++;
                }
            }

            // Set the number of the models that was actually rendered this frame.
            if (!Text.SetRenderCount(renderCount, D3D.DeviceContext))
                return false;

            // Turn off the Z buffer to begin all 2D rendering.
            D3D.TurnZBufferOff();

            // Turn on the alpha blending before rendering the text.
            D3D.TurnOnAlphaBlending();

            // Render the text string.
            if (!Text.Render(D3D.DeviceContext, D3D.WorldMatrix, orthoMatrix))
                return false;

            // Turn off the alpha blending before rendering the text.
            D3D.TurnOffAlphaBlending();

            // Turn on the Z buffer to begin all 2D rendering.
            D3D.TurnZBufferOn();

            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }
    }
}