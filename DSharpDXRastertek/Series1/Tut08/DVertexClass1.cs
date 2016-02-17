using System;

namespace DSharpDXRastertek.Tut08
{
    public class DVertex
    {
        public float x;
		public float y;
		public float z;

		public DVertex(string vertex)
		{
			var vertexCoords = vertex.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
			x = float.Parse(vertexCoords[0]);
			y = float.Parse(vertexCoords[1]);
			z = float.Parse(vertexCoords[2]);
		}
    }


    public class DMayaVertex : DVertex
    {
        public DMayaVertex(string vertex)
            : base(vertex)
        {
            z = -z;
        }
    }
}
