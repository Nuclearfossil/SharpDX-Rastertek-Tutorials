using DSharpDXRastertek.Series2.TutTerr11.Graphics.Camera;
using DSharpDXRastertek.Series2.TutTerr11.Graphics.Data;
using DSharpDXRastertek.Series2.TutTerr11.Graphics.Input;
using DSharpDXRastertek.Series2.TutTerr11.Graphics.Models;
using DSharpDXRastertek.Series2.TutTerr11.Graphics.Shaders;
using DSharpDXRastertek.Series2.TutTerr11.System;
using SharpDX;
using System;

namespace DSharpDXRastertek.Series2.TutTerr11.Graphics
{
    public class DZone
    {
        public DUserInterface UserInterface { get; set; }
        public DCamera Camera { get; set; }
        private DLight Light { get; set; }
        public DPosition Position { get; set; }
        public DTerrain Terrain { get; set; }
        public DSkyDome SkyDomeModel { get; set; }
        public DFrustum Frustum { get; set; }
        public bool DisplayUI { get; set; }
        public bool WireFrame { get; set; }
        public bool CellLines { get; set; }
        public bool HeightLocked { get; set; }

        public DZone() { }

        public bool Initialze(DDX11 D3D, IntPtr windowHandle, DSystemConfiguration configuration)
        {
            // Create the user interface object.
            UserInterface = new DUserInterface();
            // Initialize the user interface object.
            if (!UserInterface.Initialize(D3D, configuration))
                return false;

            // Create the camera object
            Camera = new DCamera();
            // Initialize a base view matrix with the camera for 2D user interface rendering.
            Camera.SetPosition(0.0f, 0.0f, -10.0f);
            Camera.Render();
            Camera.RenderBaseViewMatrix();

            // Create the position object.  
            Position = new DPosition();
            // Set the initial position and rotation of the viewer.28.0f, 5.0f, -10.0f
            Position.SetPosition(512.0f, 100.0f, 1084.0f);
            Position.SetRotation(0.0f, 180.0f, 0.0f);

            // Create the light object.
            Light = new DLight();

            // Initialize the light object.
            Light.SetDiffuseColor(1.0f, 1.0f, 1.0f, 1.0f);
            Light.Direction = new Vector3(-0.5f, -1.0f, -0.5f);

            // Create and initialize the frustum object.
            Frustum = new DFrustum();
            Frustum.Initialize(DSystemConfiguration.ScreenDepth);

            // Create the sky dome object.
            SkyDomeModel = new DSkyDome();

            // Initialize the sky dome object.
            if (!SkyDomeModel.Initialize(D3D.Device, "skydome.txt"))
                return false;

            // Initialize the terrain object.
            Terrain = new DTerrain();
            // Initialize the ground model object.
            if (!Terrain.Initialize(D3D.Device, "setupS2TutTerr08.txt"))
                return false;

            // Set the UI to display by default.
            DisplayUI = true;
            // Set wire frame rendering initially to enabled.
            WireFrame = false;
            // Set the rendering of cell lines initially to enabled.
            CellLines = true;
            // Set the user locked to the terrain height for movement.
            HeightLocked = true;

            return true;
        }
        public void ShutDown()
        {
            // Release the light object.
            Light = null;
            // Release the sky dome object.
            SkyDomeModel?.ShutDown();
            SkyDomeModel = null;
            // Release the frustum object.
            Frustum._Planes = null;
            Frustum  = null;
            // Release the terrain object.
            Terrain?.ShutDown();
            Terrain = null;
            // Release the position object.
            Position = null;
            // Release the camera object.
            Camera = null;
            // Release the user interface object.
            UserInterface?.ShutDown();
            UserInterface = null;
        }
        private bool HandleInput(DInput input, float frameTime)
        {
            // Set the frame time for calculating the updated position.
            Position.SetFrameTime(frameTime);

            // Handle the input
            bool keydown = input.IsLeftArrowPressed();
            Position.TurnLeft(keydown);
            keydown = input.IsRightArrowPressed();
            Position.TurnRight(keydown);
            keydown = input.IsUpArrowPressed();
            Position.MoveForward(keydown);
            keydown = input.IsDownArrowPressed();
            Position.MoveBackward(keydown);
            keydown = input.IsPageUpPressed();
            Position.LookUpward(keydown);
            keydown = input.IsPageDownPressed();
            Position.LookDownward(keydown);
            keydown = input.IsAPressed();
            Position.MoveUpward(keydown);
            keydown = input.IsZPressed();
            Position.MoveDownward(keydown);

            // Determine if the user interface should be displayed or not.
            if (input.IsF1Toogled())
                DisplayUI = !DisplayUI;
            // Determine if the terrain should be rendered in wireframe or not.
            if (input.IsF2Toogled())
                WireFrame = !WireFrame;
            // Determine if we should render the lines around each terrain cell.
            if (input.isF3Toggled())
                CellLines = !CellLines;
            // Determine if we should be locked to the terrain height when we move around or not.
            if (input.isF4Toggled())
                HeightLocked = !HeightLocked;

            // Set the position and rOTATION of the camera.
            Camera.SetPosition(Position.PositionX, Position.PositionY, Position.PositionZ);
            Camera.SetRotation(Position.RotationX, Position.RotationY, Position.RotationZ);

            return true;
        }
        public bool Frame(DDX11 direct3D, DInput input, DShaderManager shaderManager, DTextureManager textureManager, float frameTime, int fps)
        {
            // Do the frame input processing.
            if (!HandleInput(input, frameTime))
                return false;

            // Do the frame processing for the user interface.
            if (!UserInterface.Frame(direct3D.DeviceContext, fps, Position.PositionX, Position.PositionY, Position.PositionZ, Position.RotationX, Position.RotationY, Position.RotationZ))
                return false;

            // Do the terrain frame processing.
            Terrain.Frame();

            float height = 99.0f;
            // If the height is locked to the terrain then position the camera on top of it.
            if (HeightLocked)
                Terrain.GetHeightAtPosition(Position.PositionX, Position.PositionZ, out height);
               
            Position.SetPosition(Position.PositionX, height + 1.0f, Position.PositionZ);
            Camera.SetPosition(Position.PositionX, height + 1.0f, Position.PositionZ);


            // Render the graphics.
            if (!Render(direct3D, shaderManager, textureManager))
                return false;

            return true;
        }
        public bool Render(DDX11 direct3D, DShaderManager shaderManager, DTextureManager textureManager)
        {
            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world, view, and projection matrices from the camera and d3d objects.
            Matrix worldMatrix = direct3D.WorldMatrix;
            Matrix viewCameraMatrix = Camera.ViewMatrix;
            Matrix projectionMatrix = direct3D.ProjectionMatrix;
            Matrix baseViewMatrix = Camera.BaseViewMatrix;
            Matrix orthoMatrix = direct3D.OrthoMatrix;

            // Construct the frustum.
            Frustum.ConstructFrustum(projectionMatrix, viewCameraMatrix);

            // Clear the buffers to begin the scene.
            direct3D.BeginScene(0.0f, 0.0f, 0.0f, 1.0f);

            // Turn off back face culling and turn off the Z buffer.
            direct3D.TurnOffCulling();
            direct3D.TurnZBufferOff();

            // Translate the sky dome to be centered around the camera position.
            Matrix.Translation(Camera.GetPosition().X, Camera.GetPosition().Y, Camera.GetPosition().Z, out worldMatrix);

            // Render the sky dome using the sky dome shader.
            SkyDomeModel.Render(direct3D.DeviceContext);
            if (!shaderManager.RenderSkyDomeShader(direct3D.DeviceContext, SkyDomeModel.IndexCount, worldMatrix, viewCameraMatrix, projectionMatrix, SkyDomeModel.m_apexColor, SkyDomeModel.m_centerColor))
                return false;

            // Reset the world matrix.
            worldMatrix = direct3D.WorldMatrix;

            // Turn the Z buffer back and back face culling on.
            direct3D.TurnZBufferOn();
            direct3D.TurnOnCulling();

            // Turn on wire frame rendering of the terrain if needed.
            if (WireFrame)
                direct3D.EnableWireFrame();

            // Render the terrain cells (and cell lines if needed).
            for (int i = 0; i < Terrain.m_CellCount; i++)
            {
                // Render each terrain cell if it is visible only.
                if (Terrain.RenderCell(direct3D.DeviceContext, i, Frustum))
                {
                    // Render the cell buffers using the terrain shader.
                    if (!shaderManager.RenderTerrainShader(direct3D.DeviceContext, Terrain.GetCellIndexCount(i), worldMatrix, viewCameraMatrix, projectionMatrix, textureManager.TextureArray[0].TextureResource, textureManager.TextureArray[1].TextureResource, Light.Direction, Light.DiffuseColour))
                        return false;

                    // If needed then render the bounding box around this terrain cell using the color shader. 
                    if (CellLines)
                    {
                        Terrain.RenderCellLines(direct3D.DeviceContext, i);
                        if (!shaderManager.RenderColorShader(direct3D.DeviceContext, Terrain.GetCellLinesIndexCount(i), worldMatrix, viewCameraMatrix, projectionMatrix))
                            return false;
                    }
                }
            }

            // Turn off wire frame rendering of the terrain if it was on.
            if (WireFrame)
                direct3D.DisableWireFrame();

            // Update the render counts in the UI.
            if (!UserInterface.UpdateRenderCountStrings(Terrain.m_renderCount, Terrain.m_cellsDrawn, Terrain.m_cellsCulled, direct3D.DeviceContext))
                return false;

            // Render the user interface.
            if (DisplayUI)
                if (!UserInterface.Render(direct3D, shaderManager, worldMatrix, baseViewMatrix, orthoMatrix))
                    return false;

            // Present the rendered scene to the screen.
            direct3D.EndScene();

            return true;
        }
    }
}