using OpenTK.Mathematics;

namespace LGL.Gfx
{
    public struct VertexColor
    {
        public Vector3 position;
        public Vector3 color;

        public VertexColor(float x, float y, float z, float r, float g, float b)
        {
            position = new Vector3(x, y, z);
            color = new Vector3(r, g, b);
        }

        public VertexColor(float x, float y, float z, Color4 c)
            :this(x, y, z, c.R, c.G, c.B)
        {
        }

        public VertexColor(Vector3 p, Color4 c)
        {
            position = p;
            color = new Vector3(c.R, c.G, c.B);
        }

        public static int SizeInBytes { get { return Vector3.SizeInBytes * 2; } }
    }
}
