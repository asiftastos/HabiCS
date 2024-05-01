using Habi.Graphics.OpenGL;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Habi
{
    public class Game : IDisposable
    {
        private IWindow _window;

        private IInputContext _inputContext;

        private GFX _gFX;

        private GL gl;

        private ShaderProgram _program;
        private VertexArrayObject vao;
        private VertexBufferObject vbo;

        private Matrix4X4<float> _model;
        private Matrix4X4<float> _view;
        private Matrix4X4<float> _proj;

        private Vector4D<float> _color;

        //private Silk.NET.SDL.Color tint;

        public Game(string title, int width = 800, int height = 600, GFX fx = GFX.NoApi)
        {
            _gFX = fx;

            WindowOptions options = new WindowOptions();
            options.Title = title;
            options.Size = new Vector2D<int>(width, height);
            options.IsVisible = true;

            switch (_gFX)
            {
                case GFX.OpenGL:
                    options.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.ForwardCompatible, new APIVersion(4, 3));
                    break;
                case GFX.Vulkan:
                    options.API = new GraphicsAPI(ContextAPI.Vulkan, ContextProfile.Core, ContextFlags.ForwardCompatible, new APIVersion(1, 3));
                    break;
                case GFX.NoApi:
                case GFX.DirectX12:
                case GFX.WebGPU:
                    options.API = GraphicsAPI.None;
                    break;
                default:
                    options.API = GraphicsAPI.None;
                    break;
            }

            _window = Window.Create(options);

            _window.Load += HabiOnLoad;
            _window.Closing += HabiOnClosing;
            _window.FramebufferResize += HabiOnFramebufferResize;
            _window.Render += HabiOnRender;
            _window.Update += HabiOnUpdate;
        }

        private void HabiOnClosing()
        {
            HabiGL.ResetArrayBuffer();
            vbo.Dispose();

            HabiGL.ResetShader();
            _program.Dispose();

            HabiGL.ResetArrayBuffer();
            vao.Dispose();
        }

        private void HabiOnUpdate(double obj)
        {
        }

        private void HabiOnRender(double obj)
        {
            HabiGL.Begin();

            _program.Enable();
            _program.UploadVector4("color", _color);
            _program.UploadMatrix("model", _model);
            _program.UploadMatrix("viewproj", _proj);

            vao.Enable();
            vao.Draw(PrimitiveType.Triangles, 0);

            _window.SwapBuffers();
        }

        private void HabiOnFramebufferResize(Vector2D<int> obj)
        {
        }

        private void HabiOnLoad()
        {
            _window.Center();

            _inputContext = _window.CreateInput();
            Console.WriteLine($"Attached keyboards: {_inputContext.Keyboards.Count}");

            _inputContext.Keyboards[0].KeyDown += HabiOnKeyDown;

            gl = GL.GetApi(_window);

            HabiGL.Init(gl);

            HabiGL.Depth(true);

            _program = HabiGL.CreateShaderFromFile("Assets/Shaders/color.vert", 
                                                    "Assets/Shaders/color.frag",
                                                    new string[] { "viewproj", "model", "color" });

            float[] verts =
            {
                10.0f, 10.0f, 0.0f, 1.0f, 0.0f, 0.0f,
                200.0f, 10.0f, 0.0f, 0.0f, 0.0f, 1.0f,
                100.0f, 100.0f, 0.0f, 0.0f, 1.0f, 0.0f
            };

            vao = HabiGL.CreateVertexArray();
            vao.Enable();
            unsafe
            {
                fixed (float* p = verts)
                vbo = HabiGL.CreateStaticArrayBuffer(verts.Length * sizeof(float), p);
            }
            vao.Attributes(new VertexArrayObject.VertexAttribute[]
            {
                new VertexArrayObject.VertexAttribute(0, 3, 6 * sizeof(float), 0),
                new VertexArrayObject.VertexAttribute(1, 3, 6 * sizeof(float), 3 * sizeof(float)),
            });

            HabiGL.ResetArrayBuffer();
            HabiGL.ResetVertexArray();

            vao.PrimitiveCount = 3;

            _model = Matrix4X4<float>.Identity;
            _view = Matrix4X4<float>.Identity;
            _proj = Matrix4X4.CreateOrthographicOffCenter(0f, (float)_window.Size.X, 0f, (float)_window.Size.Y, 0.01f, 1.0f);

            _color = new Vector4D<float>(1.0f);
        }

        private void HabiOnKeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            if (arg2 == Key.Escape)
            {
                _window.Close();
            }
        }

        public void Dispose()
        {
            _inputContext.Dispose();
            _window.Dispose();
        }

        public void Start()
        {
            _window.Run();
        }
    }
}
