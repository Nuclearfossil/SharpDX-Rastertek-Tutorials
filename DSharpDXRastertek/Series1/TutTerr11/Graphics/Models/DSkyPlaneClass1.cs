using DSharpDXRastertek.TutTerr11.System;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Runtime.InteropServices;

namespace DSharpDXRastertek.TutTerr11.Graphics.Models
{
    public class DSkyPlane                  // 286 LINES
    {
        // Structs
        [StructLayout(LayoutKind.Sequential)]
        public struct DSkyPlaneType
        {
            public float x, y, z;
            public float tu, tv;
        } 
        [StructLayout(LayoutKind.Sequential)]
        internal struct DVertexType
        {
            internal Vector3 position;
            internal Vector2 texture;
        }

        // Variables
        public float m_Brightness;
        public float[] m_TranslationSpeed;
        public float[] m_TextureTranslation;

        // Properties
        public int VertexCount { get; set; }
        public int IndexCount { get; set; }
        public DSkyPlaneType[] SkyPlane { get; set; }
        public SharpDX.Direct3D11.Buffer VertexBuffer { get; set; }
        public SharpDX.Direct3D11.Buffer IndexBuffer { get; set; }
        public DTexture CloudTexture1 { get; set; }
        public DTexture CloudTexture2 { get; set; }

        // Constructors
        public DSkyPlane() { }

        // Methods
        public bool Initialze(SharpDX.Direct3D11.Device device, string textureFilename1, string textureFilename2)
        {
            // Set the sky plane parameters.
            int skyPlaneResolution = 10;
            float skyPlaneWidth = 10.0f;
            float skyPlaneTop = 0.5f;
            float skyPlaneBottom = 0.0f;
            int textureRepeat = 4;

            // Set the brightness of the clouds.
            m_Brightness = 0.65f;

            // Setup the cloud translation speed increments.
            m_TranslationSpeed = new float[4];
            m_TranslationSpeed[0] = 0.0003f;   // First texture X translation speed.
            m_TranslationSpeed[1] = 0.0f;      // First texture Z translation speed.
            m_TranslationSpeed[2] = 0.00015f;  // Second texture X translation speed.
            m_TranslationSpeed[3] = 0.0f;      // Second texture Z translation speed.

            // Initialize the texture translation values.
            m_TextureTranslation = new float[4];

            // Create the sky plane.
            if (!InitializeSkyPlane(skyPlaneResolution, skyPlaneWidth, skyPlaneTop, skyPlaneBottom, textureRepeat))
                return false;

            // Create the vertex and index buffer for the sky plane.
            if (!InitializeBuffers(device, skyPlaneResolution))
                return false;

            // Load the sky plane textures.
            if (!LoadTextures(device, textureFilename1, textureFilename2))
                return false;

            return true;
        }
        private bool InitializeBuffers(SharpDX.Direct3D11.Device device, int skyPlaneResolution)
        {
            // Calculate the number of vertices in the sky plane mesh.
            VertexCount = (skyPlaneResolution + 1) * (skyPlaneResolution + 1) * 6;
            // Set the index count to the same as the vertex count.
            IndexCount = VertexCount;

            // Create the vertex array.
            DVertexType[] vertices = new DVertexType[VertexCount];
            // Create the index array.
            int[] indices = new int[IndexCount];

            // Initialize the index into the vertex array.
            int index = 0;
            // Load the vertex and index array with the sky plane array data.
            for (int j = 0; j < skyPlaneResolution; j++)
            {
                for (int i = 0; i < skyPlaneResolution; i++)
                {
                    int index1 = j * (skyPlaneResolution + 1) + i;
                    int index2 = j * (skyPlaneResolution + 1) + (i + 1);
                    int index3 = (j + 1) * (skyPlaneResolution + 1) + i;
                    int index4 = (j + 1) * (skyPlaneResolution + 1) + (i + 1);

                    // Triangle 1 - Upper Left
                    vertices[index].position = new Vector3(SkyPlane[index1].x, SkyPlane[index1].y, SkyPlane[index1].z);
                    vertices[index].texture = new Vector2(SkyPlane[index1].tu, SkyPlane[index1].tv);
                    indices[index] = index;
                    index++;

                    // Triangle 1 - Upper Right
                    vertices[index].position = new Vector3(SkyPlane[index2].x, SkyPlane[index2].y, SkyPlane[index2].z);
                    vertices[index].texture = new Vector2(SkyPlane[index2].tu, SkyPlane[index2].tv);
                    indices[index] = index;
                    index++;

                    // Triangle 1 - Bottom Left
                    vertices[index].position = new Vector3(SkyPlane[index3].x, SkyPlane[index3].y, SkyPlane[index3].z);
                    vertices[index].texture = new Vector2(SkyPlane[index3].tu, SkyPlane[index3].tv);
                    indices[index] = index;
                    index++;

                    // Triangle 2 - Bottom Left
                    vertices[index].position = new Vector3(SkyPlane[index3].x, SkyPlane[index3].y, SkyPlane[index3].z);
                    vertices[index].texture = new Vector2(SkyPlane[index3].tu, SkyPlane[index3].tv);
                    indices[index] = index;
                    index++;

                    // Triangle 2 - Upper Right
                    vertices[index].position = new Vector3(SkyPlane[index2].x, SkyPlane[index2].y, SkyPlane[index2].z);
                    vertices[index].texture = new Vector2(SkyPlane[index2].tu, SkyPlane[index2].tv);
                    indices[index] = index;
                    index++;

                    // Triangle 2 - Bottom Right
                    vertices[index].position = new Vector3(SkyPlane[index4].x, SkyPlane[index4].y, SkyPlane[index4].z);
                    vertices[index].texture = new Vector2(SkyPlane[index4].tu, SkyPlane[index4].tv);
                    indices[index] = index;
                    index++;
                }
            }

            // Set up the description of the vertex buffer.
            // Create the vertex buffer.
            VertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);

            // Create the index buffer.
            IndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indices);

            // Release the arrays now that the buffers have been created and loaded.
            vertices = null;
            indices = null;

            return true;
        }
        private bool InitializeSkyPlane(int skyPlaneResolution, float skyPlaneWidth, float skyPlaneTop, float skyPlaneBottom, int textureRepeat)
        {
            // Create the array to hold the sky plane coordinates.
            SkyPlane = new DSkyPlaneType[(skyPlaneResolution + 1) * (skyPlaneResolution + 1)];

            // Determine the size of each quad on the sky plane.
            float quadSize = skyPlaneWidth / (float)skyPlaneResolution;
            // Calculate the radius of the sky plane based on the width.
            float radius = skyPlaneWidth / 2.0f;
            // Calculate the height constant to increment by.
            float constant = (skyPlaneTop - skyPlaneBottom) / (radius * radius);
            // Calculate the texture coordinate increment value.
            float textureDelta = (float)textureRepeat / (float)skyPlaneResolution;

            // Loop through the sky plane and build the coordinates based on the increment values given.
            for (int j = 0; j <= skyPlaneResolution; j++)
            {
                for (int i = 0; i <= skyPlaneResolution; i++)
                {
                    // Calculate the vertex coordinates.
                    float positionX = (-0.5f * skyPlaneWidth) + ((float)i * quadSize);
                    float positionZ = (-0.5f * skyPlaneWidth) + ((float)j * quadSize);
                    float positionY = skyPlaneTop - (constant * ((positionX * positionX) + (positionZ * positionZ)));

                    // Calculate the texture coordinates.
                    float tu = (float)i * textureDelta;
                    float tv = (float)j * textureDelta;

                    // Calculate the index into the sky plane array to add this coordinate.
                    int index = j * (skyPlaneResolution + 1) + i;

                    // Add the coordinates to the sky plane array.
                    SkyPlane[index].x = positionX;
                    SkyPlane[index].y = positionY;
                    SkyPlane[index].z = positionZ;
                    SkyPlane[index].tu = tu;
                    SkyPlane[index].tv = tv;
                }
            }

            return true;
        }
        private bool LoadTextures(SharpDX.Direct3D11.Device device, string textureFilename1, string textureFilename2)
        {
            // Create the first cloud texture object.
            CloudTexture1 = new DTexture();

            // Initialize the first cloud texture object.
            if (!CloudTexture1.Initialize(device, DSystemConfiguration.DataFilePath + textureFilename1))
                return false;

            // Create the second cloud texture object.
            CloudTexture2 = new DTexture();

            // Initialize the second cloud texture object.
            if (!CloudTexture2.Initialize(device, DSystemConfiguration.DataFilePath + textureFilename2))
                return false;

            return true;
        }
        public void ShurDown()
        {
            // Release the sky plane textures.
            ReleaseTextures();

            // Release the vertex and index buffer that were used for rendering the sky plane.
            ShutdownBuffers();

            // Release the sky plane array.
            ShutdownSkyPlane();
        }
        private void ReleaseTextures()
        {
            // Release the texture objects.
            CloudTexture1?.ShutDown();
            CloudTexture1 = null;
            CloudTexture2?.ShutDown();
            CloudTexture2 = null;
        }
        private void ShutdownBuffers()
        {
            // Release the index buffer.
            IndexBuffer?.Dispose();
            IndexBuffer = null;
            // Release the vertex buffer.
            VertexBuffer?.Dispose();
            VertexBuffer = null;
        }
        private void ShutdownSkyPlane()
        {
            // Release the sky plane array.
            if (SkyPlane != null)
                SkyPlane = null;
        }
        public void Render(DeviceContext deviceContext)
        {
            // Render the sky plane.
            RenderBuffers(deviceContext);
        }
        private void RenderBuffers(DeviceContext deviceContext)
        {
            // Set vertex buffer stride and offset.
            // Set the vertex buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, Utilities.SizeOf<DVertexType>(), 0));

            // Set the index buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);

            // Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles.
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }
        public void Frame()
        {
            // Increment the translation values to simulate the moving clouds.
            // This matches the C++ implementation for comparing FPS, when not using VSync.
            m_TextureTranslation[0] += (m_TranslationSpeed[0] );  
            m_TextureTranslation[1] += (m_TranslationSpeed[1] );   
            m_TextureTranslation[2] += (m_TranslationSpeed[2] );   
            m_TextureTranslation[3] += (m_TranslationSpeed[3] ); 

            // Keep the values in the zero to one range.
            if (m_TextureTranslation[0] > 1.0f) 
                m_TextureTranslation[0] -= 1.0f;
            if (m_TextureTranslation[1] > 1.0f) 
                m_TextureTranslation[1] -= 1.0f;
            if (m_TextureTranslation[2] > 1.0f) 
                m_TextureTranslation[2] -= 1.0f;
            if (m_TextureTranslation[3] > 1.0f) 
                m_TextureTranslation[3] -= 1.0f;
        }
    }
}