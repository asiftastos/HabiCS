
using BasicWindow;
using HabiWindow;

HabiOptions options = new HabiOptions("Basic");
using (Game game = new Game(options))
{
    game.Start();
}
