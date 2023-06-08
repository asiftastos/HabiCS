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
        private Camera _cam;

        private Vox _vox;
        private VoxModel _voxModel;
        private Matrix4 _model;

        private Texture _palleteTex;

        public DemoVox(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            VSync = VSyncMode.On;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            _shader = Shader.Load("Color", 2, "Assets/Shaders/voxmodel.vert", "Assets/Shaders/voxmodel.frag", false);
            _shader.Enable();
            _shader.SetupUniforms(new string[] { "model", "view", "ortho" });

            _palleteTex = Texture.Load("Assets/Textures/pal1.png");

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            _cam = new Camera(new Vector3(0.0f, 40.0f, 50.0f), new Vector3(0.0f, 0.0f, 16.0f), Vector3.UnitY);

            _model = Matrix4.Identity; //Matrix4.CreateScale(0.6f);

            _vox = Vox.Load("Assets/Models/Soil.vox");
            _voxModel = new VoxModel(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(16.0f, 16.0f, 16.0f), _vox.Count);
            foreach (var v in _vox.Voxels)
            {
                //_voxModel.AddVoxel(v, VoxPallete.ToColor(_vox.Pallete[v.C]));
                float ustep = 1.0f / (float)_palleteTex.Width;
                float u = ustep * (float)v.C;
                Color4 color = _vox.Pallete[(int)v.C]; //Color4.White
                _voxModel.AddVoxel(v, color, u, ustep);
            }
            _voxModel.BuildMesh();

        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _shader?.Dispose();
            _palleteTex?.Dispose();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if(CursorState == CursorState.Grabbed) { _cam.Update(this); }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            Matrix4 view = _cam.View;
            Matrix4 proj = _cam.Projection((float)(ClientSize.X / ClientSize.Y));

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Enable();
            _shader.UploadMatrix("view", ref view);
            _shader.UploadMatrix("ortho", ref proj);
            _shader.UploadMatrix("model", ref _model);
            _palleteTex.Bind();
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

            if (e.Key == Keys.Tab)
            {
                if (CursorState == CursorState.Normal) 
                { 
                    CursorState = CursorState.Grabbed;
                }
                else
                {
                    CursorState = CursorState.Normal;
                }
            }
        }
    }
}
