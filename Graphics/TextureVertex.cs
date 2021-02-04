using OpenTK.Mathematics;

namespace HabiCS.Graphics
{
    public struct TextureVertex
    {
        public static int SizeInBytes { get { return Vector3.SizeInBytes + Vector2.SizeInBytes; } }
        public TextureVertex(float x, float y, float z, float u, float v)
        {
            position = new Vector3(x, y, z);
            texCoords = new Vector2(u, v);
        }
        public Vector3 position;
        public Vector2 texCoords;
    }
}