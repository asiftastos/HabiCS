using Silk.NET.Windowing;
using Silk.NET.Maths;
using Silk.NET.Input;

namespace HabiWindow
{
    public class Habi : IDisposable
    {
        private IWindow window;

        private IInputContext inputContext;

        public IWindow MainWindow => window;

        public IInputContext Input => inputContext;

        public Habi(HabiOptions options)
        {
            WindowOptions woptions = WindowOptions.Default;
            woptions.Title = options.title;
            woptions.Size = new Vector2D<int>(options.width, options.height);

            switch (options.gfx)
            {
                case GFX.OpenGL:
                    woptions.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.ForwardCompatible, new APIVersion(4, 3));
                    break;
                case GFX.Vulkan:
                    woptions.API = new GraphicsAPI(ContextAPI.Vulkan, ContextProfile.Core, ContextFlags.ForwardCompatible, new APIVersion(1,3));
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

            window.Load += Window_Load;
        }

        private void Window_Load()
        {
            window.Center();
            inputContext = window.CreateInput();
        }

        public void Dispose()
        {
            inputContext.Dispose();
            window.Dispose();
        }

        public void Run()
        {
            window.Run();
        }
    }
}