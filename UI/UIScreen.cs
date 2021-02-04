using System;
using System.Collections.Generic;
using OpenTK.Windowing.Common;
using HabiCS.Loaders;

namespace HabiCS.UI
{
    public class UIScreen: IDisposable
    {
        private Game game;

        public Dictionary<string, IUIElem> Elements { get; set; }

        public UIScreen(Game g)
        {
            game = g;
            Elements = new Dictionary<string, IUIElem>();
        }

        public IUIElem GetElem(string name)
        {
            if(Elements.ContainsKey(name))
                return Elements[name];
            return null;
        }
        
        public virtual void Load()
        {
        }

        public virtual void Draw(ref Shader sh)
        {
            foreach (var elem in Elements)
            {
                elem.Value.Draw(ref sh);
            }
        }

        public void OnMouseDown(MouseButtonEventArgs e)
        {
            foreach (var item in Elements)
            {
                if(item.Value.Inderactable)
                    item.Value.ProcessMouseDown(e);
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
                        item.Value.Dispose();
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