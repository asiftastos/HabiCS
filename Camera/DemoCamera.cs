using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using LGL.Loaders;
using OpenTK.Windowing.GraphicsLibraryFramework;
using LGL.Gfx;
using System;
using LGL.Utilities;

namespace CameraDemo
{
    public class DemoCamera : GameWindow
    {
        private Camera _camera;

        private Matrix4 _model;
        private int vao;
        private int vbo;

        private float _debugYaw;
        private bool _debugCam;
        private Matrix4 _debugModel;
        private Matrix4 _debugView;
        private int _debugVao;
        private int _debugVbo;
        private int _debugCamCoordsVao;
        private int _debugCamCoords;
        private Vector3 _camDebugPosition;
        private Vector3 _camDebugTarget;

        private Shader shader;
        private FontRenderer fontRenderer;

        public DemoCamera(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            VSync = VSyncMode.On;
            _debugCam = false;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Enable(EnableCap.DepthTest);

            BuildCube();

            shader = Shader.Load("simple", 2, "Assets/Shaders/color.vert", "Assets/Shaders/color.frag", false);
            shader.SetupUniforms(new string[] { "viewproj", "model", "color" });

            _model = Matrix4.Identity;
            
            _camera = new Camera(new Vector3(0.0f, 10.0f, 24.0f), Vector3.Zero, Vector3.UnitY);
            _camera.Mode = Camera.CameraMode.FREE;

            BuildDebug();
            CreateDebugCamera();

            fontRenderer = new FontRenderer(ClientSize.X, ClientSize.Y);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            fontRenderer.Dispose();

            GL.DeleteBuffer(vbo);
            GL.DeleteVertexArray(vao);

            GL.DeleteBuffer(_debugVbo);
            GL.DeleteBuffer(_debugCamCoords);
            GL.DeleteVertexArray(_debugVao);
            GL.DeleteVertexArray(_debugCamCoordsVao);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            
            _camera.Update(this);

            UpdateDebug();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 vp = (_debugCam ? _debugView : _camera.View) * _camera.Projection((float)ClientSize.X / (float)ClientSize.Y);
            //Matrix4 vp = _camera.View * _camera.Projection((float)ClientSize.X / (float)ClientSize.Y);

            shader.Use();
            shader.UploadMatrix("viewproj", ref vp);
            shader.UploadMatrix("model", ref _model);
            shader.UploadColor("color", Color4.White);
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.BindVertexArray(0);

            if(_debugCam)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                shader.Use();
                shader.UploadMatrix("viewproj", ref vp);
                shader.UploadMatrix("model", ref _debugModel);
                shader.UploadColor("color", Color4.White);
                GL.BindVertexArray(_debugVao);
                GL.BindBuffer(BufferTarget.ArrayBuffer, _debugVbo);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.LineWidth(4.0f);
                GL.BindVertexArray(_debugCamCoordsVao);
                GL.BindBuffer(BufferTarget.ArrayBuffer, _debugCamCoords);
            
                Vector3 forward = _camera.Forward;
                Vector3 direction = Vector3.Normalize(_camera.Position - _camera.Target);
                Vector3 right = _camera.Right;
                float[] v =
                {
                    0.0f,   0.0f,   0.0f,                   0.0f, 0.0f, 1.0f,
                    forward.X, forward.Y, forward.Z,  0.0f, 0.0f, 1.0f,
                    0.0f,   0.0f,   0.0f,                                   1.0f, 0.0f, 0.0f,
                    _camera.Up.X, _camera.Up.Y, _camera.Up.Z,                           1.0f, 0.0f, 0.0f,
                    0.0f,   0.0f,   0.0f,                                   0.0f, 1.0f, 0.0f,
                    right.X, right.Y, right.Z,                  0.0f, 1.0f, 0.0f,
                    0.0f,   0.0f,   0.0f,                                   1.0f, 1.0f, 0.0f,
                    direction.X,   direction.Y,   direction.Z,                    1.0f, 1.0f, 0.0f
                };
                GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)(0), 48 * sizeof(float), v);
                GL.DrawArrays(PrimitiveType.Lines, 0, 8);
                GL.BindVertexArray(0);
                GL.LineWidth(1.0f);
            }

            fontRenderer.DrawText($"Cam pos: {_camera.Position}", new Vector2(0.0f, 20.0f), 32.0f);
            fontRenderer.EndRender();


            SwapBuffers();
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            //if (e.Key == Keys.PageUp)
            //    _camMoveSpeed += 0.5f;
            //if (e.Key == Keys.PageDown)
            //    _camMoveSpeed -= 0.5f;
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);
     
            if(e.Key == Keys.Escape)
                Close();
            if (e.Key == Keys.F2)
                _debugCam = !_debugCam;
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

        
        private void BuildDebug()
        {
            _debugModel = Matrix4.CreateTranslation(_camera.Position);

            float[] verts =
            {
                -0.2f, -0.2f, -0.2f,  1.0f, 1.0f, 1.0f,
                 0.2f, -0.2f, -0.2f,  1.0f, 1.0f, 1.0f,
                 0.2f,  0.2f, -0.2f,  1.0f, 1.0f, 1.0f,
                 0.2f,  0.2f, -0.2f,  1.0f, 1.0f, 1.0f,
                -0.2f,  0.2f, -0.2f,  1.0f, 1.0f, 1.0f,
                -0.2f, -0.2f, -0.2f,  1.0f, 1.0f, 1.0f,

                -0.2f, -0.2f,  0.2f,  1.0f, 1.0f, 1.0f,
                 0.2f, -0.2f,  0.2f,  1.0f, 1.0f, 1.0f,
                 0.2f,  0.2f,  0.2f,  1.0f, 1.0f, 1.0f,
                 0.2f,  0.2f,  0.2f,  1.0f, 1.0f, 1.0f,
                -0.2f,  0.2f,  0.2f,  1.0f, 1.0f, 1.0f,
                -0.2f, -0.2f,  0.2f,  1.0f, 1.0f, 1.0f,

                -0.2f,  0.2f,  0.2f,  1.0f, 1.0f, 1.0f,
                -0.2f,  0.2f, -0.2f,  1.0f, 1.0f, 1.0f,
                -0.2f, -0.2f, -0.2f,  1.0f, 1.0f, 1.0f,
                -0.2f, -0.2f, -0.2f,  1.0f, 1.0f, 1.0f,
                -0.2f, -0.2f,  0.2f,  1.0f, 1.0f, 1.0f,
                -0.2f,  0.2f,  0.2f,  1.0f, 1.0f, 1.0f,

                 0.2f,  0.2f,  0.2f,  1.0f, 1.0f, 1.0f,
                 0.2f,  0.2f, -0.2f,  1.0f, 1.0f, 1.0f,
                 0.2f, -0.2f, -0.2f,  1.0f, 1.0f, 1.0f,
                 0.2f, -0.2f, -0.2f,  1.0f, 1.0f, 1.0f,
                 0.2f, -0.2f,  0.2f,  1.0f, 1.0f, 1.0f,
                 0.2f,  0.2f,  0.2f,  1.0f, 1.0f, 1.0f,

                -0.2f, -0.2f, -0.2f,  1.0f, 1.0f, 1.0f,
                 0.2f, -0.2f, -0.2f,  1.0f, 1.0f, 1.0f,
                 0.2f, -0.2f,  0.2f,  1.0f, 1.0f, 1.0f,
                 0.2f, -0.2f,  0.2f,  1.0f, 1.0f, 1.0f,
                -0.2f, -0.2f,  0.2f,  1.0f, 1.0f, 1.0f,
                -0.2f, -0.2f, -0.2f,  1.0f, 1.0f, 1.0f,

                -0.2f,  0.2f, -0.2f,  1.0f, 1.0f, 1.0f,
                 0.2f,  0.2f, -0.2f,  1.0f, 1.0f, 1.0f,
                 0.2f,  0.2f,  0.2f,  1.0f, 1.0f, 1.0f,
                 0.2f,  0.2f,  0.2f,  1.0f, 1.0f, 1.0f,
                -0.2f,  0.2f,  0.2f,  1.0f, 1.0f, 1.0f,
                -0.2f,  0.2f, -0.2f,  1.0f, 1.0f, 1.0f
            };

            _debugVao = GL.GenVertexArray();
            GL.BindVertexArray(_debugVao);

            _debugVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _debugVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * sizeof(float), verts, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            Vector3 forward = _camera.Forward;
            Vector3 direction = Vector3.Normalize(_camera.Position - _camera.Target);
            float[] v =
            {
                0.0f,   0.0f,   0.0f,                   0.0f, 0.0f, 1.0f,
                forward.X * 2, forward.Y * 2, forward.Z * 2,  0.0f, 0.0f, 1.0f,
                0.0f,   0.0f,   0.0f,                   1.0f, 0.0f, 0.0f,
                _camera.Up.X * 2, _camera.Up.Y * 2, _camera.Up.Z * 2,           1.0f, 0.0f, 0.0f,
                0.0f,   0.0f,   0.0f,                   0.0f, 1.0f, 0.0f,
                _camera.Right.X * 2, _camera.Right.Y * 2, _camera.Right.Z * 2,  0.0f, 1.0f, 0.0f,
                0.0f,   0.0f,   0.0f,                   1.0f, 1.0f, 0.0f,
                direction.X * 2, direction.Y * 2, direction.Z * 2,        1.0f, 1.0f, 0.0f
            };

            _debugCamCoordsVao = GL.GenVertexArray();
            GL.BindVertexArray(_debugCamCoordsVao);
            _debugCamCoords = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _debugCamCoords);
            GL.BufferData(BufferTarget.ArrayBuffer, v.Length * sizeof(float), v, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
        }
        

        
        private void CreateDebugCamera()
        {
            _camDebugPosition = new Vector3(24.0f, 20.0f, 64.0f);
            _camDebugTarget = _camera.Position;

            _debugYaw = 0.0f;

            _debugView = Matrix4.LookAt(_camDebugPosition, _camDebugTarget, Vector3.UnitY);
        }
        

        
        private void UpdateDebug()
        {
            _debugModel = Matrix4.CreateTranslation(_camera.Position); /*Matrix4.CreateRotationY(_debugYaw) * */
        }

    }
}
