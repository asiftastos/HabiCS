using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using HabiCS.Scenes;

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

        private UIManager uiManager;

        public UIManager UIManager {
            get {
                return uiManager;
            }
        }

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        :base(gameWindowSettings, nativeWindowSettings)
        {
            sceneManager = new SceneManager(this);
            KeyDown += sceneManager.ProcessKeyInput;

            uiManager = new UIManager(this);
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

            //ui
            uiManager.Load();

            //scenes
            //sceneManager.ChangeScene(new Simple(this));
            sceneManager.ChangeScene(new MainMenu(this));
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
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //3D
            sceneManager.Render(args.Time);

            //2D
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            
            uiManager.Render(args.Time);

            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);

            // swap
            SwapBuffers();
        }

        protected override void OnUnload()
        {
            uiManager.Dispose();
            sceneManager.Dispose();

            base.OnUnload();
        }
    }
}