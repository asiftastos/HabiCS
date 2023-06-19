using HabiGraphics.OpenGL;
using HabiWindow;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace Habi2D
{
    public class Game : IDisposable
    {
        private Habi _habi;

        private VertexArray vao;
        private VertexBuffer vbo;

        public Game(HabiOptions options)
        {
            _habi = new Habi(options);

            _habi.MainWindow.Load += HabiOnLoad;
            _habi.MainWindow.FramebufferResize += HabiOnFramebufferResize;
            _habi.MainWindow.Render += HabiOnRender;
            _habi.MainWindow.Update += HabiOnUpdate;
        }

        private void HabiOnUpdate(double obj)
        {
        }

        private void HabiOnRender(double obj)
        {
            HabiGL.Begin();

            HabiGL.End();
        }

        private void HabiOnFramebufferResize(Vector2D<int> obj)
        {
            HabiGL.Viewport(obj);
        }

        private void HabiOnLoad()
        {
            _habi.Input.Keyboards[0].KeyDown += HabiOnKeyDown;

            if (_habi.MainWindow.GLContext is not null)
            {
                HabiGL.Init(_habi.MainWindow.GLContext);
            }

            vao = HabiGL.CreateVertexArray();
            vao.Enable();

            unsafe
            {
                vbo = HabiGL.CreateStaticArrayBuffer(0, null);
            }

            HabiGL.ResetVertexArray();
        }

        private void HabiOnKeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            if (arg2 == Key.Escape)
            {
                _habi.MainWindow.Close();
            }
        }

        public void Dispose()
        {
            vbo.Dispose();
            vao.Dispose();
            _habi.Dispose();
        }

        public void Start()
        {
            _habi.Run();
        }
    }
}
