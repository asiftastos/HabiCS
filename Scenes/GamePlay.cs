using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using HabiCS.UI;

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
            {
                var scene = new Start(game);
                game.SceneManager.ChangeScene(scene);
                Label l = (Label)game.SceneManager.CurrentScreen.GetElem("Name");
                if(l is not null)
                    l.Text = scene.Name;
            }
        }

        public override void Load()
        {
            base.Load();
            GL.ClearColor(Color4.White);
        }
    }
}