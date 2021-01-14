﻿using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace HabiCS.World
{
    class Chunk : IDisposable
    {
        public static int CHUNK_SIZE = 16;
        public static int CHUNK_HEIGHT = 64;

        private Vector2i position;

        private int vertexBuffer;
        private int vertexArray;
        private int vertCount;
        
        private bool disposedValue;

        public Vector2i Position { get { return position; } }

        public Chunk(int x, int z)
        {
            vertCount = 0;
            position = new Vector2i(x, z);
            vertexArray = GL.GenVertexArray();
            vertexBuffer = GL.GenBuffer();
        }

        public void UpdateMesh(List<Vector3> vertices)
        {
            vertCount = vertices.Count;

            GL.BindVertexArray(vertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, Vector3.SizeInBytes * vertCount, vertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindVertexArray(0);
        }

        public void Draw()
        {
            GL.BindVertexArray(vertexArray);
            GL.DrawArrays(PrimitiveType.Points, 0, vertCount);
            GL.BindVertexArray(0);
        }

        #region DISPOSABLE

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
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
