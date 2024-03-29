﻿using LGL.Gfx;
using LGL.Loaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


namespace VoxModel
{
    public class VoxModel : IDisposable
    {
        private VertexArrayObject _vao;
        private VertexBuffer _vb;
        private VertexBuffer _ib;
        private int _iCount;
        private Vector3 _size; // the size of the model
        private Vector3 _position; //position of the vox model
        private float _voxelSize;
        private Vector3 _voxelCount; //number of voxels for each axis

        private List<VertexColorTexture> _verts;
        private List<uint> _indices;

        //debug
        private VertexArrayObject _dbgVao;
        private VertexBuffer _dbgVB;
        private int _dbgVCount;

        public bool DebugDraw { get; set; }

        public Vector3 Max { get { return _position + _size; } }

        public Vector3 Min { get { return _position; } }

        public VoxModel(Vector3 pos, Vector3 size, Vector3 voxelCount)
        {
            _voxelCount = voxelCount;
            _size = size;
            _position = pos;
            _voxelSize = _size.X / _voxelCount.X; //same for all axes for now
            _iCount = 0;
            _verts = new List<VertexColorTexture>();
            _indices = new List<uint>();
            _vao = new VertexArrayObject(VertexColorTexture.SizeInBytes);
            _vb = new VertexBuffer(BufferTarget.ArrayBuffer);
            _ib = new VertexBuffer(BufferTarget.ElementArrayBuffer);

            DebugDraw = false;
            _dbgVCount = 0;
            _dbgVao = new VertexArrayObject(VertexColor.SizeInBytes);
            _dbgVB = new VertexBuffer(BufferTarget.ArrayBuffer);
            SetupDebug();
        }

        public void Dispose()
        {
            _dbgVB.Dispose();
            _dbgVao.Dispose();
            _ib.Dispose();
            _vao.Dispose();
            _vb.Dispose();
        }

        public void Draw()
        {
            _vao.Enable();
            _ib.Enable();
            GL.DrawElements(BeginMode.Triangles, _iCount, DrawElementsType.UnsignedInt, 0);

            if(DebugDraw)
            {
                _dbgVao.Enable();
                //_dbgVB.Set();
                GL.DrawArrays(PrimitiveType.Lines, 0, _dbgVCount);
            }
        }

        public void AddVoxel(Vox.Voxel voxel, Color4 color, float u, float udt) 
        {
            int vindex = _verts.Count;

            Vector3 v = new Vector3(voxel.X, voxel.Y, voxel.Z);
            Vector3 minpos = ((Min * v) + v) * _voxelSize;
            Vector3 maxpos = new Vector3(minpos.X + _voxelSize, minpos.Y + _voxelSize, minpos.Z + _voxelSize);

            _verts.AddRange(new VertexColorTexture[]
            {
                new VertexColorTexture(new Vector3(minpos.X, minpos.Y, maxpos.Z), color, new Vector2(u, 0.0f)),
                new VertexColorTexture(new Vector3(maxpos.X, minpos.Y, maxpos.Z), color, new Vector2(u, 0.0f)),
                new VertexColorTexture(new Vector3(maxpos.X, maxpos.Y, maxpos.Z), color, new Vector2(u, 1.0f)),
                new VertexColorTexture(new Vector3(minpos.X, maxpos.Y, maxpos.Z), color, new Vector2(u, 1.0f)),

                new VertexColorTexture(new Vector3(maxpos.X, minpos.Y, maxpos.Z), color, new Vector2(u, 0.0f)),
                new VertexColorTexture(new Vector3(maxpos.X, minpos.Y, minpos.Z), color, new Vector2(u, 0.0f)),
                new VertexColorTexture(new Vector3(maxpos.X, maxpos.Y, minpos.Z), color, new Vector2(u, 1.0f)),
                new VertexColorTexture(new Vector3(maxpos.X, maxpos.Y, maxpos.Z), color, new Vector2(u, 1.0f)),

                new VertexColorTexture(new Vector3(maxpos.X, minpos.Y, minpos.Z), color, new Vector2(u, 0.0f)),
                new VertexColorTexture(new Vector3(minpos.X, minpos.Y, minpos.Z), color, new Vector2(u, 0.0f)),
                new VertexColorTexture(new Vector3(minpos.X, maxpos.Y, minpos.Z), color, new Vector2(u, 1.0f)),
                new VertexColorTexture(new Vector3(maxpos.X, maxpos.Y, minpos.Z), color, new Vector2(u, 1.0f)),

                new VertexColorTexture(new Vector3(minpos.X, minpos.Y, minpos.Z), color, new Vector2(u, 0.0f)),
                new VertexColorTexture(new Vector3(minpos.X, minpos.Y, maxpos.Z), color, new Vector2(u, 0.0f)),
                new VertexColorTexture(new Vector3(minpos.X, maxpos.Y, maxpos.Z), color, new Vector2(u, 1.0f)),
                new VertexColorTexture(new Vector3(minpos.X, maxpos.Y, minpos.Z), color, new Vector2(u, 1.0f)),

                new VertexColorTexture(new Vector3(minpos.X, maxpos.Y, maxpos.Z), color, new Vector2(u, 0.0f)),
                new VertexColorTexture(new Vector3(maxpos.X, maxpos.Y, maxpos.Z), color, new Vector2(u, 0.0f)),
                new VertexColorTexture(new Vector3(maxpos.X, maxpos.Y, minpos.Z), color, new Vector2(u, 1.0f)),
                new VertexColorTexture(new Vector3(minpos.X, maxpos.Y, minpos.Z), color, new Vector2(u, 1.0f)),
            });

            _indices.AddRange(new uint[] {
                //front
                (uint)vindex, (uint)(vindex + 2), (uint)(vindex + 3),
                (uint)vindex, (uint)(vindex + 1), (uint)(vindex + 2),

                //right
                (uint)(vindex + 4), (uint)(vindex + 5), (uint)(vindex + 6),
                (uint)(vindex + 4), (uint)(vindex + 6), (uint)(vindex + 7),

                //back
                (uint)(vindex + 8), (uint)(vindex + 9), (uint)(vindex + 10),
                (uint)(vindex + 8), (uint)(vindex + 10), (uint)(vindex + 11),


                //left
                (uint)(vindex + 12), (uint)(vindex + 13), (uint)(vindex + 14),
                (uint)(vindex + 12), (uint)(vindex + 14), (uint)(vindex + 15),

                //top
                (uint)(vindex + 16), (uint)(vindex + 17), (uint)(vindex + 18),
                (uint)(vindex + 16), (uint)(vindex + 18), (uint)(vindex + 19),
            });
        }

        public void BuildMesh()
        {
            _vao.Enable();
            _vb.Enable();
            _vb.Data<VertexColorTexture>(BufferUsageHint.StaticDraw, _verts.ToArray(), VertexColorTexture.SizeInBytes);
            _vao.Attributes(new VertexAttribute[]
            {
                new VertexAttribute(0, 3, 0),
                new VertexAttribute(1, 3, Vector3.SizeInBytes),
                new VertexAttribute(2, 2, Vector3.SizeInBytes * 2),
            }, VertexAttribPointerType.Float);
            _ib.Enable();
            _ib.Data<uint>(BufferUsageHint.StaticDraw, _indices.ToArray(), sizeof(uint));
            _iCount = _indices.Count;

            Console.WriteLine($"Model: {_verts.Count} vertices, {_indices.Count} indices");
            _verts.Clear();
            _indices.Clear();
        }

        private void SetupDebug()
        {
            List<VertexColor> verts = new List<VertexColor>();
            verts.AddRange(new VertexColor[]
            {
                new VertexColor(Min.X, Min.Y, Min.Z, 1.0f, 1.0f, 1.0f),
                new VertexColor(Max.X, Min.Y, Min.Z, 1.0f, 1.0f, 1.0f),
                new VertexColor(Max.X, Min.Y, Min.Z, 1.0f, 1.0f, 1.0f),
                new VertexColor(Max.X, Max.Y, Min.Z, 1.0f, 1.0f, 1.0f),
                new VertexColor(Max.X, Max.Y, Min.Z, 1.0f, 1.0f, 1.0f),
                new VertexColor(Min.X, Max.Y, Min.Z, 1.0f, 1.0f, 1.0f),
                new VertexColor(Min.X, Min.Y, Min.Z, 1.0f, 1.0f, 1.0f),
                new VertexColor(Min.X, Max.Y, Min.Z, 1.0f, 1.0f, 1.0f),

                new VertexColor(Min.X, Min.Y, Max.Z, 1.0f, 1.0f, 1.0f),
                new VertexColor(Max.X, Min.Y, Max.Z, 1.0f, 1.0f, 1.0f),
                new VertexColor(Max.X, Min.Y, Max.Z, 1.0f, 1.0f, 1.0f),
                new VertexColor(Max.X, Max.Y, Max.Z, 1.0f, 1.0f, 1.0f),
                new VertexColor(Max.X, Max.Y, Max.Z, 1.0f, 1.0f, 1.0f),
                new VertexColor(Min.X, Max.Y, Max.Z, 1.0f, 1.0f, 1.0f),
                new VertexColor(Min.X, Min.Y, Max.Z, 1.0f, 1.0f, 1.0f),
                new VertexColor(Min.X, Max.Y, Max.Z, 1.0f, 1.0f, 1.0f),

                new VertexColor(Min.X, Min.Y, Min.Z, 1.0f, 1.0f, 1.0f),
                new VertexColor(Min.X, Min.Y, Max.Z, 1.0f, 1.0f, 1.0f),
                new VertexColor(Min.X, Max.Y, Min.Z, 1.0f, 1.0f, 1.0f),
                new VertexColor(Min.X, Max.Y, Max.Z, 1.0f, 1.0f, 1.0f),

                new VertexColor(Max.X, Min.Y, Min.Z, 1.0f, 1.0f, 1.0f),
                new VertexColor(Max.X, Min.Y, Max.Z, 1.0f, 1.0f, 1.0f),
                new VertexColor(Max.X, Max.Y, Min.Z, 1.0f, 1.0f, 1.0f),
                new VertexColor(Max.X, Max.Y, Max.Z, 1.0f, 1.0f, 1.0f),
            });
            _dbgVCount = verts.Count;

            
            _dbgVao.Enable();
            _dbgVB.Enable();
            _dbgVB.Data<VertexColor>(BufferUsageHint.StaticDraw, verts.ToArray(), VertexColor.SizeInBytes);
            _dbgVao.Attributes(new VertexAttribute[] {
                new VertexAttribute(0, 3, 0),
                new VertexAttribute(1, 3, Vector3.SizeInBytes)
            }, VertexAttribPointerType.Float);
        }
    }
}
