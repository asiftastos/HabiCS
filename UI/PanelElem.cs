using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using HabiCS.Loaders;

namespace HabiCS.UI
{
    public class PanelElem: IDisposable
    {
        private UIMesh mesh;

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

            mesh = new UIMesh();
            mesh.Build(verts, new UIMesh.Attribute[] {
                new UIMesh.Attribute(0, 3, 6, 0),
                new UIMesh.Attribute(1, 3, 6, sizeof(float) * 3)
            });
        }

        public void Draw(ref Matrix4 ortho)
        {
            shader.Use();
            shader.UploadMatrix("model", ref model);
            shader.UploadMatrix("ortho", ref ortho);
            mesh.Draw(PrimitiveType.LineLoop);
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
                    mesh.Dispose();
                }

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