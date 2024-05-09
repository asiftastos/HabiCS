using Silk.NET.Maths;

namespace Habi.Graphics.OpenGL
{
    public record struct VertexColor(Vector3D<float> vertex, Vector3D<float> color)
    {
        public static int SizeInBytes => sizeof(float) * 6;
    }
}
