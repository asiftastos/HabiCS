using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using LGL.Gfx;
using OpenTK.Mathematics;

namespace Lighting
{
    public class Block : IDisposable
    {
        private VertexArrayObject _vao;
        private VertexBuffer _vbo;
        private VertexBuffer _normalsVbo;
        private VertexBuffer _ebo;
        private int elementCount;
        
        public Color4 Color { get; set; }

        public Block()
        {
            _vao = new VertexArrayObject();
            _vbo = new VertexBuffer(BufferTarget.ArrayBuffer);
            _normalsVbo = new VertexBuffer(BufferTarget.ArrayBuffer);
            _ebo = new VertexBuffer(BufferTarget.ElementArrayBuffer);
        }

        public void Init()
        {
            float[] vertices =
            {
                0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 1.0f,
                0.0f, 1.0f, 1.0f,

                1.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f,
                1.0f, 1.0f, 0.0f,
                1.0f, 1.0f, 1.0f,
                
                1.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 0.0f,
                0.0f, 1.0f, 0.0f,
                1.0f, 1.0f, 0.0f,
                
                0.0f, 0.0f, 0.0f,
                0.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 1.0f,
                0.0f, 1.0f, 0.0f,
                
                0.0f, 1.0f, 0.0f,
                0.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 1.0f,
                1.0f, 1.0f, 0.0f,
                
                0.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f
            };

            float[] normals =
            {
                0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f,

                1.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 0.0f,

                0.0f, 0.0f, -1.0f,
                0.0f, 0.0f, -1.0f,
                0.0f, 0.0f, -1.0f,
                0.0f, 0.0f, -1.0f,

                -1.0f, 0.0f, 0.0f,
                -1.0f, 0.0f, 0.0f,
                -1.0f, 0.0f, 0.0f,
                -1.0f, 0.0f, 0.0f,

                0.0f, 1.0f, 0.0f,
                0.0f, 1.0f, 0.0f,
                0.0f, 1.0f, 0.0f,
                0.0f, 1.0f, 0.0f,

                0.0f, -1.0f, 0.0f,
                0.0f, -1.0f, 0.0f,
                0.0f, -1.0f, 0.0f,
                0.0f, -1.0f, 0.0f
            };

            uint[] indices =
            {
                0, 1, 2, 0, 2, 3,
                4, 5, 6, 4, 6, 7,
                8, 9, 10, 8, 10, 11,
                12, 13, 14, 12, 14, 15,
                16, 17, 18, 16, 18, 19,
                20, 21, 22, 20, 22, 23
            };

            elementCount = indices.Length;

            _vao.Set();
            _vbo.Set();
            _vbo.Data<float>(BufferUsageHint.StaticDraw, vertices, sizeof(float) * 3);
            _vao.Attributes(new VertexAttribute[] {
                new VertexAttribute(0, 3, sizeof(float) * 3, 0)
            }, VertexAttribPointerType.Float);
            _normalsVbo.Set();
            _normalsVbo.Data<float>(BufferUsageHint.StaticDraw, normals, sizeof(float) * 3);
            _vao.Attributes(new VertexAttribute[] {
                new VertexAttribute(1, 3, sizeof(float) * 3, 0)
            }, VertexAttribPointerType.Float);

            _ebo.Set();
            _ebo.Data<uint>(BufferUsageHint.StaticDraw, indices, sizeof(uint));
        }

        public void Draw()
        {
            _vao.Set();
            _ebo.Set();
            GL.DrawElements(BeginMode.Triangles, elementCount, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose()
        {
            _vao.Dispose();
            _vbo.Dispose();
            _normalsVbo.Dispose();
            _ebo.Dispose();
        }
    }
}
