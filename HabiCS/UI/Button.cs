using System;
using OpenTK.Windowing.Common;
using LGL.Loaders;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace HabiCS.UI
{
    public class Button: IUIElem
    {
        private UIRect _bounds;
        private UIBackground _background;
        private UIMesh _backgroundMesh;

        private Label _caption;

        private UIRect _margin;

        public bool Inderactable {get; set;}

        public Action OnClicked;

        public Button(float x, float y, float w, float h, string text, Font font)
        {
            Inderactable = true;
            _margin = new UIRect(2, 2, 2, 2);
            _bounds = new UIRect((int)x, (int)y, (int)(x + w), (int)(y + h));
            _caption = new Label(x + _margin.Position.X, y + _margin.Position.Y, 
                                w - _margin.Size.X, h - _margin.Size.Y, text, font);
            
            _background = new UIBackground(Color4.Gray, Color4.LightGray, Color4.DarkGray);
            _backgroundMesh = new UIMesh();
            Vector2i pMax = Vector2i.Add(_bounds.Position, new Vector2i((int)w + (_margin.Size.X * 2),(int)h + (_margin.Size.Y * 2)));
            float[] verts = new float[] {
                _bounds.Position.X, _bounds.Position.Y, -1.0f,
                pMax.X, _bounds.Position.Y, -1.0f,
                pMax.X, pMax.Y, -1.0f,
                _bounds.Position.X, _bounds.Position.Y, -1.0f,
                pMax.X, pMax.Y, -1.0f,
                _bounds.Position.X, pMax.Y, -1.0f,
            };

            _backgroundMesh.Build(verts, new UIMesh.Attribute[] {
                new UIMesh.Attribute(0, 3, 3, 0)
            });
        }

        public bool ProcessMouseDown(MouseButtonEventArgs e, Vector2 mousePos)
        {
            if(e.Button == MouseButton.Left)
            {
                if(mousePos.X > _bounds.Position.X && mousePos.X < _bounds.Size.X &&
                    mousePos.Y > _bounds.Position.Y && mousePos.Y < _bounds.Size.Y)
                    {
                        if(OnClicked is not null)
                            OnClicked();
                        
                        return true;
                    }
            }

            return false;
        }

        public void Draw(ref Shader sh)
        {
            sh.UploadColor("color", _background.Normal);
            sh.UploadBool("text", false);
            _backgroundMesh.Draw(PrimitiveType.Triangles);
            _caption.Draw(ref sh);
        }

        #region DISPOSABLE PATTERN
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _backgroundMesh.Dispose();
                    _caption.Dispose();
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