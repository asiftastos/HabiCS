using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using HabiCS.Loaders;

namespace HabiCS.UI
{
    public class PanelElem: IDisposable
    {
        private Vector2 position;
        private Vector2 size;

        private int vao;
        private int vbo;

        private int vertCount;

        private Shader shader;
        private int orthoLocation;
        private int modelLocation;

        private Matrix4 model;

        public PanelElem(float x, float y, float width, float height)
        {
            this.position = new Vector2(x, y);
            this.size = new Vector2(width, height);
            shader = Shader.Load("UI", 2, "Assets/Shaders/ui.vert", "Assets/Shaders/ui.frag");
            orthoLocation = GL.GetUniformLocation(shader.ShaderID, "ortho");
            modelLocation = GL.GetUniformLocation(shader.ShaderID, "model");

            model = Matrix4.CreateScale(1.0f, 1.0f, 1.0f);

            float[] verts = new float[]{
                position.X, position.Y, -1.0f,
                0.0f, 0.0f, 1.0f,
                position.X + size.X, position.Y, -1.0f,
                0.0f, 0.0f, 1.0f,
                position.X + size.X, position.Y + size.Y, -1.0f,
                0.0f, 0.0f, 1.0f,
                position.X, position.Y + size.Y, -1.0f,
                0.0f, 0.0f, 1.0f
            };

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * verts.Length, verts, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 6, 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, sizeof(float) * 6, sizeof(float) * 3);
            GL.EnableVertexAttribArray(1);
            GL.BindVertexArray(0);

            vertCount = verts.Length / 6;
        }

        public void Draw(ref Matrix4 ortho)
        {
            shader.Use();
            GL.UniformMatrix4(modelLocation, false, ref model);
            GL.UniformMatrix4(orthoLocation, false, ref ortho);
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.LineLoop, 0, vertCount);
            GL.BindVertexArray(0);
        }

        #region DISPOSABLE PATTERN

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    shader.Dispose();
                    GL.DeleteBuffer(vbo);
                    GL.DeleteVertexArray(vao);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Font()
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