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

        public Color4 Color { get; set; }

        public Block()
        {
            _vao = new VertexArrayObject(Vector3.SizeInBytes);
            _vbo = new VertexBuffer(BufferTarget.ArrayBuffer);
            _normalsVbo = new VertexBuffer(BufferTarget.ArrayBuffer);
            _ebo = new VertexBuffer(BufferTarget.ElementArrayBuffer);
        }

        public void Init()
        {
            Vector3[] vertices =
            {
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(1.0f, 0.0f, 1.0f),
                new Vector3(1.0f, 1.0f, 1.0f),
                new Vector3(0.0f, 1.0f, 1.0f),

                new Vector3(1.0f, 0.0f, 1.0f),
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 1.0f, 0.0f),
                new Vector3(1.0f, 1.0f, 1.0f),
                
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(1.0f, 1.0f, 0.0f),
                
                new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 1.0f, 1.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 1.0f),
                new Vector3(1.0f, 1.0f, 1.0f),
                new Vector3(1.0f, 1.0f, 0.0f),
                
                new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 1.0f)
            };

            Vector3[] normals =
            {
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(0.0f, 0.0f, 1.0f),

                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f),

                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),
                new Vector3(0.0f, 0.0f, -1.0f),

                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),
                new Vector3(-1.0f, 0.0f, 0.0f),

                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),

                new Vector3(0.0f, -1.0f, 0.0f),
                new Vector3(0.0f, -1.0f, 0.0f),
                new Vector3(0.0f, -1.0f, 0.0f),
                new Vector3(0.0f, -1.0f, 0.0f)
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

            List<Vector3> norms = new List<Vector3>();
            foreach(var v in vertices)
            {
                Vector3[] tmp = FindSharedNormals(v, vertices, normals);
                norms.Add(CalculateNormal(tmp));
            }

            _vao.PrimitiveCount = indices.Length;

            _vao.Enable();
            _vbo.Enable();
            _vbo.Data<Vector3>(BufferUsageHint.StaticDraw, vertices, Vector3.SizeInBytes);
            _vao.Attributes(new VertexAttribute[] {
                new VertexAttribute(0, 3, 0)
            }, VertexAttribPointerType.Float);
            _normalsVbo.Enable();
            //_normalsVbo.Data<Vector3>(BufferUsageHint.StaticDraw, normals, Vector3.SizeInBytes);
            _normalsVbo.Data<Vector3>(BufferUsageHint.StaticDraw, norms.ToArray(), Vector3.SizeInBytes);  //averaged vertex normals of each face it is shared
            _vao.Attributes(new VertexAttribute[] {
                new VertexAttribute(1, 3, 0)
            }, VertexAttribPointerType.Float);

            _ebo.Enable();
            _ebo.Data<uint>(BufferUsageHint.StaticDraw, indices, sizeof(uint));
        }

        public void Draw()
        {
            _ebo.Enable();
            _vao.DrawIndexed(BeginMode.Triangles, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose()
        {
            _vao.Dispose();
            _vbo.Dispose();
            _normalsVbo.Dispose();
            _ebo.Dispose();
        }

        //average normal of a vertex shared in 3 faces
        private Vector3 CalculateNormal(Vector3[] norms)
        {
            return Vector3.Normalize(norms[0] + norms[1] + norms[2]);
        }

        private Vector3[] FindSharedNormals(Vector3 v, Vector3[] verts, Vector3[] norms)
        {
            Vector3[] tmp = new Vector3[3];
            int index = 0;
            for (int i = 0; i < verts.Length; i++)
            {
                if(verts[i].Equals(v))
                {
                    tmp[index] = norms[i];
                    index++;
                }
                if (index == 3)
                    break;
            }
            return tmp;
        }
    }
}
