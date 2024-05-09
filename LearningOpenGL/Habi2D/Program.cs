using Habi;

GameOptions options = GameOptions.DefaultOGL;
options.Title = "Basic Window";

using (Game game = new Game(options))
{
    game.Start();
}
