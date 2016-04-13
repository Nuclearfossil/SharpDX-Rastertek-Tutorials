using DSharpDXRastertek.Series2.TutTerr01.Graphics.Camera;
using DSharpDXRastertek.Series2.TutTerr01.Graphics.Data;
using DSharpDXRastertek.Series2.TutTerr01.Graphics.Input;
using DSharpDXRastertek.Series2.TutTerr01.Graphics.Models;
using DSharpDXRastertek.Series2.TutTerr01.Graphics.Shaders;
using DSharpDXRastertek.Series2.TutTerr01.System;
using SharpDX;
using System;

namespace DSharpDXRastertek.Series2.TutTerr01.Graphics
{
    public class DZone
    {
        public DUserInterface UserInterface { get; set; }
        public DCamera Camera { get; set; }
        public DPosition Position { get; set; }
        public DTerrain Terrain { get; set; }
        public bool DisplayUI { get; set; }

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
            // Set the initial position and rotation of the viewer.
            Position.SetPosition(128.0f, 5.0f, -10.0f);
            Position.SetRotation(0.0f, 0.0f, 0.0f);

            // Initialize the terrain object.
            Terrain = new DTerrain();
            // Initialize the ground model object.
            if (!Terrain.Initialize(D3D.Device))
                return false;

            // Set the UI to display by default.
            DisplayUI = true;

            return true;
        }
        public void ShutDown()
        {
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

            // Set the position and rOTATION of the camera.
            Camera.SetPosition(Position.PositionX, Position.PositionY, Position.PositionZ);
            Camera.SetRotation(Position.RotationX, Position.RotationY, Position.RotationZ);

            return true;
        }
        public bool Frame(DDX11 direct3D, DInput input, DShaderManager shaderManager, float frameTime, int fps)
        {
            // Do the frame input processing.
            if (!HandleInput(input, frameTime))
                return false;

            // Do the frame processing for the user interface.
            if (!UserInterface.Frame(direct3D.DeviceContext, fps, Position.PositionX, Position.PositionY, Position.PositionZ, Position.RotationX, Position.RotationY, Position.RotationZ))
                return false;

            // Render the graphics.
            if (!Render(direct3D, shaderManager))
                return false;

            return true;
        }
        public bool Render(DDX11 direct3D, DShaderManager shaderManager)
        {
            // Generate the view matrix based on the camera's position.
            Camera.Render();

            // Get the world, view, and projection matrices from the camera and d3d objects.
            Matrix worldMatrix = direct3D.WorldMatrix;
            Matrix viewCameraMatrix = Camera.ViewMatrix;
            Matrix projectionMatrix = direct3D.ProjectionMatrix;
            Matrix baseViewMatrix = Camera.BaseViewMatrix;
            Matrix orthoMatrix = direct3D.OrthoMatrix;

            // Clear the buffers to begin the scene.
            direct3D.BeginScene(0.0f, 0.0f, 0.0f, 1.0f);

            // Render the terrain grid using the color shader.
            Terrain.Render(direct3D.DeviceContext);
            if (!shaderManager.RenderColorShader(direct3D.DeviceContext, Terrain.IndexCount, worldMatrix, viewCameraMatrix, projectionMatrix))
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
