using HabiWindow;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace BasicWindow
{
    public class Game : IDisposable
    {
        private Habi _habi;

        public Game(HabiOptions options)
        {
            _habi = new Habi(options);

            _habi.MainWindow.Load += HabiOnLoad;
            _habi.MainWindow.FramebufferResize += HabiOnFramebufferResize;
            _habi.MainWindow.Render += HaboOnRender;
            _habi.MainWindow.Update += HabiOnUpdate;
        }

        private void HabiOnUpdate(double obj)
        {
        }

        private void HaboOnRender(double obj)
        {
        }

        private void HabiOnFramebufferResize(Vector2D<int> obj)
        {
        }

        private void HabiOnLoad()
        {
            _habi.Input.Keyboards[0].KeyDown += HabiOnKeyDown;
        }

        private void HabiOnKeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            if(arg2 == Key.Escape)
            {
                _habi.MainWindow.Close();
            }
        }

        public void Dispose()
        {
            _habi.Dispose();
        }

        public void Start()
        {
            _habi.Run();
        }
    }
}
