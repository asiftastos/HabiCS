using System;
using OpenTK.Graphics.OpenGL4;

namespace LGL.Gfx
{
    public class VertexBuffer : IDisposable
    {
        private int _vbo;
        private BufferTarget _target;

        public VertexBuffer(BufferTarget bufferTarget)
        {
            _target = bufferTarget;
            _vbo = GL.GenBuffer();
        }


        public void Dispose()
        {
            GL.DeleteBuffer(_vbo);
        }

        public void Set()
        {
            GL.BindBuffer(_target, _vbo);
        }

        public unsafe void Data<T>(BufferUsageHint usage, T[] data, int elementSize) where T : struct
        {
            GL.BufferData(_target, data.Length * elementSize, data, usage);
        }
    }
}
