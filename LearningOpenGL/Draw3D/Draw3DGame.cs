﻿using LGL.Gfx;
using LGL.Loaders;
using LGL.Utils;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using static LGL.LGLState;

namespace Draw3D
{
    public class Draw3DGame : GameWindow
    {
        private Shader _shader;
        
        private OrbitCamera _camera;
        private Matrix4 _projection;
        private DebugDraw debugDraw;
        private Plane _plane;
        private FontRenderer _fontRenderer;
        private double _frameCounter;
        private string _frameTimeText;

        public Draw3DGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            VSync = VSyncMode.On;
            _frameCounter = 0.0f;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            _frameTimeText = "Frame Time: ";

            _fontRenderer = new FontRenderer(ClientSize.X, ClientSize.Y);

            _shader = Shader.Load("Color", 2, "Assets/Shaders/color.vert", "Assets/Shaders/color.frag", false);
            _shader.Enable();
            _shader.SetupUniforms(new string[] { "model", "viewproj", "color" });

            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)ClientSize.X / (float)ClientSize.Y, 0.1f, 1000.0f);

            InitState(this);

            debugDraw = new DebugDraw();

            _plane = new Plane(10, 10, 2.0f);
            _plane.Load();

            _camera = new OrbitCamera(new Vector3(0.0f, 20.0f, 0.0f), new Vector3(5.0f, 0.0f, 5.0f));
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _fontRenderer.Dispose();
            _shader?.Dispose();
            debugDraw?.Dispose();
            _plane?.Dispose();
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
            _frameCounter += args.Time;
            

            base.OnRenderFrame(args);

            Matrix4 vp = _camera.View * _projection;

            BeginDraw();

            _shader.Enable();
            _shader.UploadMatrix("viewproj", ref vp);
            _shader.UploadColor("color", Color4.White);

            Matrix4 m = _plane.Model;
            _shader.UploadMatrix("model", ref m);
            _plane.Draw();

            m = debugDraw.Model;
            _shader.UploadMatrix("model", ref m);
            debugDraw.Draw();

            BeginDraw2D();

            _fontRenderer.BeginRender();

            _fontRenderer.DrawText(_frameTimeText, new Vector2(0.0f, 0.0f), 18.0f);
            _fontRenderer.DrawText("Second test to draw for testing if the batching drawing works", new Vector2(0.0f, 44.0f), 18.0f);

            int batchedChars = _fontRenderer.CurrentBatchedChars;
            _fontRenderer.EndRender();

            EndDraw2D();

            EndDraw(this);

            if(_frameCounter >= 1.0f)
            {
                _frameTimeText = $"Frame Time: {(args.Time * 1000.0f).ToString("0.###")}ms";
                _frameCounter = 0.0f;
                Console.WriteLine($"Batched Chars: {batchedChars}/{_fontRenderer.MaxBatchedChars}({_fontRenderer.DrawedBatches})");
            }
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == Keys.Escape)
                Close();
        }
    }
}
