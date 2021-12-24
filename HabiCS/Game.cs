using System;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using HabiCS.Scenes;
using HabiCS.Graphics;
using HabiCS.UI;

namespace HabiCS
{
    public class Game : GameWindow
    {
        private SceneManager sceneManager;

        public SceneManager SceneManager {
            get {
                return sceneManager;
            }
        }

        public RenderPass RenderPass { get; set; }
        

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        :base(gameWindowSettings, nativeWindowSettings)
        {
            sceneManager = new SceneManager(this);
            KeyDown += sceneManager.ProcessKeyInput;
            MouseDown += sceneManager.ProcessMouseButtonDown;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        }
        
        protected override void OnLoad()
        {
            base.OnLoad();

            VSync = VSyncMode.On;

            GL.ClearColor(0.3f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            //scenes
            sceneManager.Load();
            
            //uiscreen
            var screen = new UIScreen(this);

            Button b = new Button(0.0f, 0.0f, 200.0f, 40.0f, "Texturing", SceneManager.Font);
            b.OnClicked += OnTexturing;
            screen.Elements.Add("Texturing", b);

            Button b1 = new Button(0.0f, 45.0f, 200.0f, 40.0f, "Simple", SceneManager.Font);
            b1.OnClicked += OnSimple;
            screen.Elements.Add("Simple", b1);
            
            SceneManager.ChangeScreen(screen);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (IsKeyReleased(Keys.Escape))
                Close();

            sceneManager.Update(args.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            //3D
            RenderPass = RenderPass.PASS3D;
            sceneManager.Render(args.Time);

            //2D
            RenderPass = RenderPass.PASS2D;
            sceneManager.Render(args.Time);

            // swap
            SwapBuffers();
        }

        protected override void OnUnload()
        {
            sceneManager.Dispose();

            base.OnUnload();
        }

        protected void OnTexturing()
        {
            Console.WriteLine("Texturing scene clicked");
            SceneManager.ChangeScene(new Texturing(this));
        }

        protected void OnSimple()
        {
            Console.WriteLine("Simple scene clicked");
            SceneManager.ChangeScene(new Simple(this));
        }
    }
}