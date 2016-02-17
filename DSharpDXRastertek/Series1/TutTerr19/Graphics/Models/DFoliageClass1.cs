using DSharpDXRastertek.TutTerr19.System;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;
using System.Runtime.InteropServices;

namespace DSharpDXRastertek.TutTerr19.Graphics.Models
{
    public class DFoliage                   // 280 lines
    {
        // Structs
        [StructLayout(LayoutKind.Sequential)]
        internal struct DVertexType
        {
            internal Vector3 position;
            internal Vector2 texture;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct DFoliageType
        {
            internal float x, z;
            internal float r, g, b;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct DInstanceType
        {
            public Matrix matrix;
            public Vector3 color;
        }

        // Properties
        public int FoliageCount { get; set; }
        public int VertexCount { get; set; }
        public int InstanceCount { get; set; }
        internal DFoliageType[] FoliageArray { get; set; }
        public DInstanceType[] Instances { get; set; }
        public SharpDX.Direct3D11.Buffer VertexBuffer { get; set; }
        public SharpDX.Direct3D11.Buffer InstanceBuffer { get; set; }
        public DTexture Texture { get; set; }
        public float WindRotation { get; set; }
        public int WindDirection { get; set; }

        // Methods
        public bool Initialize(Device device, string textureFileName, int foliageCount)
        {
            // Set the foliage count.
            FoliageCount = foliageCount;

            // Generate the positions of the foliage.
            if (!GeneratePositions())
                return false;

            // Initialize the vertex and instance buffer that hold the geometry for the foliage model.
            if (!InitializeBuffers(device))
                return false;

            // Load the texture for this model.
            if (!LoadTexture(device, textureFileName))
                return false;

            // Set the initial wind rotation and direction.
            WindRotation = 0.9f;
            WindDirection = 1;

            return true;
        }
        private bool LoadTexture(Device device, string textureFileName)
        {
            // Create the cloud texture object.
            Texture = new DTexture();

            // Initialize the cloud texture object.
            if (!Texture.Initialize(device, DSystemConfiguration.DataFilePath + textureFileName))
                return false;

            return true;
        }
        private bool InitializeBuffers(Device device)
        {
            // Set the number of vertices in the vertex array.
            VertexCount = 6;

            // Create the vertex array.
            DVertexType[] vertices = new DVertexType[VertexCount];

            // Load the vertex array with data.
            vertices[0].position = new Vector3(0.0f, 0.0f, 0.0f);  // Bottom left.
            vertices[0].texture = new Vector2(0.0f, 1.0f);
            vertices[1].position = new Vector3(0.0f, 1.0f, 0.0f);  // Top left.
            vertices[1].texture = new Vector2(0.0f, 0.0f);
            vertices[2].position = new Vector3(1.0f, 0.0f, 0.0f);  // Bottom right.
            vertices[2].texture = new Vector2(1.0f, 1.0f);
            vertices[3].position = new Vector3(1.0f, 0.0f, 0.0f);  // Bottom right.
            vertices[3].texture = new Vector2(1.0f, 1.0f);
            vertices[4].position = new Vector3(0.0f, 1.0f, 0.0f);  // Top left.
            vertices[4].texture = new Vector2(0.0f, 0.0f);
            vertices[5].position = new Vector3(1.0f, 1.0f, 0.0f);  // Top right.
            vertices[5].texture = new Vector2(1.0f, 0.0f);

            // Set up the description of the vertex buffer.
            // Now finally create the vertex buffer.
            VertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);

            // Release the array now that the vertex buffer has been created and loaded.
            vertices = null;

            // Set the number of instances in the array.
            InstanceCount = FoliageCount;

            // Create the instance array.
            Instances = new DInstanceType[InstanceCount];

            // Setup an initial matrix.
            Matrix matrix = Matrix.Identity;

            // Load the instance array with data.
            for (int i = 0; i < InstanceCount; i++)
            {
                Instances[i].matrix = matrix;
                Instances[i].color = new Vector3(FoliageArray[i].r, FoliageArray[i].g, FoliageArray[i].b);
            }

            // Set up the description of the Dynamic instance buffer.
            BufferDescription instanceBufferDesc = new BufferDescription()
            {
                Usage = ResourceUsage.Dynamic,
                SizeInBytes = Utilities.SizeOf<DInstanceType>() * InstanceCount,
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0
            };

            // Create the instance buffer.
            InstanceBuffer = SharpDX.Direct3D11.Buffer.Create(device, Instances, instanceBufferDesc);  /// C+ Way

            return true;
        }
        private bool GeneratePositions()
        {
            // Set macimum possible random value here.
            int RAND_MAX = 32767;

            // Create an array to store all the foliage information.
            FoliageArray = new DFoliageType[FoliageCount];

            // Seed the random generator.
            Random rnd = new Random((int)DateTime.Now.Ticks);

            // Set random positions and random colors for each piece of foliage.
            for (int i = 0; i < FoliageCount; i++)
            {
                float testX = ((float)rnd.Next(RAND_MAX) / (float)RAND_MAX) * 9.0f - 4.5f;
                float testZ = ((float)rnd.Next(RAND_MAX) / (float)RAND_MAX) * 9.0f - 4.5f;
                FoliageArray[i].x = testX;
                FoliageArray[i].z = testZ;

                float red = ((float)rnd.Next(RAND_MAX) / (float)RAND_MAX) * 1.0f;
                float green = ((float)rnd.Next(RAND_MAX) / (float)RAND_MAX) * 1.0f;
                FoliageArray[i].r = red + 1.0f;
                FoliageArray[i].g = green + 0.5f;
                FoliageArray[i].b = 0.0f;
            }

            return true;
        }
        public void ShutDown()
        {
            // Release the model texture.
            ReleaseTexture();

            // Release the vertex and instance buffers.
            ShutdownBuffers();

            // Release the foliage array.
            if (FoliageArray != null)
                FoliageArray = null;
        }
        private void ShutdownBuffers()
        {
            // Release the instance buffer.
                InstanceBuffer?.Dispose();
                InstanceBuffer = null;
            // Release the vertex buffer.
                VertexBuffer?.Dispose();
                VertexBuffer = null;

            // Release the instance array.
                Instances = null;
        }
        private void ReleaseTexture()
        {
            // Release the texture objects.
            Texture?.ShutDown();
            Texture = null;
        }
        public void Render(DeviceContext deviceContext)
        {
            // Put the vertex and instance buffers on the graphics pipeline to prepare them for drawing.
            RenderBuffers(deviceContext);
        }
        private void RenderBuffers(DeviceContext deviceContext)
        {
            // Set the vertex and instance buffers to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<DVertexType>(), 0), new VertexBufferBinding(InstanceBuffer, Utilities.SizeOf<DInstanceType>(), 0)); // * InstanceCount

            // Set the type of primitive that should be rendered from this vertex buffer, in this case triangles.
            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }
        public bool Frame(Vector3 cameraPosition, DeviceContext deviceContext)
        {
            // Update the wind rotation.
            if (WindDirection == 1)
            {
                WindRotation += 0.1f;
                if (WindRotation > 10.0f)
                {
                    WindDirection = 2;
                }
            }
            else
            {
                WindRotation -= 0.1f;
                if (WindRotation < -10.0f)
                {
                    WindDirection = 1;
                }
            }

            double angle;
            float rotation;
            Vector3 modelPosition;
            Matrix rotateMatrix, rotateMatrix2, translationMatrix, finalMatrix;
            float windRotation;
            double D3DX_PI = 3.14159265358979323846;

            // Load the instance buffer with the updated locations.
            for (int i = 0; i < FoliageCount; i++)
            {
                // Get the position of this piece of foliage.
                modelPosition = new Vector3();
                modelPosition.X = FoliageArray[i].x;
                modelPosition.Y = -0.1f;  // Place foliage on the ground.
                modelPosition.Z = FoliageArray[i].z;

                // Calculate the rotation that needs to be applied to the billboard model to face the current camera position using the arc tangent function.
                angle = Math.Atan2(modelPosition.X - cameraPosition.X, modelPosition.Z - cameraPosition.Z) * (180.0 / D3DX_PI);

                // Convert rotation into radians.
                rotation = (float)angle * 0.0174532925f;
                // Setup the X rotation of the billboard.
                rotateMatrix = Matrix.RotationY(rotation);
                // Get the wind rotation for the foliage.
                windRotation = WindRotation * 0.0174532925f;
                // Setup the wind rotation.
                rotateMatrix2 = Matrix.RotationX(windRotation);
                // Setup the translation matrix.
                translationMatrix = Matrix.Translation(modelPosition.X, modelPosition.Y, modelPosition.Z);

                // Create the final matrix and store it in the instances array.
                finalMatrix = Matrix.Multiply(rotateMatrix, rotateMatrix2);
                Instances[i].matrix = Matrix.Multiply(finalMatrix, translationMatrix);
            }

            // Lock the instance buffer so it can be written to.
            DataStream mappedResource;
            deviceContext.MapSubresource(InstanceBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out mappedResource);

            // Copy the instances array into the instance buffer.
            mappedResource.WriteRange(Instances);

            // Unlock the instance buffer.
            deviceContext.UnmapSubresource(InstanceBuffer, 0);

            return true;
        }
    }
}