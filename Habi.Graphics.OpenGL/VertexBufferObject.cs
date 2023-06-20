using Silk.NET.OpenGL;

namespace Habi.Graphics.OpenGL
{
    public class VertexBufferObject : IDisposable
    {
        private GL _gl;
        private uint _id;
        private int _size; //in bytes
        private BufferTargetARB _target;
        private BufferUsageARB _usage;

        public int Size => _size;

        internal unsafe VertexBufferObject(GL gl, BufferTargetARB target, int size, void* data, BufferUsageARB usage)
        {
            _gl = gl;
            _target = target;
            _usage = usage;
            _id = _gl.CreateBuffer();

            if(size > 0 )
            {
                Enable();
                UploadData(size, data, _usage);
            }
        }

        public void Enable()
        {
            _gl.BindBuffer(_target, _id);
        }

        public unsafe void UploadData(int size, void* data, BufferUsageARB usage)
        {
            _size = size;
            _gl.BufferData(_target, (nuint)_size, data, usage);
        }

        public unsafe void UpdateData<T>(int offset, uint size, ReadOnlySpan<T> data)
        {
            //fixed(T* d = data)
            //{
            //    _gl.BufferSubData(_target, offset, size, (void*)d);
            //}
        }

        public void Dispose()
        {
            _gl.DeleteBuffer(_id);
        }
    }
}
