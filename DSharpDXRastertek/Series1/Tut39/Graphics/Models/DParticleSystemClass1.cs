using DSharpDXRastertek.Tut39.System;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Runtime.InteropServices;

namespace DSharpDXRastertek.Tut39.Graphics
{
    public class DParticleSystem                    // 397 lines
    {
        // Structs
        [StructLayout(LayoutKind.Sequential)]
        public struct DParticleType
        {
            public float positionX, positionY, positionZ;
            public float red, green, blue;
            public float velocity;
            public bool active;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct DVertexType
        {
            public Vector3 position;
            public Vector2 texture;
            public Vector4 color;
        }

        // Variables
        private float m_ParticleDeviationX, m_ParticleDeviationY, m_ParticleDeviationZ;
        private float m_ParticleVelocity, m_ParticleVelocityVariation;
        private float m_ParticleSize, m_ParticlesPerSecond;
        private int m_MaxParticles;
        private int m_CurrentParticleCount;
        private float m_AccumulatedTime;

        // Properties
        public SharpDX.Direct3D11.Buffer VertexBuffer { get; set; }
        public SharpDX.Direct3D11.Buffer IndexBuffer { get; set; }
        private int VertexCount { get; set; }
        public int IndexCount { get; private set; }
        public DTexture Texture { get; private set; }
        public DParticleType[] ParticleList { get; set; }
        public DVertexType[] Vertices { get; set; }

        // Constructor
        public DParticleSystem() { }

        // Methods.
        public bool Initialize(SharpDX.Direct3D11.Device device, string textureFileName)
        {
            // Load the texture that is used for the particles.
            if (!LoadTexture(device, textureFileName))
                return false;

            // Initialize the particle system.
            if (!InitializeParticleSystem())
                return false;

            // Create the buffers that will be used to render the particles with.
            if (!InitializeBuffers(device))
                return false;

            return true;
        }

        private bool InitializeParticleSystem()
        {
            // Set the random deviation of where the particles can be located when emitted.
            m_ParticleDeviationX = 0.5f;
            m_ParticleDeviationY = 0.1f;
            m_ParticleDeviationZ = 2.0f;

            // Set the speed and speed variation of particles.
            m_ParticleVelocity = 1.0f;
            m_ParticleVelocityVariation = 0.2f;

            // Set the physical size of the particles.
            m_ParticleSize = 0.2f;
            // Set the number of particles to emit per second.
            m_ParticlesPerSecond = 250.0f;
            // Set the maximum number of particles allowed in the particle system.
            m_MaxParticles = 5000;
            // Create the particle list.
            ParticleList = new DParticleType[m_MaxParticles];

            // Initialize the particle list.
            for(int i = 0; i < m_MaxParticles; i++)
                ParticleList[i].active = false;

            // Initialize the current particle count to zero since none are emitted yet.
            m_CurrentParticleCount = 0;

            // Clear the initial accumulated time for the particle per second emission rate.
            m_AccumulatedTime = 0.0f;

            return true;
        }
        private bool InitializeBuffers(SharpDX.Direct3D11.Device device)
        {
            try
            {
                // Set the maximum number of vertices in the vertex array.
                VertexCount = m_MaxParticles * 6;   
                // Set the maximum number of indices in the index array.
                IndexCount = VertexCount;
              
                // Create the vertex array for the particles that will be rendered.
                Vertices = new DVertexType[VertexCount];       
                // Create the index array.
                int[] indices = new int[IndexCount];

                // Initialize the index array.
                for (int i = 0; i < IndexCount; i++)
                    indices[i] = i;      

                // Set up the description of the dynamic vertex buffer.
                BufferDescription vertexBufferDescription = new BufferDescription() 
                {
                    Usage = ResourceUsage.Dynamic,
                    SizeInBytes = Utilities.SizeOf<DVertexType>() * VertexCount,
                    BindFlags = BindFlags.VertexBuffer,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0
                };

                // Create the Dynamic vertex buffer.
                VertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, Vertices, vertexBufferDescription);
                // VertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, Vertices);

                // Create the static index buffer.
                IndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indices);

                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool LoadTexture(SharpDX.Direct3D11.Device device, string textureFileName)
        {
            textureFileName = DSystemConfiguration.DataFilePath + textureFileName;

            // Create the texture object.
            Texture = new DTexture();

            // Initialize the texture object.
            bool result = Texture.Initialize(device, textureFileName);

            return result;
        }
        public void ShutDown()
        {
            // Release the vertex and index buffers.
            ShutdownBuffers();

            // Release the particle system.
            ShutdownParticleSystem();

            // Release the texture used for the particles.
            ReleaseTexture();
        }

        private void ShutdownParticleSystem()
        {
            // Release the particle list.
            ParticleList = null;
        }
        private void ReleaseTexture()
        {
            // Release the texture object.
            Texture?.ShutDown();
            Texture = null;
        }
        private void ShutdownBuffers()
        {
            // Return the index buffer.
            IndexBuffer?.Dispose();
            IndexBuffer = null;
            // Release the vertex buffer.
            VertexBuffer?.Dispose();
            VertexBuffer = null;
        }
        public bool Frame(float frameTime, DeviceContext deviceContext)
        {
            // Release old particles.
            KillParticles();

            // Emit new particles.
            EmitParticles(frameTime);

            // Update the position of the particles.
            UpdateParticles(frameTime);

            // Update the dynamic vertex buffer with the new position of each particle.
            if (!UpdateBuffers(deviceContext))
                return false;

            return true;
        }
        private bool UpdateBuffers(DeviceContext deviceContext)
        {
            // Initialize vertex array to zeros at first.
            Vertices = new DVertexType[VertexCount];

            // Now build the vertex array from the particle list array.  
            // Each particle is a quad made out of two triangles.
            int index = 0;
            for(int i = 0; i < m_CurrentParticleCount; i++)
            {
                // Bottom left.
                Vertices[index].position = new Vector3(ParticleList[i].positionX - m_ParticleSize, ParticleList[i].positionY - m_ParticleSize, ParticleList[i].positionZ);
                Vertices[index].texture = new Vector2(0.0f, 1.0f);
                Vertices[index].color = new Vector4(ParticleList[i].red, ParticleList[i].green, ParticleList[i].blue, 1.0f);
                index++;

                // Top left.
                Vertices[index].position = new Vector3(ParticleList[i].positionX - m_ParticleSize, ParticleList[i].positionY + m_ParticleSize, ParticleList[i].positionZ);
                Vertices[index].texture = new Vector2(0.0f, 0.0f);
                Vertices[index].color = new Vector4(ParticleList[i].red, ParticleList[i].green, ParticleList[i].blue, 1.0f);
                index++;

                // Bottom right.
                Vertices[index].position = new Vector3(ParticleList[i].positionX + m_ParticleSize, ParticleList[i].positionY - m_ParticleSize, ParticleList[i].positionZ);
                Vertices[index].texture = new Vector2(1.0f, 1.0f);
                Vertices[index].color = new Vector4(ParticleList[i].red, ParticleList[i].green, ParticleList[i].blue, 1.0f);
                index++;

                // Bottom right.
                Vertices[index].position = new Vector3(ParticleList[i].positionX + m_ParticleSize, ParticleList[i].positionY - m_ParticleSize, ParticleList[i].positionZ);
                Vertices[index].texture = new Vector2(1.0f, 1.0f);
                Vertices[index].color = new Vector4(ParticleList[i].red, ParticleList[i].green, ParticleList[i].blue, 1.0f);
                index++;

                // Top left.
		        Vertices[index].position = new Vector3(ParticleList[i].positionX - m_ParticleSize, ParticleList[i].positionY +     m_ParticleSize, ParticleList[i].positionZ);
		        Vertices[index].texture = new Vector2(0.0f, 0.0f);
		        Vertices[index].color = new Vector4(ParticleList[i].red, ParticleList[i].green, ParticleList[i].blue, 1.0f);
		        index++;

		        // Top right.
		        Vertices[index].position = new Vector3(ParticleList[i].positionX + m_ParticleSize, ParticleList[i].positionY +     m_ParticleSize, ParticleList[i].positionZ);
		        Vertices[index].texture = new Vector2(1.0f, 0.0f);
		        Vertices[index].color = new Vector4(ParticleList[i].red, ParticleList[i].green, ParticleList[i].blue, 1.0f);
		        index++;
            }

            
            DataStream mappedResource;
            // mappedResource = VertexBuffer.Map(MapMode.WriteDiscard);
            
            // Lock the vertex buffer.
            deviceContext.MapSubresource(VertexBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out mappedResource);
           
            // Copy the data into the vertex buffer.
            mappedResource.WriteRange<DVertexType>(Vertices);
            // mappedResource.WriteRange<DVertexType>(Vertices);

            // Unlock the vertex buffer.
            deviceContext.UnmapSubresource(VertexBuffer, 0);
            /// VertexBuffer.Unmap();
            return true;
        }
        private void KillParticles()
        {
            // Kill all the particles that have gone below a certain height range.
            for (int i = 0; i < m_MaxParticles; i++)
            {
                if ((ParticleList[i].active == true) && (ParticleList[i].positionY < -3.0f))
                {
                    ParticleList[i].active = false;
                    m_CurrentParticleCount--;

                    // Now shift all the live particles back up the array to erase the destroyed particle and keep the array sorted correctly.
                    for (int j = i; j < m_MaxParticles - 1; j++)
                    {
                        ParticleList[j].positionX = ParticleList[j + 1].positionX;
                        ParticleList[j].positionY = ParticleList[j + 1].positionY;
                        ParticleList[j].positionZ = ParticleList[j + 1].positionZ;
                        ParticleList[j].red = ParticleList[j + 1].red;
                        ParticleList[j].green = ParticleList[j + 1].green;
                        ParticleList[j].blue = ParticleList[j + 1].blue;
                        ParticleList[j].velocity = ParticleList[j + 1].velocity;
                        ParticleList[j].active = ParticleList[j + 1].active;
                    }
                }
            }
        }
        private void UpdateParticles(float frameTime)
        {
            // Each frame we update all the particles by making them move downwards using their position, velocity, and the frame time.
            for(int i = 0; i < m_CurrentParticleCount; i++)
                ParticleList[i].positionY = ParticleList[i].positionY - (ParticleList[i].velocity * frameTime * 0.001f);
        }
        private void EmitParticles(float frameTime)
        {
            int i, j;

            // Increment the frame time.
            m_AccumulatedTime += frameTime;

            // Set emit particle to false for now.
            bool emitParticle = false;

            // Check if it is time to emit a new particle or not.
            if (m_AccumulatedTime > (1000.0f / m_ParticlesPerSecond))
            {
                m_AccumulatedTime = 0.0f;
                emitParticle = true;
            }

            // If there are particles to emit then emit one per frame.
            if ((emitParticle == true) && (m_CurrentParticleCount < (m_MaxParticles - 1)))
            {
                m_CurrentParticleCount++;

                // Now generate the randomized particle properties.
                Random rand = new Random();

                // Now generate the randomized particle properties.
                float positionX = (((float)rand.Next(32767) - (float)rand.Next(32767)) / 32767.0f) * m_ParticleDeviationX;
                float positionY = (((float)rand.Next(32767) - (float)rand.Next(32767)) / 32767.0f) * m_ParticleDeviationY;
                float positionZ = (((float)rand.Next(32767) - (float)rand.Next(32767)) / 32767.0f) * m_ParticleDeviationZ;

                float velocity = m_ParticleVelocity + (((float)rand.Next(32767) - (float)rand.Next(32767)) / 32767.0f) * m_ParticleVelocityVariation;

                float red = (((float)rand.Next(32767) - (float)rand.Next(32767)) / 32767.0f) + 0.5f;
                float green = (((float)rand.Next(32767) - (float)rand.Next(32767)) / 32767.0f) + 0.5f;
                float blue = (((float)rand.Next(32767) - (float)rand.Next(32767)) / 32767.0f) + 0.5f;

                // Now since the particles need to be rendered from back to front for blending we have to sort the particle array.
                // We will sort using Z depth so we need to find where in the list the particle should be inserted.
                int index = 0;
                bool found = false;
                while (!found)
                {
                    if ((ParticleList[index].active == false) || (ParticleList[index].positionZ < positionZ))
                        found = true;
                    else
                        index++;
                }

                // Now that we know the location to insert into we need to copy the array over by one position from the index to make room for the new particle.
                i = m_CurrentParticleCount;
                j = i - 1;

                while (i != index)
                {
                    ParticleList[i].positionX = ParticleList[j].positionX;
                    ParticleList[i].positionY = ParticleList[j].positionY;
                    ParticleList[i].positionZ = ParticleList[j].positionZ;
                    ParticleList[i].red = ParticleList[j].red;
                    ParticleList[i].green = ParticleList[j].green;
                    ParticleList[i].blue = ParticleList[j].blue;
                    ParticleList[i].velocity = ParticleList[j].velocity;
                    ParticleList[i].active = ParticleList[j].active;
                    i--;
                    j--;
                }

                // Now insert it into the particle array in the correct depth order.
                ParticleList[index].positionX = positionX;
                ParticleList[index].positionY = positionY;
                ParticleList[index].positionZ = positionZ;
                ParticleList[index].red = red;
                ParticleList[index].green = green;
                ParticleList[index].blue = blue;
                ParticleList[index].velocity = velocity;
                ParticleList[index].active = true;
            }
        }
        public void Render(DeviceContext deviceContext)
        {
            // Put the vertex and index buffers on the graphics pipeline to prepare for drawings.
            RenderBuffers(deviceContext);
        }
        private void RenderBuffers(DeviceContext deviceContext)
        {
            SharpDX.Direct3D11.Buffer[] buffers = new SharpDX.Direct3D11.Buffer[1];
            buffers[0] = VertexBuffer;
            int[] sizeStride = new int[1];
            int[] offset = new int[1];
            sizeStride[0] = Utilities.SizeOf<DVertexType>();
            offset[0] = 0;
            // Set the vertex buffer to active in the input assembler so it can be rendered.
            // deviceContext.InputAssembler.SetVertexBuffers(0, buffers, sizeStride, offset);
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<DVertexType>(), 0));
            // Set the index buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);
            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }
    }
}