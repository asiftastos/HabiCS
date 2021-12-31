/*
 * [x] Implement FontRenderer to draw text.
 * [x] Implemet an orbit camera
 *      [ ] Fix camera movement (pan) to the right axis,now it's always in world's X and Z
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
        private float _camMoveSpeed = 3.5f;
        private float _camRotSpeed = 18.5f;
        private Vector3 _camPosition;
        private Vector3 _camTarget;
        private Quaternion _camRotation;
        private Vector3 _camRight;
        private Vector3 _camUp;
        private Matrix4 _view;

        private Matrix4 _projection;

        private Matrix4 _model;
        private int vao;
        private int vbo;

        private Shader shader;
        private FontRenderer fontRenderer;

        public DemoCamera(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            VSync = VSyncMode.On;
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

            if (IsKeyDown(Keys.W))
            {
                MoveCamera(new Vector3(0.0f, 0.0f, -1.0f), _camMoveSpeed * (float)args.Time);
            }
            if(IsKeyDown(Keys.S))
            {
                MoveCamera(new Vector3(0.0f, 0.0f, 1.0f), _camMoveSpeed * (float)args.Time);
            }
            if (IsKeyDown(Keys.D))
            {
                MoveCamera(new Vector3(1.0f, 0.0f, 0.0f), _camMoveSpeed * (float)args.Time);
            }
            if (IsKeyDown(Keys.A))
            {
                MoveCamera(new Vector3(-1.0f, 0.0f, 0.0f), _camMoveSpeed * (float)args.Time);
            }
            if (IsKeyDown(Keys.E))
            {
                RotateCamera(_camRotSpeed * (float)args.Time);
            }
            if (IsKeyDown(Keys.Q))
            {
                RotateCamera(-_camRotSpeed * (float)args.Time);
            }

            UpdateCamera();
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

            fontRenderer.DrawText($"Cam pos: {_camPosition}", new Vector2(0.0f, 62.0f), 14.0f);
            fontRenderer.DrawText($"Cam speed: {_camMoveSpeed}", new Vector2(0.0f, 20.0f), 14.0f);
            fontRenderer.EndRender();


            SwapBuffers();
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Keys.PageUp)
                _camMoveSpeed += 0.5f;
            if (e.Key == Keys.PageDown)
                _camMoveSpeed -= 0.5f;
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);
     
            if(e.Key == Keys.Escape)
                Close();
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

        private void UpdateCamera()
        {
            _view = Matrix4.LookAt(_camPosition, _camTarget, Vector3.UnitY);
        }

        private void MoveCamera(Vector3 direction, float factor)
        {
            Vector3 vfactor = direction * factor;
            _camPosition += vfactor;
            _camTarget += vfactor;
        }

        private void RotateCamera(float factor)
        {
            _camRotation = Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(factor));
            Matrix4 rot = Matrix4.CreateFromQuaternion(_camRotation);
            _camPosition = Vector3.TransformPosition((_camPosition - _camTarget) + _camTarget, rot);
        }
    }
}
