/*
 *  TODO
 *  
 *  [ ] UI: Add button widget element
*/
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
        private double frameCounter;

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
            frameCounter = 0.0f;
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

            labelID = SceneManager.CurrentScreen.AddLabel("FPS", new Vector2(0.0f, 0.0f));
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
            frameCounter += args.Time;
            if (frameCounter >= 1.0f)
            {
                SceneManager.CurrentScreen.SetLabel(labelID, $"Frame Time: {args.Time * 1000.0f}ms");
                frameCounter = 0.0f;
            }

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