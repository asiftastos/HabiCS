using OpenTK.Mathematics;

namespace HabiCS.Graphics
{
    public struct TextVertex
    {
        public static int SizeInBytes { get { return Vector3.SizeInBytes + Vector2.SizeInBytes; } }
        public TextVertex(float x, float y, float z, float u, float v)
        {
            position = new Vector3(x, y, z);
            texCoords = new Vector2(u, v);
        }
        public Vector3 position;
        public Vector2 texCoords;
    }
}