/*
 * [x] Implement FontRenderer to draw text.
 * [x] Implemet an orbit camera
 *      [x] Fix camera movement (pan) to the right axis,now it's always in world's X and Z
 *      [ ] Rotation does not rotate properly the right and forward vectors
 */
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using LGL.Loaders;
using OpenTK.Windowing.GraphicsLibraryFramework;
using LGL.Gfx;
using System;

namespace Camera
{
    public class DemoCamera : GameWindow
    {
        private float _camMoveSpeed = 3.5f;
        private const float _camRotSpeed = 18.5f;
        private const float _camRadius = 20.0f;
        private Vector3 _camPosition;
        private Vector3 _camTarget;
        private Quaternion _camRotation;
        private Vector3 _camForward;
        private Vector3 _camRight;
        private Vector3 _camUp;
        private Matrix4 _view;
        private float _camYaw;
        private float _camPitch;
        private float _camRoll;

        private Matrix4 _projection;

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

            shader = Shader.Load("simple", 2, "Assets/Shaders/color.vert", "Assets/Shaders/color.frag");
            shader.SetupUniforms(new string[] { "viewproj", "model", "color" });

            _model = Matrix4.Identity;
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)ClientSize.X / (float)ClientSize.Y, 0.1f, 1000.0f);

            CreateCamera(new Vector3(0.0f, 10.0f, 24.0f), new Vector3(0.0f, 0.0f, -1.0f));

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
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (IsKeyDown(Keys.W))
            {
                MoveCamera(_camForward, _camMoveSpeed * (float)args.Time);
            }
            if(IsKeyDown(Keys.S))
            {
                MoveCamera(_camForward, -_camMoveSpeed * (float)args.Time);
            }
            if (IsKeyDown(Keys.D))
            {
                MoveCamera(_camRight, _camMoveSpeed * (float)args.Time);
            }
            if (IsKeyDown(Keys.A))
            {
                MoveCamera(_camRight, -_camMoveSpeed * (float)args.Time);
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
            UpdateDebug();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 vp = (_debugCam ? _debugView : _view) * _projection;
            
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
                Vector3 forward = _camTarget - _camPosition;
                float[] v =
                {
                    0.0f,   0.0f,   0.0f,                   0.0f, 0.0f, 1.0f,
                    _camForward.X, _camForward.Y, _camForward.Z,  0.0f, 0.0f, 1.0f,
                    0.0f,   0.0f,   0.0f,                                   1.0f, 0.0f, 0.0f,
                    _camUp.X, _camUp.Y, _camUp.Z,                           1.0f, 0.0f, 0.0f,
                    0.0f,   0.0f,   0.0f,                                   0.0f, 1.0f, 0.0f,
                    _camRight.X, _camRight.Y, _camRight.Z,                  0.0f, 1.0f, 0.0f,
                    0.0f,   0.0f,   0.0f,                                   1.0f, 1.0f, 0.0f,
                    forward.X,   forward.Y,   forward.Z,                    1.0f, 1.0f, 0.0f
                };
                GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)(0), 48 * sizeof(float), v);
                GL.DrawArrays(PrimitiveType.Lines, 0, 8);
                GL.BindVertexArray(0);
                GL.LineWidth(1.0f);
            }

            fontRenderer.DrawText($"Cam trg: {_camTarget}", new Vector2(0.0f, 110.0f), 14.0f);
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
            _debugModel = Matrix4.CreateTranslation(_camPosition);

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

            Vector3 forward = _camTarget - _camPosition;

            float[] v =
            {
                0.0f,   0.0f,   0.0f,                   0.0f, 0.0f, 1.0f,
                _camForward.X * 2, _camForward.Y * 2, _camForward.Z * 2,  0.0f, 0.0f, 1.0f,
                0.0f,   0.0f,   0.0f,                   1.0f, 0.0f, 0.0f,
                _camUp.X * 2, _camUp.Y * 2, _camUp.Z * 2,           1.0f, 0.0f, 0.0f,
                0.0f,   0.0f,   0.0f,                   0.0f, 1.0f, 0.0f,
                _camRight.X * 2, _camRight.Y * 2, _camRight.Z * 2,  0.0f, 1.0f, 0.0f,
                0.0f,   0.0f,   0.0f,                   1.0f, 1.0f, 0.0f,
                forward.X * 2, forward.Y * 2, forward.Z * 2,        1.0f, 1.0f, 0.0f
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
        private void CreateCamera(Vector3 pos, Vector3 target)
        {
            _camTarget = target;
            _camYaw = 0.0f;
            _camPitch = 30.0f;
            _camRoll = 0.0f;
            _camRotation = Quaternion.FromEulerAngles(MathHelper.DegreesToRadians(_camPitch), MathHelper.DegreesToRadians(_camYaw), MathHelper.DegreesToRadians(_camRoll));

            //_camPosition = pos;
            _camPosition.X = (float)(_camRadius * MathHelper.Sin(MathHelper.DegreesToRadians(_camYaw))) + _camTarget.X;
            _camPosition.Z = (float)(_camRadius * MathHelper.Cos(MathHelper.DegreesToRadians(_camYaw))) + _camTarget.Z;
            _camPosition.Y = (float)(_camRadius * MathHelper.Sin(MathHelper.DegreesToRadians(_camPitch))) + _camTarget.Y;//pos.Y;

            UpdateCameraDirections();

            _view = Matrix4.LookAt(_camPosition, target, Vector3.UnitY);
        }

        private void UpdateCamera()
        {
            UpdateCameraDirections();
            _view = Matrix4.LookAt(_camPosition, _camTarget, _camUp);//Vector3.UnitY);
        }

        private void UpdateCameraDirections()
        {
            Vector3 forward = Vector3.Normalize(_camTarget - _camPosition);
            _camRight = Vector3.Normalize(Vector3.Cross(forward, Vector3.UnitY));
            _camUp = Vector3.Normalize(Vector3.Cross(_camRight, forward));
            _camForward = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, _camRight));
        }

        private void MoveCamera(Vector3 direction, float factor)
        {
            Vector3 vfactor = direction * factor;
            _camTarget += vfactor;
            //_camPosition += vfactor;
            _camPosition.X = (float)(_camRadius * MathHelper.Sin(MathHelper.DegreesToRadians(_camYaw))) + _camTarget.X;
            _camPosition.Z = (float)(_camRadius * MathHelper.Cos(MathHelper.DegreesToRadians(_camYaw))) + _camTarget.Z;
            _camPosition.Y = (float)(_camRadius * MathHelper.Sin(MathHelper.DegreesToRadians(_camPitch))) + _camTarget.Y;
        }

        private void RotateCamera(float factor)
        {
            //_camRotation.Y = MathHelper.DegreesToRadians(factor);
            //Matrix4 rot = Matrix4.CreateFromQuaternion(_camRotation);
            //_camPosition = Vector3.TransformPosition((_camPosition - _camTarget) + _camTarget, rot);
            
            _camYaw += factor;
            _camPosition.X = (float)(_camRadius * MathHelper.Sin(MathHelper.DegreesToRadians(_camYaw))) + _camTarget.X;
            _camPosition.Z = (float)(_camRadius * MathHelper.Cos(MathHelper.DegreesToRadians(_camYaw))) + _camTarget.Z;
            _camPosition.Y = (float)(_camRadius * MathHelper.Sin(MathHelper.DegreesToRadians(_camPitch))) + _camTarget.Y;

            //debug
            Quaternion.ToEulerAngles(_camRotation, out Vector3 r);
            _debugYaw += r.Y;
        }

        private void CreateDebugCamera()
        {
            _camDebugPosition = new Vector3(24.0f, 20.0f, 44.0f);
            _camDebugTarget = _camPosition;

            _debugYaw = 0.0f;

            _debugView = Matrix4.LookAt(_camDebugPosition, _camDebugTarget, Vector3.UnitY);
        }

        private void UpdateDebug()
        {
            _debugModel = /*Matrix4.CreateRotationY(_debugYaw) * */Matrix4.CreateTranslation(_camPosition);
        }
    }
}
