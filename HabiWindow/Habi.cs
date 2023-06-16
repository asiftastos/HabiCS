using Silk.NET.Windowing;
using Silk.NET.Maths;

namespace HabiWindow
{
    public class Habi : IDisposable
    {
        private IWindow window;

        public IWindow MainWindow => window;

        public Habi(HabiOptions options)
        {
            WindowOptions woptions = WindowOptions.Default;
            woptions.Title = options.title;
            woptions.Size = new Vector2D<int>(options.width, options.height);

            switch (options.gfx)
            {
                case GFX.OpenGL:
                    woptions.API = GraphicsAPI.Default;
                    break;
                case GFX.Vulkan:
                    woptions.API = GraphicsAPI.DefaultVulkan;
                    break;
                case GFX.None:
                case GFX.DirectX12:
                case GFX.WebGpu:
                    woptions.API = GraphicsAPI.None;
                    break;
                default:
                    woptions.API = GraphicsAPI.None;
                    break;
            }

            window = Window.Create(woptions);
        }

        public void Dispose()
        {
            window.Dispose();
        }

        public void Run()
        {
            window.Run();
        }
    }
}