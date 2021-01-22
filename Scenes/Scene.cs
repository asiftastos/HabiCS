﻿using System;
using OpenTK.Windowing.Common;

namespace HabiCS.Scenes
{
    class Scene : IDisposable
    {
        public string Name { get; set; }

        public Scene(string name)
        {
            Name = name;
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

        public virtual void ProcessInput(KeyboardKeyEventArgs e)
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
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Scene()
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
