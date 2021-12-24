using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HabiCS.World
{
    public class Chunk : IDisposable
    {
        public static readonly int CHUNK_SIZE = 32;
        public static readonly int CHUNK_HEIGHT = 64;

        private Vector2i position;

        private int vertexBuffer;
        private int colorBuffer;
        private int vertexArray;
        private int vertCount;
        
        private bool disposedValue;

        //0 for air,1 for solid
        public Dictionary<Vector3i, ushort> Blocks {get; set;}

        public Vector2i Position { get { return position; } }

        public int VertCount {get {return vertCount; } }

        //Bit flags for neighbour chunks [front, right, back, left].
        public BitArray Neighbours { get; set; }

        public Chunk(int x, int z)
        {
            Blocks = new Dictionary<Vector3i, ushort>();
            vertCount = 0;
            position = new Vector2i(x, z);
            Neighbours = new BitArray(4);
            for(int i = 0; i < 4; i++)
                Neighbours.Set(i, false);
            vertexArray = GL.GenVertexArray();
            vertexBuffer = GL.GenBuffer();
            colorBuffer = GL.GenBuffer();
        }

        public void UpdateMesh(List<Vector3> vertices, List<Vector3> colors)
        {
            vertCount = vertices.Count;

            GL.BindVertexArray(vertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, Vector3.SizeInBytes * vertCount, vertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, colorBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, Vector3.SizeInBytes * colors.Count, colors.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.BindVertexArray(0);
        }

        public void Draw()
        {
            GL.BindVertexArray(vertexArray);
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertCount);
            GL.BindVertexArray(0);
        }

        public bool IsSolid(Vector3i blockPos) 
        {
            return Blocks.ContainsKey(blockPos) && Blocks[blockPos] == 1;
        }
        
        #region DISPOSABLE

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    GL.DeleteBuffer(colorBuffer);
                    GL.DeleteBuffer(vertexBuffer);
                    GL.DeleteVertexArray(vertexArray);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Chunk()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
