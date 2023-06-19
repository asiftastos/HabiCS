using Silk.NET.OpenGL;

namespace HabiGraphics.OpenGL
{
    public class Shader : IDisposable
    {
        private GL _gl;
        private uint _id;
        private Dictionary<string, int> uniforms = new Dictionary<string, int>();

        internal Shader(GL gl)
        {
            _gl = gl;
            _id = _gl.CreateProgram();
        }

        public void Dispose()
        {
            _gl.DeleteProgram(_id);
        }

        public void Enable()
        {
            _gl.UseProgram(_id);
        }
    }
}
