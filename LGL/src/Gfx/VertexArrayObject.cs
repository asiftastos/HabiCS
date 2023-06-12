using System;
using OpenTK.Graphics.OpenGL4;

namespace LGL.Gfx
{
    /*
     * Vertex array objects needed for version 3.3 and up
     * 
     * _stride: the size in bytes of 1 element to be drawn in a buffer, 
     *          like a single vertex is 3*sizeof(float) (3 floats) or a vertex and color ara 6*sizeof(float) (6 floats)
     */
    public class VertexArrayObject : IDisposable
    {
        private int _vao;
        private int _stride;
        
        /*
         * The number of primitives to draw.
         * Set it after uploaded the data to the buffer controled by this VAO
         * Use it in a DrawArray/DrawElements and such functions
         */
        public int PrimitiveCount { get; set; }

        public VertexArrayObject(int stride)
        {
            _vao = GL.GenVertexArray();
            _stride = stride;
        }


        public void Dispose()
        {
            GL.DeleteVertexArray(_vao);
        }

        public void Enable()
        {
            GL.BindVertexArray(_vao);
        }

        public void Disable()
        {
            GL.BindVertexArray(0);
        }

        public void Attributes(VertexAttribute[] attrs, VertexAttribPointerType attrType)
        {
            foreach(var attr in attrs)
            {
                GL.VertexAttribPointer(attr.Index, attr.ElementsCount, attrType, false, _stride, attr.Offset);
                GL.EnableVertexAttribArray(attr.Index);
            }
        }

        public void Draw(PrimitiveType primitiveType, int first)
        {
            Enable();
            GL.DrawArrays(primitiveType, first, PrimitiveCount);
            Disable();
        }

        public void DrawIndexed(BeginMode mode, DrawElementsType elementsType, int offset)
        {
            Enable();
            GL.DrawElements(mode, PrimitiveCount, elementsType, offset);
            Disable();
        }
    }
}
