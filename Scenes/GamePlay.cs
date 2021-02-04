using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using  OpenTK.Windowing.GraphicsLibraryFramework;

namespace HabiCS.Scenes
{
    public class GamePlay: Scene
    {
        public GamePlay(Game g):base("GamePlay", g)
        {
        }

        public override void ProcessKeyInput(KeyboardKeyEventArgs e)
        {
            base.ProcessKeyInput(e);
            if(e.Key == Keys.D1)
                game.SceneManager.ChangeScene(new Start(game));
        }

        public override void Load()
        {
            base.Load();
            GL.ClearColor(Color4.Black);
        }
    }
}