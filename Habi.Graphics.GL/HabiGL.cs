using Silk.NET.Core.Contexts;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace HabiGraphics.OpenGL
{
    public unsafe static class HabiGL
    {
        private static IGLContext gLContext;
        private static GL gl;

        public static Vector4D<float> ClearColorValue { get; set; }

        public static void Init(IGLContext context)
        {
            gLContext = context;
            gl = GL.GetApi(gLContext);

            ClearColorValue = new Vector4D<float>(50f, 50f, 50f, 1.0f);
            gl.ClearColor(ClearColorValue);

            EnableDepth(true);
        }

        public static void BeginRender()
        {
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public static void EndRender()
        {
            gLContext.SwapBuffers();
        }

        public static void Viewport(Vector2D<int> size)
        {
            gl.Viewport(size);
        }

        public static void EnableDepth(bool enabled) 
        {
            if(enabled)
            {
                gl.Enable(EnableCap.DepthTest);
                return;
            }

            gl.Disable(EnableCap.DepthTest);
        }
    }
}