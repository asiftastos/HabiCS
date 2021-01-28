using System;
using OpenTK.Windowing.Common;
using HabiCS.Scenes;

namespace HabiCS
{
    public class SceneManager: IDisposable
    {
        private Game game;

        private Scene currentScene;


        public SceneManager(Game g)
        {
            game = g;
            currentScene = null;
        }

        public void Update(double time)
        {
            if(currentScene != null)
                currentScene.Update(time);
        }

        public void Render(double time)
        {
            if(currentScene != null)
                currentScene.Render(time);
        }

        public void ChangeScene(Scene newScene)
        {
            if(currentScene != null)
            {
                currentScene.Dispose();
            }
            
            currentScene = newScene;
            currentScene.Load();
        }

        public void ProcessKeyInput(KeyboardKeyEventArgs e)
        {
            if(currentScene != null)
                currentScene.ProcessInput(e);
        }

        #region DISPOSABLE PATTERN

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(currentScene != null)
                        currentScene.Dispose();
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