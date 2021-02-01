using System;
using System.Collections.Generic;
using OpenTK.Windowing.Common;
using HabiCS.Loaders;

namespace HabiCS.UI
{
    public class UIScreen: IDisposable
    {
        private UIManager uiManager;

        public List<IUIElem> Elements { get; set; }

        public UIScreen(UIManager ui)
        {
            uiManager = ui;
            Elements = new List<IUIElem>();
        }

        public virtual void Load()
        {
        }

        public virtual void Draw(ref Shader sh)
        {
            foreach (var elem in Elements)
            {
                elem.Draw(ref sh);
            }
        }

        public void OnMouseDown(MouseButtonEventArgs e)
        {
            foreach (var item in Elements)
            {
                if(item.Inderactable)
                    item.ProcessMouseDown(e);
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
                    foreach (var item in Elements)
                    {
                        item.Dispose();
                    }
                    Elements.Clear();
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