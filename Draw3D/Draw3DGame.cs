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
        VertexArrayObject _vao;
        VertexBuffer _vbo;
        Shader _shader;

        Matrix4 _model;
        Matrix4 _view;
        Matrix4 _projection;

        List<VertexColor> _verts;
        int _vCount;

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

            _verts = new List<VertexColor>();
            _verts.AddRange(new VertexColor[] {
                new VertexColor(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f),
                new VertexColor(1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f),
                new VertexColor(0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f),
                new VertexColor(0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f),
                new VertexColor(0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f),
                new VertexColor(0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f),
            });
            _vCount = _verts.Count;

            _vao = new VertexArrayObject();
            _vao.Set();
            _vbo = new VertexBuffer(BufferTarget.ArrayBuffer);
            _vbo.Set();
            _vbo.Data<VertexColor>(BufferUsageHint.StaticDraw, _verts.ToArray(), VertexColor.SizeInBytes);
            _vao.Attributes(new VertexAttribute[] {
                new VertexAttribute(0, 3, VertexColor.SizeInBytes, 0),
                new VertexAttribute(1, 3, VertexColor.SizeInBytes, Vector3.SizeInBytes)
            }, VertexAttribPointerType.Float);

            _model = Matrix4.Identity;
            _view = Matrix4.LookAt(new Vector3(1.0f, 2.0f, 8.0f), Vector3.Zero, Vector3.UnitY);
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)ClientSize.X / (float)ClientSize.Y, 0.1f, 1000.0f);

            GL.Enable(EnableCap.DepthTest);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _verts.Clear();
            _vao?.Dispose();
            _vbo?.Dispose();
            _shader?.Dispose();
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

            GL.PointSize(2.0f);
            _shader.Use();
            _shader.UploadMatrix("model", ref _model);
            _shader.UploadMatrix("viewproj", ref vp);
            _shader.UploadColor("color", Color4.White);
            _vao.Set();
            GL.DrawArrays(PrimitiveType.Lines, 0, _vCount);

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
