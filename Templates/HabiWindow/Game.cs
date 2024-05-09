using HabiWindow;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace Habi
{
    public class Game : IDisposable
    {
        private IWindow _window;

        private IInputContext _inputContext;

        private GameOptions _gameOptions;

        public Game(GameOptions gameoptions)
        {
            _gameOptions = gameoptions;

            WindowOptions windowoptions = new WindowOptions();
            windowoptions.Title = _gameOptions.Title;
            windowoptions.Size = new Vector2D<int>(_gameOptions.Width, _gameOptions.Height);
            windowoptions.IsVisible = true;

            switch (_gameOptions.GfxApi)
            {
                case GFX.OpenGL:
                    windowoptions.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.ForwardCompatible, new APIVersion(4, 3));
                    break;
                case GFX.Vulkan:
                    windowoptions.API = new GraphicsAPI(ContextAPI.Vulkan, ContextProfile.Core, ContextFlags.ForwardCompatible, new APIVersion(1, 3));
                    break;
                case GFX.NoApi:
                case GFX.DirectX12:
                case GFX.WebGPU:
                    windowoptions.API = GraphicsAPI.None;
                    break;
                default:
                    windowoptions.API = GraphicsAPI.None;
                    break;
            }

            _window = Window.Create(windowoptions);

            Console.WriteLine($"Created window: {gameoptions.ToString()}");

            _window.Load += HabiOnLoad;
            _window.Closing += HabiOnClosing;
            _window.FramebufferResize += HabiOnFramebufferResize;
            _window.Render += HabiOnRender;
            _window.Update += HabiOnUpdate;
        }

        private void HabiOnClosing()
        {
            Console.WriteLine("Closing Game....");
        }

        private void HabiOnUpdate(double obj)
        {
        }

        private void HabiOnRender(double obj)
        {
        }

        private void HabiOnFramebufferResize(Vector2D<int> obj)
        {
        }

        private void HabiOnLoad()
        {
            Console.WriteLine("Loading game....");

            _window.Center();

            _inputContext = _window.CreateInput();
            Console.WriteLine($"Attached keyboards: {_inputContext.Keyboards.Count} x {_inputContext.Keyboards[0].Name}");
            Console.WriteLine($"Attached mouse: {_inputContext.Mice.Count} x {_inputContext.Mice[0].Name}");

            _inputContext.Keyboards[0].KeyDown += HabiOnKeyDown;
        }

        private void HabiOnKeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            if(arg2 == Key.Escape)
            {
                _window.Close();
            }
        }

        public void Dispose()
        {
            Console.WriteLine("Disposing resources....");

            _window.Dispose();
        }

        public void Start()
        {
            _window.Run();
        }
    }
}
