using System;
using System.Collections.Generic;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using LGL.Loaders;
using LGL.Utilities;

namespace Lighting
{
    public class LightingGame : GameWindow
    {
        private Shader _shader;
        private Matrix4 _proj;
        private OrbitCamera _camera;
        private Block _block;
        private Matrix4 _blockModel;
        private Color4 _lightColor;
        private const float _ambientStrength = 0.5f;
        private Vector3 _lightPosition;

        public LightingGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            VSync = VSyncMode.On;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Enable(EnableCap.DepthTest);

            _shader = Shader.Load("Lighting", 2, "Assets/Shaders/lighting.vert", "Assets/Shaders/lighting.frag");
            _shader.Use();
            _shader.SetupUniforms(new string[] { "viewproj", "model", "objectColor", 
                "lightColor", "ambientStrenth", "lightPos" });

            _proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)ClientSize.X / (float)ClientSize.Y, 0.1f, 1000.0f);

            _camera = new OrbitCamera(new Vector3(0.0f, 10.0f, 0.0f), new Vector3(0.5f, 1.0f, 0.5f));

            _block = new Block();
            _block.Init();
            _block.Color = Color4.Red;

            _blockModel = Matrix4.CreateScale(1.0f);

            _lightColor = Color4.White;

            _lightPosition = new Vector3(1.0f, 2.0f, 4.0f);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _shader.Dispose();
            _block.Dispose();
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

            _camera.Update(args.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 vp = _camera.View * _proj;
            _shader.Use();
            _shader.UploadMatrix("viewproj", ref vp);
            _shader.UploadMatrix("model", ref _blockModel);
            _shader.UploadColor("objectColor", _block.Color);
            _shader.UploadColor("lightColor", _lightColor);
            _shader.UploadFloat("ambientStrenth", _ambientStrength);
            _shader.UploadVector3("lightPos", _lightPosition);
            _block.Draw();

            SwapBuffers();
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);

            if(e.Key == Keys.Escape)
                Close();
        }
    }
}
