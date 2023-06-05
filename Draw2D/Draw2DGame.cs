using LGL.Gfx;
using LGL.Loaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Draw2D
{
    public class Draw2DGame : GameWindow
    {
        private VertexArrayObject _vArray;
        private VertexBuffer _vBuffer;
        private int _vCount;
        private Color4 _tint;

        private Shader _shader;
        private Pipeline _pipeline;

        private Matrix4 _model;
        private Matrix4 _projection;
        private bool fillMode;

        private Vector3 _position;


        public Draw2DGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            fillMode = true;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            GL.Enable(EnableCap.DepthTest);

            VertexColor[] vc =
            {
                new VertexColor(0.0f, 0.0f, -1.0f, 1.0f, 1.0f, 1.0f),
                new VertexColor(0.0f, 50.0f, -1.0f, 1.0f, 1.0f, 1.0f),
                new VertexColor(100.0f, 50.0f, -1.0f, 1.0f, 1.0f, 1.0f),
                new VertexColor(100.0f, 0.0f, -1.0f, 1.0f, 1.0f, 1.0f)
            };
            _vCount = vc.Length;

            _vArray = new VertexArrayObject(VertexColor.SizeInBytes);
            _vArray.Enable();
            _vBuffer = new VertexBuffer(BufferTarget.ArrayBuffer);
            _vBuffer.Enable();
            _vBuffer.Data<VertexColor>(BufferUsageHint.StaticDraw, vc, VertexColor.SizeInBytes);

            _vArray.Attributes(new VertexAttribute[]
            {
                new VertexAttribute(0, 3, 0),
                new VertexAttribute(1, 3, Vector3.SizeInBytes)
            }, VertexAttribPointerType.Float);

            _shader = new Shader("Color", 2);
            _shader.CompileVertexFromFile("Assets/Shaders/color.vert");
            _shader.CompileFragmentFromFile("Assets/Shaders/color.frag");
            _shader.CreateProgram(true);
            _shader.SetupUniforms(new string[] { "viewproj", "model", "color" });
            _pipeline = new Pipeline();
            _pipeline.Use(_shader, ProgramStageMask.VertexShaderBit | ProgramStageMask.FragmentShaderBit);

            _position = new Vector3(0.0f, 0.0f, 0.0f);

            _model = Matrix4.CreateTranslation(_position.X, _position.Y, 0.0f);
            _projection = Matrix4.CreateOrthographicOffCenter(0.0f, (float)ClientSize.X, 0.0f, (float)ClientSize.Y, 0.1f, 1.0f);

            _tint = new Color4(255, 255, 255, 255);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            SetPolygonMode();
            _pipeline.Enable();
            _shader.UploadMatrix("model", ref _model);
            _shader.UploadMatrix("viewproj", ref _projection);
            _shader.UploadColor("color", _tint);
            _vArray.Enable();
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, _vCount);

            SwapBuffers();
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _pipeline.Dispose();
            _shader.Dispose();
            _vBuffer.Dispose();
            _vArray.Dispose();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);

            if(e.Key == Keys.Escape)
                Close();

            if (e.Key == Keys.P)
                fillMode = !fillMode;
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if(e.Button == MouseButton.Left)
            {
                Vector3 newPos = new Vector3(MousePosition.X, ClientSize.Y - MousePosition.Y, 0.0f);
                _model = Matrix4.CreateTranslation(newPos);
            }
        }

        private void SetPolygonMode()
        {
            if (fillMode)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }
            else
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }
        }
    }
}
