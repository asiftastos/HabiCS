using System;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using HabiCS.Scenes;
using LGL.Loaders;
using HabiCS.UI;
using HabiCS.Graphics;

namespace HabiCS
{
    public class SceneManager: IDisposable
    {
        private Game game;

        private Scene currentScene;
        private UIScreen currentScreen;

        public UIScreen CurrentScreen {
            get { return currentScreen; }
        }

        public SceneManager(Game g)
        {
            game = g;
            currentScene = null;
            currentScreen = null;
        }

        public void Load()
        {
        }

        public void Update(double time)
        {
            if(currentScene is not null)
                currentScene.Update(time);
        }

        public void Render(double time)
        {
            switch (game.RenderPass)
            {
                case RenderPass.PASS3D:
                    Render3D(time);
                    break;
                case RenderPass.PASS2D:
                    Render2D(time);
                    break;
                default:
                    return;
            }
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

        public void ChangeScreen(UIScreen newScreen)
        {
            if(currentScreen is not null)
                currentScreen.Dispose();
            
            currentScreen = newScreen;
            currentScreen.Load();
        }

        public void ProcessKeyInput(KeyboardKeyEventArgs e)
        {
            if(currentScene is not null)
                currentScene.ProcessKeyInput(e);
        }

        public void ProcessMouseButtonDown(MouseButtonEventArgs e)
        {
            if(currentScreen is not null)
            {
                Vector2 invertedMPos = new Vector2(game.MousePosition.X, game.ClientSize.Y - game.MousePosition.Y);
                //We scale (multiply) vertices so divide mouse pos with the scale factor
                // NOTE: Need absolute int,divide returns floats
                currentScreen.OnMouseDown(e, Vector2.Divide(invertedMPos, 0.6f));
            }
            if(currentScene is not null)
            {
                currentScene.ProcessMouseDown(e);
            }
        }

        private void Render3D(double time)
        {
            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            if (currentScene is not null)
                currentScene.Render(time);
        }
    
        private void Render2D(double time)
        {
            if(currentScreen is null)
                return;
            
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            
            //2d
            if(currentScene is not null)
                currentScene.Render(time);
            
            //ui
            currentScreen.Render(time);
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
                    
                    if(currentScreen is not null)
                        currentScreen.Dispose();
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