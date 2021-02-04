using System;
using OpenTK.Windowing.Common;
using HabiCS.Loaders;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common.Input;

namespace HabiCS.UI
{
    public class Button: IUIElem
    {
        private UIRect _bounds;
        private Panel _background;

        private Label _caption;

        private UIRect _margin;

        public bool Inderactable {get; set;}

        public Action OnClicked;

        public Button(float x, float y, float w, float h, string text, Font font)
        {
            Inderactable = true;
            _margin = new UIRect(2, 2, 2, 2);
            _bounds = new UIRect((int)x, (int)y, (int)(x + w), (int)(y + h));
            _background = new Panel(x, y, w + (_margin.Size.X * 2), h + (_margin.Size.Y * 2), Color4.Gray);
            _caption = new Label(x + _margin.Position.X, y + _margin.Position.Y, 
                                w - _margin.Size.X, h - _margin.Size.Y, text, font);
        }

        public void ProcessMouseDown(MouseButtonEventArgs e, Vector2 mousePos)
        {
            if(e.Button == OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left)
            {
                if(mousePos.X > _bounds.Position.X && mousePos.X < _bounds.Size.X &&
                    mousePos.Y > _bounds.Position.Y && mousePos.Y < _bounds.Size.Y)
                    {
                        Console.WriteLine("Clicked from inside button");
                        if(OnClicked is not null)
                            OnClicked();
                    }
            }
        }

        public void Draw(ref Shader sh)
        {
            _background.Draw(ref sh);
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