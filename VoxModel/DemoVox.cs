using LGL.Loaders;
using LGL.Utils;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace VoxModel
{
    public class DemoVox : GameWindow
    {
        private Shader _shader;
        private OrbitCamera _camera;
        private Matrix4 _projection;

        private Vox _vox;
        private VoxModel _voxModel;
        private Matrix4 _model;

        public DemoVox(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            VSync = VSyncMode.On;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            _shader = Shader.Load("Color", 2, "Assets/Shaders/color.vert", "Assets/Shaders/color.frag", false);
            _shader.Enable();
            _shader.SetupUniforms(new string[] { "model", "viewproj", "color" });

            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)ClientSize.X / (float)ClientSize.Y, 0.1f, 1000.0f);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            _camera = new OrbitCamera(new Vector3(0.0f, 40.0f, 50.0f), new Vector3(0.0f, 0.0f, 25.0f));
            _camera.Radius = 32.0f;

            _model = Matrix4.Identity; //Matrix4.CreateScale(0.6f);

            _vox = Vox.Load("Assets/Models/Soil.vox");
            _voxModel = new VoxModel(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(16.0f, 16.0f, 16.0f), _vox.Count);
            foreach (var v in _vox.Voxels)
            {
                //_voxModel.AddVoxel(v, VoxPallete.ToColor(_vox.Pallete[v.C]));
                _voxModel.AddVoxel(v, _vox.Pallete[(int)v.C]);
            }
            _voxModel.BuildMesh();
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _shader?.Dispose();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.E))
            {
                _camera.Rotate(0.5f * args.Time);
            }
            if (KeyboardState.IsKeyDown(Keys.Q))
            {
                _camera.Rotate(-0.5f * args.Time);
            }
            if (KeyboardState.IsKeyDown(Keys.D))
            {
                _camera.Move(0.5f * args.Time, _camera.Right);
            }
            if (KeyboardState.IsKeyDown(Keys.A))
            {
                _camera.Move(-0.5f * args.Time, _camera.Right);
            }
            if (KeyboardState.IsKeyDown(Keys.W))
            {
                _camera.Move(-0.5f * args.Time, _camera.Forward);
            }
            if (KeyboardState.IsKeyDown(Keys.S))
            {
                _camera.Move(0.5f * args.Time, _camera.Forward);
            }

            _camera.Radius += MouseState.ScrollDelta.Y;
            _camera.Update(args.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            Matrix4 vp = _camera.View * _projection;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Enable();
            _shader.UploadMatrix("viewproj", ref vp);
            _shader.UploadColor("color", Color4.White);
            _shader.UploadMatrix("model", ref _model);
            _voxModel.Draw();

            SwapBuffers();
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == Keys.Escape)
                Close();

            if(e.Key == Keys.F3)
                _voxModel.DebugDraw = !_voxModel.DebugDraw;
        }
    }
}
