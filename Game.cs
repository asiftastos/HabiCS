using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using HabiCS.Scenes;

namespace HabiCS
{
    public class Game : GameWindow
    {
        private Scene currentScene;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        :base(gameWindowSettings, nativeWindowSettings)
        {

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
            currentScene = new Simple(this);
            currentScene.Load();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (IsKeyReleased(Keys.Escape))
                Close();

            currentScene.Update(args.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            currentScene.Render(args.Time);

            SwapBuffers();
        }

        protected override void OnUnload()
        {
            currentScene.Dispose();

            base.OnUnload();
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            currentScene.ProcessInput(e);
        }
    }
}