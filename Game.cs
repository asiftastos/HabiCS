using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using HabiCS.Scenes;
using HabiCS.Graphics;

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
            sceneManager.ChangeScene(new Start(this));
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
            sceneManager.Render(args.Time, RenderPass.PASS3D);

            //2D
            sceneManager.Render(args.Time, RenderPass.PASS2D);

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