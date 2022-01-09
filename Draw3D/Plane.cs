using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using LGL.Gfx;
using OpenTK.Mathematics;

namespace Draw3D
{
    public class Plane : IDisposable
    {
        private VertexArrayObject _vertexArrayObject;
        private VertexBuffer _vertexBuffer;
        private VertexBuffer _indexBuffer;
        private int _vCount;
        private Matrix4 _model;
        private int _xSegs;
        private int _ySegs;
        private float _segSide;

        public Matrix4 Model { get { return _model; } }

        public Plane(int xWidth, int zWidth, float sideSize)
        {
            _xSegs = xWidth;
            _ySegs = zWidth;
            _segSide = sideSize;

            _vertexArrayObject = new VertexArrayObject();
            _vertexBuffer = new VertexBuffer(BufferTarget.ArrayBuffer);
            _indexBuffer = new VertexBuffer(BufferTarget.ElementArrayBuffer);
        }

        public void Load()
        {
            CreateData(_xSegs, _ySegs, _segSide);

            _model = Matrix4.Identity;
        }

        public void Draw()
        {
            _vertexArrayObject.Set();
            _indexBuffer.Set();
            GL.DrawElements(BeginMode.Triangles, _vCount, DrawElementsType.UnsignedShort, 0);
        }


        public void Dispose()
        {
            _indexBuffer?.Dispose();
            _vertexBuffer?.Dispose();
            _vertexArrayObject?.Dispose();
        }

        private void CreateData(int xWidth, int zWidth, float sideSize)
        {
            float color = 0.0f;
            List<VertexColor> vertexColors = new List<VertexColor>();
            List<ushort> indices = new List<ushort>();
            for(int z = 0; z < zWidth * sideSize; z += (int)sideSize)
            {
                for (int x = 0; x < xWidth * sideSize; x += (int)sideSize)
                {
                    color += 1.0f / (xWidth * zWidth);
                    if(color >= 1.0f)
                        color = 1.0f;

                    vertexColors.Add(new VertexColor(x, 0.0f, z, 0.0f, color, 0.0f));
                    vertexColors.Add(new VertexColor(x, 0.0f, z + sideSize, 0.0f, color, 0.0f));
                    vertexColors.Add(new VertexColor(x + sideSize, 0.0f, z + sideSize, 0.0f, color, 0.0f));
                    vertexColors.Add(new VertexColor(x + sideSize, 0.0f, z, 0.0f, color, 0.0f));

                    ushort index = (ushort)(vertexColors.Count - 1);
                    indices.AddRange(new ushort[] {(ushort)(index - 3),  (ushort)(index - 2), (ushort)(index - 1), (ushort)(index - 3), (ushort)(index - 1), (ushort)(index) });
                }
            }
            _vCount = indices.Count;

            _vertexArrayObject.Set();
            _vertexBuffer.Set();
            _vertexBuffer.Data<VertexColor>(BufferUsageHint.StaticDraw, vertexColors.ToArray(), VertexColor.SizeInBytes);
            _vertexArrayObject.Attributes(new VertexAttribute[]
            {
                new VertexAttribute(0, 3, VertexColor.SizeInBytes, 0),
                new VertexAttribute(1, 3, VertexColor.SizeInBytes, Vector3.SizeInBytes)
            }, VertexAttribPointerType.Float);
            _indexBuffer.Set();
            _indexBuffer.Data<ushort>(BufferUsageHint.StaticDraw, indices.ToArray(), sizeof(ushort));

            vertexColors.Clear();
            indices.Clear();
        }
    }
}
