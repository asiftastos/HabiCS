using OpenTK.Mathematics;

namespace LGL.Gfx
{
    public struct VertexColorTexture
    {
        public Vector3 position;
        public Vector3 color;
        public Vector2 uv;

        public VertexColorTexture(Vector3 position, Vector3 color, Vector2 uv)
        {
            this.position = position;
            this.color = color;
            this.uv = uv;
        }

        public VertexColorTexture(Vector3 position, Color4 color, Vector2 uv)
        {
            this.position = position;
            this.color = new Vector3(color.R, color.G, color.B);
            this.uv = uv;
        }

        public static int SizeInBytes { get { return (Vector3.SizeInBytes * 2) + Vector2.SizeInBytes; } }
    }
}
