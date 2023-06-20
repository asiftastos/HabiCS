using Silk.NET.OpenGL;

namespace Habi.Graphics.OpenGL
{
    public class VertexArrayObject : IDisposable
    {
        private GL _gl;
        private uint _id;

        public uint PrimitiveCount { get; set; }

        internal VertexArrayObject(GL gl)
        {
            _gl = gl;
            _id = _gl.CreateVertexArray();
        }

        public void Dispose()
        {
            _gl.DeleteVertexArray(_id);
        }

        public void Enable()
        {
            _gl.BindVertexArray(_id);
        }

        public unsafe void Attributes(VertexAttribute[] attributes)
        {
            if (attributes == null || attributes.Length == 0) return;

            foreach (var attribute in attributes)
            {
                _gl.EnableVertexAttribArray((uint)attribute.Index);
                _gl.VertexAttribPointer((uint)attribute.Index, attribute.Size, VertexAttribPointerType.Float, false, (uint)attribute.Stride, (void*)attribute.Offset);
            }
        }

        public void Draw(PrimitiveType primitiveType, int firstPrimitive)
        {
            Enable();
            _gl.DrawArrays(primitiveType, firstPrimitive, PrimitiveCount);
        }

        public struct VertexAttribute
        {
            public int Index;
            public int Size;
            public int Stride;
            public int Offset;

            public VertexAttribute(int index, int size, int stride, int offset)
            {
                Index = index;
                Size = size;
                Stride = stride;
                Offset = offset;
            }
        }
    }
}
