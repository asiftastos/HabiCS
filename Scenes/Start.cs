using System;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using HabiCS.UI;

namespace HabiCS.Scenes
{
    public class Start: Scene
    {
        public Start(Game g): base("Start", g)
        {
        }

        public override void ProcessKeyInput(KeyboardKeyEventArgs e)
        {
            base.ProcessKeyInput(e);
            if(e.Key == Keys.D2)
            {
                var scene = new GamePlay(game);
                game.SceneManager.ChangeScene(scene);
                Label elem = (Label)game.SceneManager.CurrentScreen.GetElem("Name");
                if(elem is not null)
                    elem.Text = scene.Name;
            }
        }

        public override void Load()
        {
            base.Load();
            GL.ClearColor(Color4.Blue);

            if(game.SceneManager.CurrentScreen is null)
            {
                var screen = new UIScreen(game);
                screen.Elements.Add("Name", new Label(0.0f, 0.0f, 100.0f, 30.0f, Name, game.SceneManager.Font));
                Button b = new Button(0.0f, 50.0f, 100.0f, 50.0f, "Exit", game.SceneManager.Font);
                b.OnClicked = this.OnExitClicked;
                screen.Elements.Add("Exit", b);
                game.SceneManager.ChangeScreen(screen);
            }
        }

        public void OnExitClicked()
        {
            Console.WriteLine("Exit clicked");
        }
    }
}