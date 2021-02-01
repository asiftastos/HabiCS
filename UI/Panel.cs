using System;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using HabiCS.Loaders;

namespace HabiCS.UI
{
    public class Panel: IUIElem
    {
        private Matrix4 _model;
        private UIMesh _mesh;

        private UIRect _bounds;

        private UIBackground _background;

        public Panel(float x, float y, float w, float h, Color4 normal)
        {
            _bounds = new UIRect((int)x , (int)y, (int)w, (int)h);
            _background = new UIBackground(normal, Color4.Transparent, Color4.Transparent);
            _mesh = new UIMesh();
            
            Vector2i pMax = Vector2i.Add(_bounds.Position, _bounds.Size);
            float[] verts = new float[] {
                _bounds.Position.X, _bounds.Position.Y, -1.0f,
                pMax.X, _bounds.Position.Y, -1.0f,
                pMax.X, pMax.Y, -1.0f,
                _bounds.Position.X, _bounds.Position.Y, -1.0f,
                pMax.X, pMax.Y, -1.0f,
                _bounds.Position.X, pMax.Y, -1.0f,
            };

            _mesh.Build(verts, new UIMesh.Attribute[] {
                new UIMesh.Attribute(0, 3, 3, 0)
            });

            _model = Matrix4.Identity;
        }

        public void Draw(ref Shader sh)
        {
            sh.UploadMatrix("model",ref _model);
            sh.UploadColor("color", _background.Normal);
            sh.UploadBool("text", false);
            _mesh.Draw(PrimitiveType.Triangles);
        }

        #region DISPOSABLE PATTERN
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _mesh.Dispose();
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