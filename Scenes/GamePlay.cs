using System.Collections.Generic;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using HabiCS.UI;
using HabiCS.Loaders;
using HabiCS.Graphics;

namespace HabiCS.Scenes
{
    public class GamePlay: Scene
    {
        private Matrix4 _ortho;

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

        public override void ProcessMouseDown(MouseButtonEventArgs e)
        {
            base.ProcessMouseDown(e);
        }

        public override void Load()
        {
            base.Load();
            GL.ClearColor(Color4.Black);
            _ortho = game.SceneManager.Ortho;
        }

        public override void Update(double time)
        {
            base.Update(time);
        }

        public override void Render(double time)
        {
            base.Render(time);
            if(game.RenderPass == Graphics.RenderPass.PASS3D)
                return;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}