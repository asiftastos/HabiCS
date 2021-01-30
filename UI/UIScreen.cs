using System;

namespace HabiCS.UI
{
    public class UIScreen: IDisposable
    {
        private UIManager uiManager;

        public UIScreen(UIManager ui)
        {
            uiManager = ui;
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