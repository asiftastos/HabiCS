using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace CameraDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GameWindowSettings gameWinSettings = new GameWindowSettings();
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                Title = "Camera Demo",
                APIVersion = new System.Version(4, 5),
                API = ContextAPI.OpenGL,
                Flags = ContextFlags.ForwardCompatible,
                Profile = ContextProfile.Core
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

            nativeWindowSettings.Size = new Vector2i(width, height);
            nativeWindowSettings.Location = new Vector2i(xpos, ypos);

            using (var game = new DemoCamera(gameWinSettings, nativeWindowSettings))
            {
                game.Run();
            }
        }
    }
}
