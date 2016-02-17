using DSharpDXRastertek.Tut46.Graphics.Camera;
using DSharpDXRastertek.Tut46.Graphics.Data;
using DSharpDXRastertek.Tut46.Graphics.Models;
using DSharpDXRastertek.Tut46.Graphics.Shaders;
using DSharpDXRastertek.Tut46.System;
using SharpDX;
using System;
using System.Windows.Forms;

namespace DSharpDXRastertek.Tut46.Graphics
{
    public class DGraphics                  // 533 lines
    {
        // Properties
        private DDX11 D3D { get; set; }
        public DCamera Camera { get; set; }
        
        #region Data
        public DRenderTexture RenderTexture { get; set; }
        public DRenderTexture DownSampleTexture { get; set; }
        public DRenderTexture HorizontalBlurTexture { get; set; }
        public DRenderTexture VerticalBlurTexture { get; set; }
        public DRenderTexture UpSampleTexure { get; set; }
        #endregion

        #region Models
        private DBitmap BitMap { get; set; }
        public DOrthoWindow SmallWindow { get; set; }
        public DOrthoWindow FullScreenWindow { get; set; }
        #endregion

        #region Shaders
        private DTextureShader TextureShader { get; set; }
        public DVerticalBlurShader VerticalBlurShader { get; set; }
        public DHorizontalBlurShader HorizontalBlurShader { get; set; }
        public DGlowMapShader GlowMapShader { get; set; }
        public DGlowShader GlowShader { get; set; }
        #endregion     

        // Construtor
        public DGraphics() { }

        // Methods.
        public bool Initialize(DSystemConfiguration configuration, IntPtr windowHandle)
        {
            // Set the size to sample down to.
            int downSampleWidth = configuration.Width / 2;
            int downSampleHeight = configuration.Height / 2;

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
                Camera.SetPosition(0.0f, 0.0f, -10.0f);
                #endregion

                // Create the texture shader object.
                TextureShader = new DTextureShader();

                // Create the texture shader object.
                if (!TextureShader.Initialize(D3D.Device, windowHandle))
                    return false;

                // Create the bitmap object.
                BitMap = new DBitmap();

                // Initialize the bitmap object.
                if (!BitMap.Initialize(D3D.Device, configuration.Width, configuration.Height, "test.dds", "glowmap.dds", 256, 32))
                    return false;

                // Create the render to texture object.
                RenderTexture = new DRenderTexture();

                // Initialize the render to texture object.
                if (!RenderTexture.Initialize(D3D.Device, configuration.Width, configuration.Height, DSystemConfiguration.ScreenNear, DSystemConfiguration.ScreenDepth))
                    return false;

                // Create the down sample render to texture object.
                DownSampleTexture = new DRenderTexture();

                // Initialize the down sample render to texture object.
                if (!DownSampleTexture.Initialize(D3D.Device, configuration.Width / 2, configuration.Height / 2, DSystemConfiguration.ScreenNear, DSystemConfiguration.ScreenDepth))
                    return false;

                // Create the small ortho window object.
                SmallWindow = new DOrthoWindow();

                // Initialize the small ortho window object.
                if (!SmallWindow.Initialize(D3D.Device, configuration.Width / 2, configuration.Height / 2))
                    return false;

                // Create the horizontal blur render to texture object.
                HorizontalBlurTexture = new DRenderTexture();

                // Initialize the horizontal blur render to texture object.
                if (!HorizontalBlurTexture.Initialize(D3D.Device, configuration.Width / 2, configuration.Height / 2, DSystemConfiguration.ScreenNear, DSystemConfiguration.ScreenDepth))
                    return false;

                // Create the horizontal blur shader object.
                HorizontalBlurShader = new DHorizontalBlurShader();

                // Initialize the horizontal blur shader object.
                if (!HorizontalBlurShader.Initialize(D3D.Device, windowHandle))
                    return false;

                // Create the vertical blur render to texture object.
                VerticalBlurTexture = new DRenderTexture();

                // Initialize the vertical blur render to texture object.
                if (!VerticalBlurTexture.Initialize(D3D.Device, configuration.Width / 2, configuration.Height / 2, DSystemConfiguration.ScreenNear, DSystemConfiguration.ScreenDepth))
                    return false;

                // Create the vertical blur shader object.
                VerticalBlurShader = new DVerticalBlurShader();

                // Initialize the vertical blur shader object.
                if (!VerticalBlurShader.Initialize(D3D.Device, windowHandle))
                    return false;

                // Create the up sample render to texture object.
                UpSampleTexure = new DRenderTexture();

                // Initialize the up sample render to texture object.
                if (!UpSampleTexure.Initialize(D3D.Device, configuration.Width, configuration.Height, DSystemConfiguration.ScreenNear, DSystemConfiguration.ScreenDepth))
                    return false;

                // Create the full screen ortho window object.
                FullScreenWindow = new DOrthoWindow();

                // Initialize the full screen ortho window object.
                if (!FullScreenWindow.Initialize(D3D.Device, configuration.Width, configuration.Height))
                    return false;

                // Create the glow map shader object.
                GlowMapShader = new DGlowMapShader();

                // Initialize the glow map shader object.
                if (!GlowMapShader.Initialize(D3D.Device, windowHandle))
                    return false;

                // Create the glow shader object.
                GlowShader = new DGlowShader();

                // Initialize the glow shader object.
                if (!GlowShader.Initialize(D3D.Device, windowHandle))
                    return false;

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

            // Release the glow shader object.
            GlowShader?.ShutDown();
            GlowShader = null;
            // Release the glow map shader object.
            GlowMapShader?.ShutDown();
            GlowMapShader = null;
            // Release the full screen ortho window object.
            FullScreenWindow?.Shutdown();
            FullScreenWindow = null;
            // Release the small ortho window object.
            SmallWindow?.Shutdown();
            SmallWindow = null;
            // Release the up sample render to texture object.
            UpSampleTexure?.Shutdown();
            UpSampleTexure = null;
            // Release the vertical blur render to texture object.
            VerticalBlurTexture?.Shutdown();
            VerticalBlurTexture = null;
            // Release the vertical blur shader object.
            VerticalBlurShader?.ShutDown();
            VerticalBlurShader = null;
            // Release the horizontal blur render to texture object.
            HorizontalBlurTexture?.Shutdown();
            HorizontalBlurTexture = null;
            // Release the horizontal blur shader object.
            HorizontalBlurShader?.ShutDown();
            HorizontalBlurShader = null;
            // Release the small ortho window object.
            SmallWindow?.Shutdown();
            SmallWindow = null;
            // Release the down sample render to texture object.
            DownSampleTexture?.Shutdown();
            DownSampleTexture = null;
            // Release the render to texture object.
            RenderTexture?.Shutdown();
            RenderTexture = null;
            // Release the model object.
            BitMap?.Shutdown();
            BitMap = null;
            // Release the texture shader object.
            TextureShader?.ShutDown();
            TextureShader = null;
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
        private bool Render()
        {
            ///The Blur Algorithm
            // 1: First render the glow maps to a render texture.
            if (!RenderGlowMapToTexture())
                return false;

            // 2: Next down sample the render texture to a smaller sized texture.
            if (!DownSampleTextureMethod())
                return false;

            // 3: Perform a horizontal blur on the down sampled render texture.
            if (!RenderHorizontalBlurToTexture())
                return false;

            // 4: Now perform a vertical blur on the horizontal blur render texture.
            if (!RenderVerticalBlurToTexture())
                return false;

            // 5: Up sample the final blurred render texture to screen size again.
            if (!UpSampleTexture())
                return false;

            // 6: Render the regular UI elements to a full screen texture.
            if (!RenderUIElementsToTexture())
                return false;

            // Render the final scene combining the UI elements with the glowing UI elements.
            if (!RenderGlowScene())
                return false;

            return true;
        }
        private bool RenderGlowMapToTexture()
        {
            // Set the render target to be the render to texture.
            RenderTexture.SetRenderTarget(D3D.DeviceContext);

            // Clear the render to texture.
            RenderTexture.ClearRenderTarget(D3D.DeviceContext, 0.0f, 0.0f, 0.0f, 1.0f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world, view, and projection matrices from the camera and d3d objects.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix viewMatrix = Camera.ViewMatrix;
            Matrix orthoMatrix = D3D.OrthoMatrix;

            // Turn off the Z buffer to begin all 2D rendering.
            D3D.TurnZBufferOff();

            // Put the bitmap vertex and index buffers on the graphics pipeline to prepare them for drawing.
            if (!BitMap.Render(D3D.DeviceContext, 100, 100))
                return false;

            // Render the bitmap using the glow map shader.
            if (!GlowMapShader.Render(D3D.DeviceContext, BitMap.IndexCount, worldMatrix, viewMatrix, orthoMatrix, BitMap.Texture.TextureResource, BitMap.GlowMapTexture.TextureResource))
                return false;
            
            // Turn the Z buffer back on now that all 2D rendering has completed.
            D3D.TurnZBufferOn();

            // Reset the render target back to the original back buffer and not the render to texture anymore.
            D3D.SetBackBufferRenderTarget();

            // Reset the viewport back to the original.
            D3D.ResetViewPort();

            return true;
        }        
        private bool DownSampleTextureMethod()
        {
            // Set the render target to be the render to texture.
            DownSampleTexture.SetRenderTarget(D3D.DeviceContext);

            // Clear the render to texture.
            DownSampleTexture.ClearRenderTarget(D3D.DeviceContext, 0.0f, 1.0f, 0.0f, 1.0f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world and view matrices from the camera and d3d objects.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix viewMatrix = Camera.ViewMatrix;

            // Get the ortho matrix from the render to texture since texture has different dimensions being that it is smaller.
            Matrix orthoMatrix = DownSampleTexture.OrthoMatrix;

            // Turn off the Z buffer to begin all 2D rendering.
            D3D.TurnZBufferOff();

            // Put the small ortho window vertex and index buffers on the graphics pipeline to prepare them for drawing.
            if (!SmallWindow.Render(D3D.DeviceContext))
                return false;

            // Render the small ortho window using the texture shader and the render to texture of the scene as the texture resource.
            if (!TextureShader.Render(D3D.DeviceContext, SmallWindow.IndexCount, worldMatrix, viewMatrix, orthoMatrix, RenderTexture.ShaderResourceView))
                return false;

            // Turn the Z buffer back on now that all 2D rendering has completed.
            D3D.TurnZBufferOn();

            // Reset the render target back to the original back buffer and not the render to texture anymore.
            D3D.SetBackBufferRenderTarget();

            // Reset the viewport back to the original.
            D3D.ResetViewPort();

            return true;
        }
        private bool RenderHorizontalBlurToTexture()
        {
            /// Set the render target to be the render to texture.
            HorizontalBlurTexture.SetRenderTarget(D3D.DeviceContext);
               
            // Clear the render to texture.
            HorizontalBlurTexture.ClearRenderTarget(D3D.DeviceContext, 0.0f, 0.0f, 0.0f, 1.0f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world and view matrices from the camera and d3d objects.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix viewMatrix = Camera.ViewMatrix;

            // Get the ortho matrix from the render to texture since texture has different dimensions.
            Matrix orthoMatrix = HorizontalBlurTexture.OrthoMatrix;

            // Turn off the Z buffer to begin all 2D rendering.
            D3D.TurnZBufferOff();

            // Store the screen width in a float that will be used in the horizontal blur shader.
            float screenSizeX = (float)HorizontalBlurTexture.TextureWidth;

            // Put the small ortho window vertex and index buffers on the graphics pipeline to prepare them for drawing.
            SmallWindow.Render(D3D.DeviceContext);

            // Render the small ortho window using the horizontal blur shader and the down sampled render to texture resource.
            if (!HorizontalBlurShader.Render(D3D.DeviceContext, SmallWindow.IndexCount, worldMatrix, viewMatrix, orthoMatrix, DownSampleTexture.ShaderResourceView, screenSizeX))
                return false;

            // Turn the Z buffer back on now that all 2D rendering has completed.
            D3D.TurnZBufferOn();

            // Reset the render target back to the original back buffer and not the render to texture anymore.
            D3D.SetBackBufferRenderTarget();

            // Reset the viewport back to the original.
            D3D.ResetViewPort();

            return true;
        }
        private bool RenderVerticalBlurToTexture()
        {
            // Store the screen height in a float that will be used in the vertical blur shader.
            float screenSizeY = (float)VerticalBlurTexture.TextureHeight;

            // Set the render target to be the render to texture.
            VerticalBlurTexture.SetRenderTarget(D3D.DeviceContext);

            // Clear the render to texture.
            VerticalBlurTexture.ClearRenderTarget(D3D.DeviceContext,  0.0f, 0.0f, 0.0f, 1.0f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world and view matrices from the camera and d3d objects.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix viewMatrix = Camera.ViewMatrix;

            // Get the ortho matrix from the render to texture since texture has different dimensions.
            Matrix orthoMatrix = VerticalBlurTexture.OrthoMatrix;

            // Turn off the Z buffer to begin all 2D rendering.
            D3D.TurnZBufferOff();

            // Put the small ortho window vertex and index buffers on the graphics pipeline to prepare them for drawing.
            SmallWindow.Render(D3D.DeviceContext);

            // Render the small ortho window using the vertical blur shader and the horizontal blurred render to texture resource.
            if (!VerticalBlurShader.Render(D3D.DeviceContext, SmallWindow.IndexCount, worldMatrix, viewMatrix, orthoMatrix, HorizontalBlurTexture.ShaderResourceView, screenSizeY))
                return false;

            // Turn the Z buffer back on now that all 2D rendering has completed.
            D3D.TurnZBufferOn();

            // Reset the render target back to the original back buffer and not the render to texture anymore.
            D3D.SetBackBufferRenderTarget();

            // Reset the viewport back to the original.
            D3D.ResetViewPort();

            return true;
        }
        private bool UpSampleTexture()
        {
            // Set the render target to be the render to texture.
            UpSampleTexure.SetRenderTarget(D3D.DeviceContext);

            // Clear the render to texture.
            UpSampleTexure.ClearRenderTarget(D3D.DeviceContext, 0.0f, 0.0f, 0.0f, 1.0f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world and view matrices from the camera and d3d objects.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix viewMatrix = Camera.ViewMatrix;

            // Get the ortho matrix from the render to texture since texture has different dimensions.
            Matrix orthoMatrix = UpSampleTexure.OrthoMatrix;

            // Turn off the Z buffer to begin all 2D rendering.
            D3D.TurnZBufferOff();

            // Put the full screen ortho window vertex and index buffers on the graphics pipeline to prepare them for drawing.
            FullScreenWindow.Render(D3D.DeviceContext);

            // Render the full screen ortho window using the texture shader and the small sized final blurred render to texture resource.
            if (!TextureShader.Render(D3D.DeviceContext, FullScreenWindow.IndexCount, worldMatrix, viewMatrix, orthoMatrix, VerticalBlurTexture.ShaderResourceView))
                return false;

            // Turn the Z buffer back on now that all 2D rendering has completed.
            D3D.TurnZBufferOn();

            // Reset the render target back to the original back buffer and not the render to texture anymore.
            D3D.SetBackBufferRenderTarget();

            // Reset the viewport back to the original.
            D3D.ResetViewPort();

            return true;
        }
        private bool RenderUIElementsToTexture()
        {
            // Set the render target to be the render to texture.
            RenderTexture.SetRenderTarget(D3D.DeviceContext);

            // Clear the render to texture.
            RenderTexture.ClearRenderTarget(D3D.DeviceContext, 0.0f, 0.0f, 0.0f, 1.0f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world, view, and ortho matrices from the camera and d3d objects.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix viewMatrix = Camera.ViewMatrix;
            Matrix orthoMatrix = D3D.OrthoMatrix;

            // Turn off the Z buffer to begin all 2D rendering.
            D3D.TurnZBufferOff();

            // Put the bitmap vertex and index buffers on the graphics pipeline to prepare them for drawing.
            if (!BitMap.Render(D3D.DeviceContext, 100, 100))
                return false;


            // Render the bitmap using the texture shader.
            if (!TextureShader.Render(D3D.DeviceContext, BitMap.IndexCount, worldMatrix, viewMatrix, orthoMatrix,BitMap.Texture.TextureResource))
                return false;

            // Turn the Z buffer back on now that all 2D rendering has completed.
            D3D.TurnZBufferOn();

            // Reset the render target back to the original back buffer and not the render to texture anymore.
            D3D.SetBackBufferRenderTarget();

            // Reset the viewport back to the original.
            D3D.ResetViewPort();

            return true;
        }
        private bool RenderGlowScene()
        {
            // Clear the buffers to begin the scene.
            D3D.BeginScene(1.0f, 0.0f, 0.0f, 0.0f);

            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world, view, and ortho matrices from the camera and d3d objects.
            Matrix worldMatrix = D3D.WorldMatrix;
            Matrix viewMatrix = Camera.ViewMatrix;
            Matrix orthoMatrix = D3D.OrthoMatrix;

            // Turn off the Z buffer to begin all 2D rendering.
            D3D.TurnZBufferOff();

            // Put the full screen ortho window vertex and index buffers on the graphics pipeline to prepare them for drawing.
            if (!FullScreenWindow.Render(D3D.DeviceContext))
                return false;

            // Render the full screen ortho window using the texture shader and the full screen sized blurred render to texture resource.
            if (!GlowShader.Render(D3D.DeviceContext, FullScreenWindow.IndexCount, worldMatrix, viewMatrix, orthoMatrix, RenderTexture.ShaderResourceView, UpSampleTexure.ShaderResourceView, 3.0f))
                return false;

            // Turn the Z buffer back on now that all 2D rendering has completed.
            D3D.TurnZBufferOn();

            // Present the rendered scene to the screen.
            D3D.EndScene();

            return true;
        }
    }
}