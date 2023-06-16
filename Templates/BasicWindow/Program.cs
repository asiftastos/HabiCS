
using HabiWindow;

HabiOptions options = new HabiOptions("Basic");
using (Habi game = new Habi(options))
{
    game.MainWindow.Load += () =>
    {
        game.Input.Keyboards[0].KeyDown += (arg1, arg2, arg3) => { 
            if(arg2 == Silk.NET.Input.Key.Escape)
            {
                game.MainWindow.Close();
            }
        };
    };

    game.Run();
}
