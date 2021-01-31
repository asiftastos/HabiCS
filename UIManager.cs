using System;
using OpenTK.Mathematics;
using HabiCS.Loaders;
using HabiCS.UI;

namespace HabiCS
{
    public class UIManager: IDisposable
    {
        private Game game;
        private Font font;

        private Shader textShader;
        private Shader uiShader;
        private Matrix4 scale;
        private UIScreen currentScreen;

        public Font Font { 
            get { return font; }
        }

        public UIManager(Game g)
        {
            game = g;
        }

        public void Load()
        {
            font = Font.Load("Assets/Fonts/font.json", game.ClientSize.X, game.ClientSize.Y);

            uiShader = Shader.Load("UI", 2, "Assets/Shaders/ui.vert", "Assets/Shaders/ui.frag");
            uiShader.SetupUniforms(new string[]{"ortho", "model"});

            textShader = Shader.Load("Font", 2, "Assets/Shaders/font.vert", "Assets/Shaders/font.frag");
            textShader.SetupUniforms(new string[]{"projTrans", "model"});
        }

        public void Render(double time)
        {
            if(currentScreen is null)
                return;
            
            currentScreen.Draw();
        }

        public void ChangeScreen(UIScreen newScreen)
        {
            if(currentScreen is not null)
                currentScreen.Dispose();
            
            currentScreen = newScreen;
            currentScreen.Load();
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
                    textShader.Dispose();
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