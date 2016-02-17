using DSharpDXRastertek.Tut47.Graphics.Camera;
using DSharpDXRastertek.Tut47.Graphics.Data;
using DSharpDXRastertek.Tut47.Graphics.Models;
using DSharpDXRastertek.Tut47.Graphics.Shaders;
using DSharpDXRastertek.Tut47.Input;
using DSharpDXRastertek.Tut47.System;
using SharpDX;
using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut47.Graphics
{
    public class DApplication                   // 303 linwa
    {
        // Variables
        bool beginMouseChexk;
        int screenWidth, screenHeight;

        // Properties
        public DInput Input { get; private set; }
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }

        #region Models
        public DModel Model { get; set; }
        private DBitmap BitMap { get; set; }
        public DLight Light { get; set; }
        public DTextClass Text { get; set; }
        #endregion

        #region Shaders
        private DTextureShader TextureShader { get; set; }
        public DLightShader LightShader { get; set; }
        #endregion     

        // Construtor
        public DApplication() { }

        // Methods.
        public bool Initialize(DSystemConfiguration configuration, IntPtr windowHandle)
        {
            // Set the size to sample down to.
            screenWidth = configuration.Width;
            screenHeight = configuration.Height;

            try
            {
                // Create the input object.
                Input = new DInput();

                // Initialize the input object.
                if (!Input.Initialize(configuration, windowHandle))
                    return false;   

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
                Camera.SetPosition(0.0f, 0.0f, -10.0f);
                Camera.Render();
                #endregion

                // Create the model object.
                Model = new DModel();

                // Initialize the model object.
                if (!Model.Initialize(D3D.Device, "sphere.txt", "blue.dds"))
                    return false;

                // Create the texture shader object.
                TextureShader = new DTextureShader();

                // Initialize the texture shader object.
                if (!TextureShader.Initialize(D3D.Device, windowHandle))
                    return false;

                // Create the light shader object.
                LightShader = new DLightShader();

                // Initialize the light shader object.
                if (!LightShader.Initialize(D3D.Device, windowHandle))
                    return false;

                // Create the light object.
                Light = new DLight();

                // Initialize the light object.
                Light.Direction = new Vector3(0.0f, 0.0f, 1.0f);

                // Create the text object.
                Text = new DTextClass();

                // Initialize the text object.
                if (!Text.Initialize(D3D.Device, D3D.DeviceContext, windowHandle, configuration.Width, configuration.Height, Camera.ViewMatrix))
                    return false;

                // Create the bitmap object as the mouse pointer.
                BitMap = new DBitmap();

                // Initialize the bitmap object.
                if (!BitMap.Initialize(D3D.Device, configuration.Width, configuration.Height, "mouse.dds", 32, 32))
                    return false;

                // Initialize that the user has not clicked on the screen to try an intersection test yet.
                beginMouseChexk = false;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not initialize Direct3D\nError is '" + ex.Message + "'");
                return false;
            }
        }
        public void ShutDown()
        {
            // Release the light object.
            Light = null;
            // Release the camera object.
            Camera = null;

            // Release the model object.
            BitMap?.Shutdown();
            BitMap = null;
            // Release the text object.
            Text?.Shutdown();
            Text = null;
            // Release the light shader object.
            LightShader?.ShutDown();
            LightShader = null;
            // Release the model object.
            Model?.Shutdown();
            Model = null;
            // Release the input object.
            Input?.Shutdown();
            Input = null;
            // Release the texture shader object.
            TextureShader?.ShutDown();
            TextureShader = null;
            // Release the Direct3D object.
            D3D?.ShutDown();
            D3D = null;
        }
        public bool Frame()
        {
            // Handle the input processing.
            if (!HandleInput())
                return false;

            // Render the graphics scene.
            if (!Render())
                return false;

            return true;
        }
        private bool HandleInput()
        {
            // Check if the left mouse button has been pressed.
            if (Input.IsMouseButtonDown())
            {
                // If they have clicked on the screen with the mouse then perform an intersection test.
                if (!beginMouseChexk)
                {
                    beginMouseChexk = true;
                    TestIntersection(Input._MouseX, Input._MouseY);
                }
            }

            // Check if the left mouse button has been released.
            if (!Input.IsMouseButtonDown())
                beginMouseChexk = false;

            return true;
        }
        private void TestIntersection(int mouseX, int mouseY)
        {
            // Move the mouse cursor coordinates into the -1 to +1 range.
            float pointX = ((2.0f * (float)mouseX) / (float)screenWidth) - 1.0f;
            float pointY = (((2.0f * (float)mouseY) / (float)screenHeight) - 1.0f) * -1.0f;

            // Adjust the points using the projection matrix to account for the aspect ratio of the viewport.
            pointX = pointX / D3D.ProjectionMatrix.M11;
            pointY = pointY / D3D.ProjectionMatrix.M22;

            // Get the inverse of the view matrix.
            Matrix inverseViewMatrix = Matrix.Invert(Camera.ViewMatrix);

            // Calculate the direction of the picking ray in view space.
            Vector3 direction = new Vector3();
            direction.X = (pointX * inverseViewMatrix.M11) + (pointY * inverseViewMatrix.M21) + inverseViewMatrix.M31;
            direction.Y = (pointX * inverseViewMatrix.M12) + (pointY * inverseViewMatrix.M22) + inverseViewMatrix.M32;
            direction.Z = (pointX * inverseViewMatrix.M23) + (pointY * inverseViewMatrix.M23) + inverseViewMatrix.M33;

            // Get the origin of the picking ray which is the position of the camera.
            Vector3 origin = Camera.GetPosition();

            // Get the world matrix and translate to the location of the sphere.
            Matrix translateMatrix = Matrix.Translation(-5.0f, 1.0f, 5.0f);
            Matrix worldMatrix = Matrix.Multiply(D3D.WorldMatrix, translateMatrix);

            // Now get the inverse of the translated world matrix.
            Matrix inversedWorldMatrix = Matrix.Invert(worldMatrix);

            // Now transform the ray origin and the ray direction from view space to world space.
            Vector3 rayOrigin = Vector3.TransformCoordinate(origin, inversedWorldMatrix);
            Vector3 rayDirection = Vector3.TransformNormal(direction, inversedWorldMatrix);

            // Normalize the ray direction.
            rayDirection = Vector3.Normalize(rayDirection);

            // Now perform the ray-sphere intersection test.
            // If it does intersect then set the intersection to "yes" in the text string that is displayed to the screen.
            // If not then set the intersection to "No".
            if (RaySphereIntersect(rayOrigin, rayDirection, 1.0f))
                Text.SetIntersection(true, D3D.DeviceContext);  
            else
                Text.SetIntersection(false, D3D.DeviceContext);
        }
        private bool RaySphereIntersect(Vector3 rayOrigin, Vector3 rayDirection, float radius)
        {
            // Calculate the a, b, and c coefficients.
           float a = (rayDirection.X * rayDirection.X) + (rayDirection.Y * rayDirection.Y) + (rayDirection.Z * rayDirection.Z);
           float b = ((rayDirection.X * rayOrigin.X) + (rayDirection.Y * rayOrigin.Y) + (rayDirection.Z * rayOrigin.Z)) * 2.0f;
           float c = ((rayOrigin.X * rayOrigin.X) + (rayOrigin.Y * rayOrigin.Y) + (rayOrigin.Z * rayOrigin.Z)) - (radius * radius);

           // Find the discriminant.
           float discriminant = (b * b) - (4 * a * c);

           // if discriminant is negative the picking ray missed the sphere, otherwise it intersected the sphere.
           if (discriminant < 0.0f)
               return false;

            return true;
        }
        private bool Render()
        {
            // Clear the buffers to begin the scene.
            D3D.BeginScene(0.0f, 0.0f, 0.0f, 1.0f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world, view, and projection and Ortho matrices from the camera and d3d objects.
            Matrix viewMatrix = Camera.ViewMatrix;
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix projectionMatrix = D3D.ProjectionMatrix;
            Matrix orthoMatrix = D3D.OrthoMatrix;

            // Translate to the location of the sphere.
            Matrix translatedWorldMatrix = Matrix.Translation(-5.0f, 1.0f, 5.0f);
            worldMatrix = Matrix.Multiply(worldMatrix, translatedWorldMatrix);

            // Render the model using the light shader.
            Model.Render(D3D.DeviceContext);
            if (!LightShader.Render(D3D.DeviceContext, Model.IndexCount, worldMatrix, viewMatrix, projectionMatrix, Model.Texture.TextureResource, Light.Direction))
                return false;

            // Reset the world matrix.
            worldMatrix = D3D.WorldMatrix;

            // Turn off the Z buffer to begin all 2D rendering.
            D3D.TurnZBufferOff();
            
            // Turn on alpha blending.
            D3D.TurnOnAlphaBlending();

            // Get the location of the mouse from the input object,
            int mouseX = Input._MouseX;
            int mouseY = Input._MouseY;

            // Render the mouse cursor with the texture shader.
            if(!BitMap.Render(D3D.DeviceContext, mouseX, mouseY)) 
                return false;
            if (!TextureShader.Render(D3D.DeviceContext, BitMap.IndexCount, worldMatrix, viewMatrix, orthoMatrix, BitMap.Texture.TextureResource)) 
                return false;

            // Render the text strings.
            if (!Text.Render(D3D.DeviceContext, worldMatrix, orthoMatrix))
                return false;

            // Turn of alpha blending.
            D3D.TurnOffAlphaBlending();

            // Turn the Z buffer back on now that all 2D rendering has completed.
            D3D.TurnZBufferOn();

            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }
    }
}