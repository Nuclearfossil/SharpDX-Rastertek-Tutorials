using System;

namespace DSharpDXRastertek.Tut08
{
    public class DFaceIndices : ICloneable
    {
        public DFaceIndices(string faceIndices)
        {
            var indices = faceIndices.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            Vertex = int.Parse(indices[0]);
            Texture = int.Parse(indices[1]);
            Normal = int.Parse(indices[2]);
        }

        public int Vertex;
        public int Texture;
        public int Normal;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class DFace
    {
        public DFaceIndices[] vertices;

		public DFace(string face)
		{
			vertices = new DFaceIndices[3];
			var vertexIndices = face.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
			vertices[0] = new DFaceIndices(vertexIndices[0]);
			vertices[1] = new DFaceIndices(vertexIndices[1]);
			vertices[2] = new DFaceIndices(vertexIndices[2]);
		}
    }

    public class DMayaFace : DFace
	{
		public DMayaFace(string face) : base(face)
		{
			var tempVertex = (DFaceIndices)vertices[0].Clone();
			vertices[0] = (DFaceIndices)vertices[2].Clone();
			vertices[2] = (DFaceIndices)tempVertex.Clone();
		}
	}
}
