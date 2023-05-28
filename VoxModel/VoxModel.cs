using LGL.Gfx;
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

        private List<VertexColor> _verts;
        private List<ushort> _indices;

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
            _verts = new List<VertexColor> ();
            _indices = new List<ushort> ();
            _vao = new VertexArrayObject();
            _vb = new VertexBuffer(BufferTarget.ArrayBuffer);
            _ib = new VertexBuffer(BufferTarget.ElementArrayBuffer);

            DebugDraw = false;
            _dbgVCount = 0;
            _dbgVao = new VertexArrayObject();
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
            _vao.Set();
            _ib.Set();
            GL.DrawElements(BeginMode.Triangles, _iCount, DrawElementsType.UnsignedShort, 0);

            if(DebugDraw)
            {
                _dbgVao.Set();
                //_dbgVB.Set();
                GL.DrawArrays(PrimitiveType.Lines, 0, _dbgVCount);
            }
        }

        public void AddVoxel(Vox.Voxel voxel, Color4 color) 
        {
            int vindex = _verts.Count;

            Vector3 v = new Vector3(voxel.X, voxel.Y, voxel.Z);
            Vector3 minpos = ((Min * v) + v) * _voxelSize;
            Vector3 maxpos = new Vector3(minpos.X + _voxelSize, minpos.Y + _voxelSize, minpos.Z + _voxelSize);

            _verts.AddRange(new VertexColor[]
            {
                new VertexColor(minpos.X, minpos.Y, maxpos.Z, color),
                new VertexColor(minpos.X, maxpos.Y, maxpos.Z, color),
                new VertexColor(maxpos.X, maxpos.Y, maxpos.Z, color),
                new VertexColor(maxpos.X, minpos.Y, maxpos.Z, color),
                new VertexColor(minpos.X, minpos.Y, minpos.Z, color),
                new VertexColor(minpos.X, maxpos.Y, minpos.Z, color),
                new VertexColor(maxpos.X, maxpos.Y, minpos.Z, color),
                new VertexColor(maxpos.X, minpos.Y, minpos.Z, color),
            });

            _indices.AddRange(new ushort[] {
                //front
                (ushort)vindex, (ushort)(vindex + 2), (ushort)(vindex + 1),
                (ushort)vindex, (ushort)(vindex + 3), (ushort)(vindex + 2),

                //back
                (ushort)(vindex + 4), (ushort)(vindex + 6), (ushort)(vindex + 7),
                (ushort)(vindex + 4), (ushort)(vindex + 5), (ushort)(vindex + 6),

                //right
                (ushort)(vindex + 3), (ushort)(vindex + 6), (ushort)(vindex + 2),
                (ushort)(vindex + 3), (ushort)(vindex + 7), (ushort)(vindex + 6),

                //left
                (ushort)(vindex + 4), (ushort)(vindex + 1), (ushort)(vindex + 5),
                (ushort)(vindex + 4), (ushort)(vindex), (ushort)(vindex + 1),

                //top
                (ushort)(vindex + 1), (ushort)(vindex + 2), (ushort)(vindex + 6),
                (ushort)(vindex + 1), (ushort)(vindex + 6), (ushort)(vindex + 5),
            });
        }

        public void BuildMesh()
        {
            _vao.Set();
            _vb.Set();
            _vb.Data<VertexColor>(BufferUsageHint.StaticDraw, _verts.ToArray(), VertexColor.SizeInBytes);
            _vao.Attributes(new VertexAttribute[]
            {
                new VertexAttribute(0, 3, VertexColor.SizeInBytes, 0),
                new VertexAttribute(1, 3, VertexColor.SizeInBytes, Vector3.SizeInBytes)
            }, VertexAttribPointerType.Float);
            _ib.Set();
            _ib.Data<ushort>(BufferUsageHint.StaticDraw, _indices.ToArray(), sizeof(ushort));
            _iCount = _indices.Count;

            //Console.WriteLine($"Model: {_verts.Count} vertices, {_indices.Count} indices");
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

            
            _dbgVao.Set();
            _dbgVB.Set();
            _dbgVB.Data<VertexColor>(BufferUsageHint.StaticDraw, verts.ToArray(), VertexColor.SizeInBytes);
            _dbgVao.Attributes(new VertexAttribute[] {
                new VertexAttribute(0, 3, VertexColor.SizeInBytes, 0),
                new VertexAttribute(1, 3, VertexColor.SizeInBytes, Vector3.SizeInBytes)
            }, VertexAttribPointerType.Float);
        }
    }
}
