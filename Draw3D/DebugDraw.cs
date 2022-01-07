using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using LGL.Loaders;
using LGL.Gfx;

namespace Draw3D
{
    public class DebugDraw : IDisposable
    {
        private VertexArrayObject _vao;
        private VertexBuffer _vbo;
        private List<VertexColor> _verts;
        private int _vCount;
        private Matrix4 _model;

        public Matrix4 Model { get { return _model; } }

        public DebugDraw()
        {
            _verts = new List<VertexColor>();
            _verts.AddRange(new VertexColor[] {
                new VertexColor(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f),
                new VertexColor(1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f),
                new VertexColor(0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f),
                new VertexColor(0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f),
                new VertexColor(0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f),
                new VertexColor(0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f),
            });
            _vCount = _verts.Count;

            _vao = new VertexArrayObject();
            _vao.Set();
            _vbo = new VertexBuffer(BufferTarget.ArrayBuffer);
            _vbo.Set();
            _vbo.Data<VertexColor>(BufferUsageHint.StaticDraw, _verts.ToArray(), VertexColor.SizeInBytes);
            _vao.Attributes(new VertexAttribute[] {
                new VertexAttribute(0, 3, VertexColor.SizeInBytes, 0),
                new VertexAttribute(1, 3, VertexColor.SizeInBytes, Vector3.SizeInBytes)
            }, VertexAttribPointerType.Float);

            _model = Matrix4.Identity;
        }

        public void Draw()
        {
            _vao.Set();
            GL.DrawArrays(PrimitiveType.Lines, 0, _vCount);
        }

        public void Dispose()
        {
            _verts.Clear();
            _vao?.Dispose();
            _vbo?.Dispose();

        }
    }
}
