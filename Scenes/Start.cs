using System;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

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
                game.SceneManager.ChangeScene(new GamePlay(game));
        }

        public override void Load()
        {
            base.Load();
            GL.ClearColor(Color4.Blue);
        }
    }
}