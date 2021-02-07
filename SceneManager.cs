using System;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using HabiCS.Scenes;
using HabiCS.Loaders;
using HabiCS.UI;
using HabiCS.Graphics;

namespace HabiCS
{
    public class SceneManager: IDisposable
    {
        private Game game;

        private Scene currentScene;
        private UIScreen currentScreen;
        private Font font;
        private Shader uiShader;
        private Matrix4 scale;
        private Matrix4 ortho;
        private Matrix4 projection;

        public Font Font { 
            get { return font; }
        }

        public UIScreen CurrentScreen {
            get { return currentScreen; }
        }

        public Matrix4 Ortho { 
            get { return ortho; }
        }

        public Matrix4 Projection {
            get { return projection; }
        }

        public SceneManager(Game g)
        {
            game = g;
            currentScene = null;
            currentScreen = null;
        }

        public void Load()
        {
            //UI resources
            font = Font.Load("Assets/Fonts/font.json", game.ClientSize.X, game.ClientSize.Y);
            uiShader = Shader.Load("UI", 2, "Assets/Shaders/ui.vert", "Assets/Shaders/ui.frag");
            uiShader.SetupUniforms(new string[]{"ortho", "model", "color", "text"});
            scale = Matrix4.CreateScale(0.6f, 0.6f, 1.0f);
            ortho = Matrix4.CreateOrthographicOffCenter(0.0f, (float)game.ClientSize.X, 0.0f, (float)game.ClientSize.Y, 0.1f, 1.0f);
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), game.ClientSize.X / game.ClientSize.Y, 0.1f, 1000.0f);
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
            if(currentScene is not null)
                currentScene.Render(time);
        }
    
        private void Render2D(double time)
        {
            if(currentScreen is null)
                return;
            
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            
            //2d
            if(currentScene is not null)
                currentScene.Render(time);
            
            //ui
            uiShader.Use();
            uiShader.UploadMatrix("ortho", ref ortho);
            uiShader.UploadMatrix("model", ref scale);
            Font.Bind();
            currentScreen.Draw(ref uiShader);
            Font.Unbind();

            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
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

                    font.Dispose();
                    uiShader.Dispose();
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