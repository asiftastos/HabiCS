using Silk.NET.Maths;

namespace Habi.Graphics.OpenGL
{
    record struct VertexColor(Vector2D<float> vertex, Vector4D<float> color)
    {
        public static int SizeInBytes => sizeof(float) * 6;
    }
}
