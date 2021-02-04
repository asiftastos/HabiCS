using System;
using OpenTK.Graphics.OpenGL4;
using HabiCS.Graphics;

namespace HabiCS.UI
{
    public class UIMesh: IDisposable
    {
        public struct Attribute
        {
            public int Index;
            public int Size;
            public int NumOfElements;
            public int Offset;

            public Attribute(int indx, int size, int numOfElements, int offset)
            {
                Index = indx;
                Size = size;
                NumOfElements = numOfElements;
                Offset = offset;
            }    
        }

        private int vao;
        private int vbo;
        private int vertCount;

        public int VerticesCount { get { return vertCount; } }

        public UIMesh()
        {
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            vertCount = 0;
        }

        public void Build(float[] verts, Attribute[] attributes)
        {
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * sizeof(float), verts, BufferUsageHint.StaticDraw);
            foreach (var attr in attributes)
            {
                GL.VertexAttribPointer(attr.Index, attr.Size, VertexAttribPointerType.Float, false, 
                                        sizeof(float) * attr.NumOfElements, attr.Offset);
                GL.EnableVertexAttribArray(attr.Index);
            }
            GL.BindVertexArray(0);

            vertCount = verts.Length / attributes[0].NumOfElements;
        }

        public void BuildText(TextureVertex[] verts, Attribute[] attributes)
        {
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * TextureVertex.SizeInBytes, verts, BufferUsageHint.StaticDraw);
            foreach (var attr in attributes)
            {
                GL.VertexAttribPointer(attr.Index, attr.Size, VertexAttribPointerType.Float, false, 
                                        TextureVertex.SizeInBytes, attr.Offset);
                GL.EnableVertexAttribArray(attr.Index);
            }
            GL.BindVertexArray(0);

            vertCount = verts.Length / attributes[0].NumOfElements;
        }

        public void Draw(PrimitiveType type)
        {
            GL.BindVertexArray(vao);
            GL.DrawArrays(type, 0, vertCount);
            GL.BindVertexArray(0);
        }

        public void DrawIndexed()
        {
            GL.BindVertexArray(vao);
        }

        #region DISPOSABLE PATTERN

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    GL.DeleteBuffer(vbo);
                    GL.DeleteVertexArray(vao);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion        
    }
}