using DSharpDXRastertek.Series2.TutTerr04.Graphics.Models;
using DSharpDXRastertek.Series2.TutTerr04.System;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace DSharpDXRastertek.Series2.TutTerr04.Graphics.Data
{
    public class DFont
    {
        // Structs
        [StructLayout(LayoutKind.Sequential)]
        public struct DVertexType
        {
            internal Vector3 position;
            internal Vector2 texture;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct DFontType
        {
            internal float left, right;
            internal int size;
        }

        // Properties.
        public int SpaceSize { get; set; }
        public float FontHeight { get; set; }
        internal DFontType[] Fonts { get; set; }
        public DTexture Texture { get; private set; }

        // Methods
        public bool Initialize(Device device, string fontFileName, string textureFileName, float fontHeight, int spaceSize)
        {
            // Store the height of the font.
            FontHeight = fontHeight;

            // Store the size of spaces in pixel size.
            SpaceSize = spaceSize;

            // Load in the text file containing the font data.
            if (!LoadFontData(fontFileName))
                return false;

            // Load the texture that has the font characters on it.
            if (!LoadTexture(device, textureFileName))
                return false;

            return true;
        }
        public void Shutdown()
        {
            // Release the font texture.
            ReleaseTexture();

            // Release the font data.
            ReleaseFontData();
        }
        private bool LoadFontData(string fontFileName)
        {
            // Create the font spacing buffer. Therre are 95 alphanumeric charecters in this font in the Text file.
            Fonts = new DFontType[95];

            try
            {
                fontFileName = DSystemConfiguration.FontFilePath + fontFileName;

                // Get all the lines containing the font data.
                var fontDataLines = File.ReadAllLines(fontFileName);

                // Create Font and fill with characters.
                // Read in the 95 used ascii characters for text.
                int index = 0;
                foreach (var line in fontDataLines)
                {
                    var modelArray = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    // The last Second last nad third last values are the ones we need.
                    Fonts[index++] = new DFontType()
                    {
                        left = float.Parse(modelArray[modelArray.Length - 3]),
                        right = float.Parse(modelArray[modelArray.Length - 2]),
                        size = int.Parse(modelArray[modelArray.Length - 1])
                    };
                }

                // Close the file.
                fontDataLines = null;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private void ReleaseFontData()
        {
            // Release the font data array.
            if (Fonts != null)
            {
                Fonts = null;
            }
        }
        private bool LoadTexture(Device device, string textureFileName)
        {
            textureFileName = DSystemConfiguration.FontFilePath + textureFileName;

            // Create new texture object.
            Texture = new DTexture();

            // Initialize the texture object.
            if (!Texture.Initialize(device, textureFileName))
                return false;

            return true;
        }
        private void ReleaseTexture()
        {
            // Release the texture object.
            Texture?.ShutDown();
            Texture = null;
        }
        public void BuildVertexArray(out List<DVertexType> vertices, string sentence, float drawX, float drawY)
        {
            // Create list of the vertices
            vertices = new List<DVertexType>();

            // Draw each letter onto a quad.
            foreach (char ch in sentence)
            {
                var letter = ch - 32;

                // If the letter is a space then just move over three pixel.
                if (letter == 0)
                    drawX += 3;
                else
                {
                    // Add quad vertices for the character.
                    BuildVertexArray(vertices, letter, ref drawX, ref drawY);

                    // Update the x location for drawing be the size of the letter and one pixel.
                    drawX += Fonts[letter].size + 1;
                }
            }
        }
        private void BuildVertexArray(List<DVertexType> vertices, int letter, ref float drawX, ref float drawY)
        {
            // First triangle in the quad
            vertices.Add // Top left.
            (
                new DVertexType()
                {
                    position = new Vector3(drawX, drawY, 0),
                    texture = new Vector2(Fonts[letter].left, 0)
                }
            );
            vertices.Add // Bottom right.
            (
                new DVertexType()
                {
                    position = new Vector3(drawX + Fonts[letter].size, drawY - FontHeight, 0),
                    texture = new Vector2(Fonts[letter].right, 1)
                }
            );
            vertices.Add // Bottom left.
            (
                new DVertexType()
                {
                    position = new Vector3(drawX, drawY - FontHeight, 0),
                    texture = new Vector2(Fonts[letter].left, 1)
                }
            );
            // Second triangle in quad.
            vertices.Add // Top left.
            (
                new DVertexType()
                {
                    position = new Vector3(drawX, drawY, 0),
                    texture = new Vector2(Fonts[letter].left, 0)
                }
            );
            vertices.Add // Top right.
            (
                new DVertexType()
                {
                    position = new Vector3(drawX + Fonts[letter].size, drawY, 0),
                    texture = new Vector2(Fonts[letter].right, 0)
                }
            );
            vertices.Add // Bottom right.
            (
                new DVertexType()
                {
                    position = new Vector3(drawX + Fonts[letter].size, drawY - FontHeight, 0),
                    texture = new Vector2(Fonts[letter].right, 1)
                }
            );
        }
    }
}
