using System;
using OpenTK.Graphics.OpenGL4;

namespace LGL.Gfx
{
    public class VertexArrayObject : IDisposable
    {
        private int _vao;

        public VertexArrayObject()
        {
            _vao = GL.GenVertexArray();
        }


        public void Dispose()
        {
            GL.DeleteVertexArray(_vao);
        }

        public void Set()
        {
            GL.BindVertexArray(_vao);
        }

        public void Attributes(VertexAttribute[] attrs, VertexAttribPointerType attrType)
        {
            foreach(var attr in attrs)
            {
                GL.VertexAttribPointer(attr.Index, attr.ElementsCount, attrType, false, attr.Stride, attr.Offset);
                GL.EnableVertexAttribArray(attr.Index);
            }
        }
    }
}
