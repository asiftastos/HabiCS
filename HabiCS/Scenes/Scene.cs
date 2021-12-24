using System;
using OpenTK.Windowing.Common;

namespace HabiCS.Scenes
{
    public class Scene : IDisposable
    {
        protected Game game;

        public string Name { get; set; }

        public Scene(string name, Game g)
        {
            Name = name;
            game = g;
        }

        public virtual void Load()
        {

        }

        public virtual void Update(double time)
        {

        }

        public virtual void Render(double time)
        {
        }

        public virtual void ProcessKeyInput(KeyboardKeyEventArgs e)
        {
        }

        public virtual void ProcessMouseDown(MouseButtonEventArgs e)
        {
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
