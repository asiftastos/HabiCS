
using HabiWindow;
using OpenGLWindow;

HabiOptions options = new HabiOptions("Basic", 1024, 800, GFX.OpenGL);
using (Game game = new Game(options))
{
    game.Start();
}
