using System;

namespace DSharpDXRastertek.Tut08
{
    public class DTexture
    {
        public float x;
		public float y;

		public DTexture(string texture)
		{
			var textureCoords = texture.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
			x = float.Parse(textureCoords[0]);
			y = float.Parse(textureCoords[1]);
		}
    }

    public class DMayaTexture : DTexture
    {
        public DMayaTexture(string texture)
            : base(texture)
        {
            y = 1 - y;
        }
    }
}
