using System;
using System.Collections.Generic;
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
            if(Elements.Count > 0)
            {
                foreach (var elem in Elements)
                {
                    elem.Draw(ref sh);
                }
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