/*
 * [x] Implement FontRenderer to draw text.
 */
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using LGL.Loaders;
using OpenTK.Windowing.GraphicsLibraryFramework;
using LGL.Gfx;

namespace Camera
{
    public class DemoCamera : GameWindow
    {
        private Vector3 _camPosition;
        private Vector3 _camTarget;
        private Quaternion _camRotation;
        private Vector3 _camRight;
        private Vector3 _camUp;
        private Matrix4 _model;

        private Matrix4 _view;
        private Matrix4 _projection;

        private int vao;
        private int vbo;

        private Shader shader;
        private FontRenderer fontRenderer;

        public DemoCamera(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Enable(EnableCap.DepthTest);

            BuildCube();

            shader = Shader.Load("simple", 2, "Assets/Shaders/simple.vert", "Assets/Shaders/simple.frag");
            shader.SetupUniforms(new string[] { "viewproj", "model" });

            _model = Matrix4.Identity;
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)ClientSize.X / (float)ClientSize.Y, 0.1f, 1000.0f);

            CreateCamera(new Vector3(0.0f, 10.0f, 24.0f), new Vector3(0.0f, 0.0f, -1.0f));

            fontRenderer = new FontRenderer(ClientSize.X, ClientSize.Y);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            fontRenderer.Dispose();

            GL.DeleteBuffer(vbo);
            GL.DeleteVertexArray(vao);
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
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 vp = _view * _projection;
            shader.Use();
            shader.UploadMatrix("viewproj", ref vp);
            shader.UploadMatrix("model", ref _model);
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.BindVertexArray(0);


            fontRenderer.DrawText($"Cam pos: {_camPosition}", new Vector2(0.0f, 20.0f), 14.0f);
            fontRenderer.EndRender();


            SwapBuffers();
        }

        private void BuildCube()
        {
            float[] verts =
            {
                -1.5f, -1.5f, -1.5f,  1.0f, 0.0f, 0.0f,
                 1.5f, -1.5f, -1.5f,  1.0f, 0.0f, 0.0f,
                 1.5f,  1.5f, -1.5f,  1.0f, 0.0f, 0.0f,
                 1.5f,  1.5f, -1.5f,  1.0f, 0.0f, 0.0f,
                -1.5f,  1.5f, -1.5f,  1.0f, 0.0f, 0.0f,
                -1.5f, -1.5f, -1.5f,  1.0f, 0.0f, 0.0f,
                 
                -1.5f, -1.5f,  1.5f,  1.0f, 1.0f, 0.0f,
                 1.5f, -1.5f,  1.5f,  1.0f, 1.0f, 0.0f,
                 1.5f,  1.5f,  1.5f,  1.0f, 1.0f, 0.0f,
                 1.5f,  1.5f,  1.5f,  1.0f, 1.0f, 0.0f,
                -1.5f,  1.5f,  1.5f,  1.0f, 1.0f, 0.0f,
                -1.5f, -1.5f,  1.5f,  1.0f, 1.0f, 0.0f,
                 
                -1.5f,  1.5f,  1.5f,  0.0f, 1.0f, 0.0f,
                -1.5f,  1.5f, -1.5f,  0.0f, 1.0f, 0.0f,
                -1.5f, -1.5f, -1.5f,  0.0f, 1.0f, 0.0f,
                -1.5f, -1.5f, -1.5f,  0.0f, 1.0f, 0.0f,
                -1.5f, -1.5f,  1.5f,  0.0f, 1.0f, 0.0f,
                -1.5f,  1.5f,  1.5f,  0.0f, 1.0f, 0.0f,
                 
                 1.5f,  1.5f,  1.5f,  0.0f, 1.0f, 1.0f,
                 1.5f,  1.5f, -1.5f,  0.0f, 1.0f, 1.0f,
                 1.5f, -1.5f, -1.5f,  0.0f, 1.0f, 1.0f,
                 1.5f, -1.5f, -1.5f,  0.0f, 1.0f, 1.0f,
                 1.5f, -1.5f,  1.5f,  0.0f, 1.0f, 1.0f,
                 1.5f,  1.5f,  1.5f,  0.0f, 1.0f, 1.0f,
                 
                -1.5f, -1.5f, -1.5f,  0.0f, 0.0f, 1.0f,
                 1.5f, -1.5f, -1.5f,  0.0f, 0.0f, 1.0f,
                 1.5f, -1.5f,  1.5f,  0.0f, 0.0f, 1.0f,
                 1.5f, -1.5f,  1.5f,  0.0f, 0.0f, 1.0f,
                -1.5f, -1.5f,  1.5f,  0.0f, 0.0f, 1.0f,
                -1.5f, -1.5f, -1.5f,  0.0f, 0.0f, 1.0f,
                 
                -1.5f,  1.5f, -1.5f,  1.0f, 0.0f, 1.0f,
                 1.5f,  1.5f, -1.5f,  1.0f, 0.0f, 1.0f,
                 1.5f,  1.5f,  1.5f,  1.0f, 0.0f, 1.0f,
                 1.5f,  1.5f,  1.5f,  1.0f, 0.0f, 1.0f,
                -1.5f,  1.5f,  1.5f,  1.0f, 0.0f, 1.0f,
                -1.5f,  1.5f, -1.5f,  1.0f, 0.0f, 1.0f
            };

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * sizeof(float), verts, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
        }

        private void CreateCamera(Vector3 pos, Vector3 target)
        {
            _camPosition = pos;
            _camTarget = target;

            
            _view = Matrix4.LookAt(pos, target, Vector3.UnitY);
        }
    }
}
