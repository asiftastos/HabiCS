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
            if(currentScene is not null)
                currentScene.Update(time);
        }

        public void Render(double time)
        {
            if(currentScene is not null)
                currentScene.Render(time);
        }

        public void ChangeScene(Scene newScene)
        {
            if(currentScene is not null)
            {
                currentScene.Dispose();
            }
            
            currentScene = newScene;
            currentScene.Load();
        }

        public void ProcessKeyInput(KeyboardKeyEventArgs e)
        {
            if(currentScene != null)
                currentScene.ProcessKeyInput(e);
        }

        #region DISPOSABLE PATTERN

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(currentScene is not null)
                        currentScene.Dispose();
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