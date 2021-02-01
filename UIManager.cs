using System;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using OpenTK.Input;
using HabiCS.Loaders;
using HabiCS.UI;

namespace HabiCS
{
    public class UIManager: IDisposable
    {
        private Game game;
        private Font font;

        private Shader uiShader;
        private Matrix4 scale;
        private Matrix4 ortho;
        private UIScreen currentScreen;

        public Font Font { 
            get { return font; }
        }

        public UIManager(Game g)
        {
            game = g;
            currentScreen = null;
            g.MouseDown += ProcessMouseButtonDown;
        }

        public void Load()
        {
            font = Font.Load("Assets/Fonts/font.json", game.ClientSize.X, game.ClientSize.Y);

            uiShader = Shader.Load("UI", 2, "Assets/Shaders/ui.vert", "Assets/Shaders/ui.frag");
            uiShader.SetupUniforms(new string[]{"ortho", "model", "color", "text"});

            ortho = Matrix4.CreateOrthographicOffCenter(0.0f, (float)game.ClientSize.X, 0.0f, (float)game.ClientSize.Y, 0.1f, 1.0f);
        }

        public void Render(double time)
        {
            if(currentScreen is null)
                return;
            
            uiShader.Use();
            uiShader.UploadMatrix("ortho", ref ortho);
            Font.Bind();
            currentScreen.Draw(ref uiShader);
            Font.Unbind();
        }

        public void ChangeScreen(UIScreen newScreen)
        {
            if(currentScreen is not null)
                currentScreen.Dispose();
            
            currentScreen = newScreen;
            currentScreen.Load();
        }

        private void ProcessMouseButtonDown(MouseButtonEventArgs e)
        {
            if(currentScreen is not null)
                currentScreen.OnMouseDown(e);
        }

        #region DISPOSABLE PATTERN

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
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