using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using HabiCS.Loaders;

namespace HabiCS.UI
{
    public class PanelElem: IDisposable
    {
        private int vao;
        private int vbo;

        private int vertCount;

        private Shader shader;

        private Matrix4 model;

        public  Matrix4 Model { get { return model; } }

        public UIRect BoundBox { get; set; }

        public PanelElem(float x, float y, float width, float height)
        {
            this.BoundBox = new UIRect((int)x, (int)y, (int)width, (int)height);
            shader = Shader.Load("UI", 2, "Assets/Shaders/ui.vert", "Assets/Shaders/ui.frag");
            shader.SetupUniforms(new string[]{"ortho", "model"});

            model = Matrix4.CreateScale(1.0f, 1.0f, 1.0f);

            Vector2i posMin = BoundBox.Position;
            Vector2i posMax = Vector2i.Add(BoundBox.Position, BoundBox.Size);
            float[] verts = new float[]{
                posMin.X, posMin.Y, -1.0f,
                0.0f, 0.0f, 1.0f,
                posMax.X, posMin.Y, -1.0f,
                0.0f, 0.0f, 1.0f,
                posMax.X, posMax.Y, -1.0f,
                0.0f, 0.0f, 1.0f,
                posMin.X, posMax.Y, -1.0f,
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
            shader.UploadMatrix("model", ref model);
            shader.UploadMatrix("ortho", ref ortho);
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

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}