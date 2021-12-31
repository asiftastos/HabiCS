﻿using LGL.Loaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Texturing
{
    public class DemoTexturing : GameWindow
    {
        private Matrix4 _ortho;
        private Matrix4 _eye;
        private Matrix4 _model;
        private Texture _texture;
        private Shader _textureShader;
        private int _texturingVao;
        private int _texturingVbo;

        public DemoTexturing(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            VSync = VSyncMode.On;

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            _model = Matrix4.Identity;
            _eye = Matrix4.LookAt(Vector3.Zero, new Vector3(0.0f, 0.0f, -1.0f), Vector3.UnitY);
            _ortho = Matrix4.CreateOrthographicOffCenter(0.0f, (float)ClientSize.X, 0.0f, (float)ClientSize.Y, 0.1f, 10.0f);

            _texture = Texture.Load("Assets/Textures/wall.jpg");

            _textureShader = Shader.Load("Texturing", 2, "Assets/Shaders/texturing.vert",
                                        "Assets/Shaders/texturing.frag");
            _textureShader.SetupUniforms(new string[] { "model", "view", "ortho" });

            float[] vertices = new float[] {
                100.0f, 100.0f, -1.0f,  1.0f, 1.0f, 1.0f,   0.0f, 0.0f,
                200.0f, 100.0f, -1.0f,  1.0f, 1.0f, 1.0f,   1.0f, 0.0f,
                100.0f, 200.0f, -1.0f,  1.0f, 1.0f, 1.0f,   0.0f, 1.0f,
                100.0f, 200.0f, -1.0f,  1.0f, 1.0f, 1.0f,   0.0f, 1.0f,
                200.0f, 100.0f, -1.0f,  1.0f, 1.0f, 1.0f,   1.0f, 0.0f,
                200.0f, 200.0f, -1.0f,  1.0f, 1.0f, 1.0f,   1.0f, 1.0f
            };

            _texturingVao = GL.GenVertexArray();
            GL.BindVertexArray(_texturingVao);

            _texturingVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _texturingVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, sizeof(float) * 3);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, sizeof(float) * 8, sizeof(float) * 6);
            GL.EnableVertexAttribArray(2);

            GL.BindVertexArray(0);
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

            _textureShader.Use();
            _textureShader.UploadMatrix("model", ref _model);
            _textureShader.UploadMatrix("view", ref _eye);
            _textureShader.UploadMatrix("ortho", ref _ortho);
            _texture.Bind();
            GL.BindVertexArray(_texturingVao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);

            SwapBuffers();
        }

        protected override void OnUnload()
        {
            _texture.Dispose();
            _textureShader.Dispose();
            GL.DeleteBuffer(_texturingVbo);
            GL.DeleteVertexArray(_texturingVao);
            
            base.OnUnload();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        }
    }
}
