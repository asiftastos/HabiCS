using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace HabiCS.World
{
    public class Map : IDisposable
    {
        private int debugVertexBuffer;
        private int debugColorBuffer;
        private int vao;
        private int debugVertCount;

        private Dictionary<Vector2i, Chunk> chunks;

        public int Size { get; set; }

        public bool ShowDebug { get; set; }

        public Map(int size)
        {
            vao = GL.GenVertexArray();
            debugVertexBuffer = GL.GenBuffer();
            debugColorBuffer = GL.GenBuffer();

            Size = size;
            if(size <= 0)
                Size = 2;

            List<float> vertices = new List<float>();
            List<float> colors = new List<float>();
            chunks = new Dictionary<Vector2i, Chunk>();
            for(int x = 0; x < Size; x++)
            {
                for(int z = 0; z < Size; z++)
                {
                    Chunk c = new Chunk(x, z);
                    chunks.Add(new Vector2i(x, z), c);

                    Vector2i chunkWorldPos = c.Position * Chunk.CHUNK_SIZE; //center of chunk
                    Vector3 chunkWorldCoords = new Vector3(chunkWorldPos.X, 0.0f, chunkWorldPos.Y);
                    float[] verts = new float[] {
                        // center line
                        //(float)chunkWorldPos.X, 0.0f, (float)chunkWorldPos.Y,
                        //(float)chunkWorldPos.X, (float)Chunk.CHUNK_HEIGHT, (float)chunkWorldPos.Y, 
                        chunkWorldCoords.X, 0.0f, chunkWorldCoords.Z,
                        chunkWorldCoords.X, Chunk.CHUNK_HEIGHT, chunkWorldCoords.Z,
                        chunkWorldCoords.X + Chunk.CHUNK_SIZE, 0.0f, chunkWorldCoords.Z,
                        chunkWorldCoords.X + Chunk.CHUNK_SIZE, Chunk.CHUNK_HEIGHT, chunkWorldCoords.Z,
                        chunkWorldCoords.X + Chunk.CHUNK_SIZE, 0.0f, chunkWorldCoords.Z + Chunk.CHUNK_SIZE,
                        chunkWorldCoords.X + Chunk.CHUNK_SIZE, Chunk.CHUNK_HEIGHT, chunkWorldCoords.Z + Chunk.CHUNK_SIZE,
                        chunkWorldCoords.X, 0.0f, chunkWorldCoords.Z + Chunk.CHUNK_SIZE,
                        chunkWorldCoords.X, Chunk.CHUNK_HEIGHT, chunkWorldCoords.Z + Chunk.CHUNK_SIZE,
                    };
                    float[] colrs = new float[] {
                        //0.0f, 0.0f, 1.0f,
                        //0.0f, 0.0f, 1.0f,
                        1.0f, 0.0f, 0.0f,
                        1.0f, 0.0f, 0.0f,
                        1.0f, 0.0f, 0.0f,
                        1.0f, 0.0f, 0.0f,
                        1.0f, 0.0f, 0.0f,
                        1.0f, 0.0f, 0.0f,
                        1.0f, 0.0f, 0.0f,
                        1.0f, 0.0f, 0.0f,
                    };

                    vertices.AddRange(verts);
                    colors.AddRange(colrs);
                }
            }

            debugVertCount = vertices.Count / 3;

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, debugVertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, Vector3.SizeInBytes * (vertices.Count / 3), vertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, debugColorBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, Vector3.SizeInBytes * (colors.Count / 3), colors.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.BindVertexArray(0);

            foreach (var item in chunks)
            {
                Chunk c = item.Value;
                Vector2i pos = c.Position;
                if(chunks.ContainsKey(new Vector2i(pos.X, pos.Y + 1)))
                    c.Neighbours.Set(0, true);
                if(chunks.ContainsKey(new Vector2i(pos.X + 1, pos.Y)))
                    c.Neighbours.Set(1, true);
                if(chunks.ContainsKey(new Vector2i(pos.X, pos.Y - 1)))
                    c.Neighbours.Set(2, true);
                if(chunks.ContainsKey(new Vector2i(pos.X - 1, pos.Y)))
                    c.Neighbours.Set(3, true);
            }
        }

        public bool ContainsChunk(Vector2i pos)
        {
            return chunks.ContainsKey(pos);
        }

        public Chunk GetChunk(Vector2i pos)
        {
            if(ContainsChunk(pos))
                return chunks[pos];
            return new Chunk(-1, -1); //default for no chunk found
        }

        public void Populate(ChunkGenerator generator, ChunkMeshBuilder meshBuilder)
        {
            foreach (KeyValuePair<Vector2i, Chunk> entry in chunks)
            {
                generator.GenerateFlat(entry.Value, 12);
                //generator.Generate(entry.Value);
                //chunkMeshBuilder.BuildMesh(entry.Value, blockSize);
                meshBuilder.BuildMeshCubes(this, entry.Value);
            }
        }

        public void Draw(ref int totalVerts)
        {
            foreach (KeyValuePair<Vector2i, Chunk> entry in chunks)
            {
                entry.Value.Draw();
                totalVerts += entry.Value.VertCount;
            }
            if(ShowDebug)
            {
                GL.BindVertexArray(vao);
                GL.DrawArrays(PrimitiveType.Lines, 0, debugVertCount);
                GL.BindVertexArray(0);
            }
        }

        #region DISPOSABLE PATTERN

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (KeyValuePair<Vector2i, Chunk> entry in chunks)
                    {
                        entry.Value.Dispose();
                    }

                    GL.DeleteBuffer(debugColorBuffer);
                    GL.DeleteBuffer(debugVertexBuffer);
                    GL.DeleteVertexArray(vao);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Scene()
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