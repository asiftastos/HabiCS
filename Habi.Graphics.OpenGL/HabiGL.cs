using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Habi.Graphics.OpenGL
{
    public unsafe static class HabiGL
    {
        private static GL gl;

        public static Vector4D<float> ClearColorValue { get; set; }

        public static void Init(GL gL)
        {
            gl = gL;

            ClearColorValue = new Vector4D<float>(50f, 50f, 50f, 1.0f);
            gl.ClearColor(ClearColorValue);
        }

        public static void Begin()
        {
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public static void End()
        {
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

        public static void PolygoneModeFill()
        {
            gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
        }

        public static void PolygoneModeLine()
        {
            gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
        }

        public static void PolygoneModePoint()
        {
            gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Point);
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

        public static VertexArrayObject CreateVertexArray()
        {
            return new VertexArrayObject(gl);
        }

        public unsafe static VertexBufferObject CreateStaticArrayBuffer(int size, void* data)
        {
            return new VertexBufferObject(gl, BufferTargetARB.ArrayBuffer, size, data, BufferUsageARB.StaticDraw);
        }

        public unsafe static VertexBufferObject CreateDynamicArrayBuffer(int size, void* data)
        {
            return new VertexBufferObject(gl, BufferTargetARB.ArrayBuffer, size, data, BufferUsageARB.DynamicDraw);
        }

        public static ShaderProgram CreateShaderFromFile(string vertexFile, string fragmentFile, string[] uniforms)
        {
            string[] shaderSources = new string[2];
            if (!string.IsNullOrEmpty(vertexFile))
            {
                shaderSources[0] = File.ReadAllText(vertexFile);
            }
            if (!string.IsNullOrEmpty(fragmentFile))
            {
                shaderSources[1] = File.ReadAllText(fragmentFile);
            }

            return CreateShaderFromSource(shaderSources[0], shaderSources[1], uniforms);
        }

        public static ShaderProgram CreateShaderFromSource(string vertexSource, string fragmentSource, string[] uniforms)
        {
            ShaderProgram sh = new ShaderProgram(gl);

            uint vert = sh.Compile(vertexSource, ShaderType.VertexShader);
            uint frag = sh.Compile(fragmentSource, ShaderType.FragmentShader);

            sh.Link(new uint[] { vert, frag });

            sh.Uniforms(uniforms);

            return sh;
        }
    }
}