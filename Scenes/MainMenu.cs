using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using HabiCS.UI;

namespace HabiCS.Scenes
{
    public class MainMenu: Scene
    {
        private UIScreen _screen;

        public MainMenu(Game g):base("Main Menu", g)
        {
            _screen = new UIScreen(g.UIManager);
        }

        public override void Load()
        {
            base.Load();

            _screen.Elements.Add(new Panel(1.0f, 1.0f, 200.0f, 100.0f, Color4.Blue));
            _screen.Elements.Add(new Label(1.0f, 110.0f, 50.0f, game.UIManager.Font.Size, "Kostas", game.UIManager.Font));

            game.UIManager.ChangeScreen(_screen);
        }

        public override void Update(double time)
        {
            base.Update(time);
        }

        public override void Render(double time)
        {
            base.Render(time);
        }

        public override void ProcessKeyInput(KeyboardKeyEventArgs e)
        {
            base.ProcessKeyInput(e);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _screen.Dispose();
        }
    }
}