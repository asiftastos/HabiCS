using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;

namespace Draw3D
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GameWindowSettings gameWinSettings = new GameWindowSettings();
            var nativeWindowSettings = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(800, 600),
                Title = "Habi",
                APIVersion = new System.Version(4, 5),
                API = ContextAPI.OpenGL,
                Flags = ContextFlags.ForwardCompatible,
                Profile = ContextProfile.Core,
            };

            int width, height, xpos, ypos;

            unsafe
            {
                var monitor = GLFW.GetPrimaryMonitor();
                var vidmode = GLFW.GetVideoMode(monitor);
                width = vidmode->Width - 200;
                height = vidmode->Height - 100;
                xpos = (vidmode->Width - width) / 2;
                ypos = (vidmode->Height - height) / 2;
            }

            nativeWindowSettings.ClientSize = new Vector2i(width, height);
            nativeWindowSettings.Location = new Vector2i(xpos, ypos);

            using(var game = new Draw3DGame(gameWinSettings, nativeWindowSettings))
            {
                game.Run();
            }
        }
    }
}
