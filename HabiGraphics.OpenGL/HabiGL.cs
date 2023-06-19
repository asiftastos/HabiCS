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
        }

        public static void Begin()
        {
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public static void End()
        {
            gLContext.SwapBuffers();
        }

        public static void Viewport(Vector2D<int> size)
        {
            gl.Viewport(size);
        }

        public static void Depth(bool enabled) 
        {
            if(enabled)
            {
                gl.Enable(EnableCap.DepthTest);
                return;
            }

            gl.Disable(EnableCap.DepthTest);
        }

        public static void Blend(bool enabled)
        {
            if( enabled)
            {
                gl.Enable(EnableCap.Blend);
                return;
            }

            gl.Disable(EnableCap.Blend);
        }

        public static void ResetShader()
        {
            gl.UseProgram(0);
        }

        public static void ResetVertexArray()
        {
            gl.BindVertexArray(0);
        }

        public static void ResetArrayBuffer()
        {
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        }

        public static VertexArray CreateVertexArray()
        {
            return new VertexArray(gl);
        }

        public unsafe static VertexBuffer CreateStaticArrayBuffer(int size, void* data)
        {
            return new VertexBuffer(gl, BufferTargetARB.ArrayBuffer, size, data, BufferUsageARB.StaticDraw);
        }

        public unsafe static VertexBuffer CreateDynamicArrayBuffer(int size, void* data)
        {
            return new VertexBuffer(gl, BufferTargetARB.ArrayBuffer, size, data, BufferUsageARB.DynamicDraw);
        }
    }
}