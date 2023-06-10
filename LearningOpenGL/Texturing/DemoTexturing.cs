using LGL.Gfx;
using LGL.Loaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using static LGL.LGLState;

namespace Texturing
{
    public class DemoTexturing : GameWindow
    {
        private Matrix4 _ortho;
        private Matrix4 _eye;
        private Matrix4 _model;
        
        private Texture _texture;
        private Texture _palleteTexture;
        
        private Shader _textureShader;

        private VertexArrayObject vao;
        private VertexBuffer vbo;

        public DemoTexturing(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            VSync = VSyncMode.On;

            InitState(this);
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            
            _model = Matrix4.Identity;
            _eye = Matrix4.LookAt(Vector3.Zero, new Vector3(0.0f, 0.0f, -1.0f), Vector3.UnitY);
            _ortho = Matrix4.CreateOrthographicOffCenter(0.0f, (float)ClientSize.X, 0.0f, (float)ClientSize.Y, 0.1f, 10.0f);

            _texture = Texture.Load("Assets/Textures/wall.jpg");
            _palleteTexture = Texture.Load("Assets/Textures/pal1.png");

            _textureShader = Shader.Load("Texturing", 2, "Assets/Shaders/texturing.vert",
                                        "Assets/Shaders/texturing.frag", false);
            _textureShader.SetupUniforms(new string[] { "model", "view", "ortho" });

            int texIndex = 100;
            float u = (1.0f / (float)_palleteTexture.Width) * (float)texIndex;
            float u1 = u + (1.0f / (float)_palleteTexture.Width);

            VertexColorTexture[] verts = 
            {
                new VertexColorTexture(new Vector3(100.0f, 100.0f, -1.0f), new Color4(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0.0f, 0.0f)),
                new VertexColorTexture(new Vector3(400.0f, 100.0f, -1.0f), new Color4(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(1.0f, 0.0f)),
                new VertexColorTexture(new Vector3(100.0f, 600.0f, -1.0f), new Color4(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0.0f, 1.0f)),
                new VertexColorTexture(new Vector3(100.0f, 600.0f, -1.0f), new Color4(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0.0f, 1.0f)),
                new VertexColorTexture(new Vector3(400.0f, 100.0f, -1.0f), new Color4(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(1.0f, 0.0f)),
                new VertexColorTexture(new Vector3(400.0f, 600.0f, -1.0f), new Color4(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(1.0f, 1.0f)),

                new VertexColorTexture(new Vector3(600.0f, 100.0f, -1.0f), new Color4(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(u, 0.0f)),
                new VertexColorTexture(new Vector3(1200.0f, 100.0f, -1.0f), new Color4(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(u, 0.0f)),
                new VertexColorTexture(new Vector3(600.0f, 600.0f, -1.0f), new Color4(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(u, 1.0f)),
                new VertexColorTexture(new Vector3(600.0f, 600.0f, -1.0f), new Color4(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(u, 1.0f)),
                new VertexColorTexture(new Vector3(1200.0f, 100.0f, -1.0f), new Color4(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(u, 0.0f)),
                new VertexColorTexture(new Vector3(1200.0f, 600.0f, -1.0f), new Color4(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(u, 1.0f)),
            };

            vao = new VertexArrayObject(VertexColorTexture.SizeInBytes);
            vao.Enable();
            vao.PrimitiveCount = verts.Length;

            vbo = new VertexBuffer(BufferTarget.ArrayBuffer);
            vbo.Enable();
            vbo.Data<VertexColorTexture>(BufferUsageHint.StaticDraw, verts, VertexColorTexture.SizeInBytes);

            vao.Attributes(new VertexAttribute[]
            {
                new VertexAttribute(0, 3, 0),
                new VertexAttribute(1, 3, Vector3.SizeInBytes),
                new VertexAttribute(2, 2, Vector3.SizeInBytes * 2),
            }, VertexAttribPointerType.Float);
            vao.Disable();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (IsKeyReleased(Keys.Escape))
                Close();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            BeginDraw();

            BeginDraw2D();

            _textureShader.Enable();
            _textureShader.UploadMatrix("model", ref _model);
            _textureShader.UploadMatrix("view", ref _eye);
            _textureShader.UploadMatrix("ortho", ref _ortho);
            _texture.Bind();
            
            vao.Draw(PrimitiveType.Triangles, 0);
            _palleteTexture.Bind();
            vao.Draw(PrimitiveType.Triangles, 6);

            EndDraw2D();
            EndDraw(this);
        }

        protected override void OnUnload()
        {
            _palleteTexture.Dispose();
            _texture.Dispose();
            _textureShader.Dispose();
            
            vbo.Dispose();
            vao.Dispose();
            
            base.OnUnload();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            Viewport(0, 0, ClientSize.X, ClientSize.Y);
        }
    }
}
