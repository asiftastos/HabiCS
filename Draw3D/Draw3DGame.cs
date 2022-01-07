using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using LGL.Loaders;
using LGL.Gfx;

namespace Draw3D
{
    public class Draw3DGame : GameWindow
    {
        Shader _shader;
        
        Matrix4 _view;
        Matrix4 _projection;

        DebugDraw debugDraw;

        public Draw3DGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            VSync = VSyncMode.On;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            _shader = Shader.Load("Color", 2, "Assets/Shaders/color.vert", "Assets/Shaders/color.frag");
            _shader.Use();
            _shader.SetupUniforms(new string[] { "model", "viewproj", "color" });

            _view = Matrix4.LookAt(new Vector3(1.0f, 2.0f, 8.0f), Vector3.Zero, Vector3.UnitY);
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)ClientSize.X / (float)ClientSize.Y, 0.1f, 1000.0f);

            GL.Enable(EnableCap.DepthTest);

            debugDraw = new DebugDraw();
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _shader?.Dispose();
            debugDraw?.Dispose();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            Matrix4 vp = _view * _projection;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();
            Matrix4 m = debugDraw.Model;
            _shader.UploadMatrix("model", ref m);
            _shader.UploadMatrix("viewproj", ref vp);
            _shader.UploadColor("color", Color4.White);
            debugDraw.Draw();

            SwapBuffers();
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == Keys.Escape)
                Close();
        }
    }
}
