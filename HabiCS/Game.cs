using HabiCS.Graphics;
using HabiCS.UI;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace HabiCS
{
    public class Game : GameWindow
    {
        private SceneManager sceneManager;
        private int labelID;

        public SceneManager SceneManager
        {
            get
            {
                return sceneManager;
            }
        }

        public RenderPass RenderPass { get; set; }


        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
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

            SceneManager.ChangeScreen(screen);

            labelID = SceneManager.CurrentScreen.AddLabel("Test", new Vector2(0.0f, 0.0f));
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (IsKeyReleased(Keys.Escape))
                Close();

            if (IsKeyReleased(Keys.Enter))
                SceneManager.CurrentScreen.SetLabel(labelID, "Kostas");

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
    }
}