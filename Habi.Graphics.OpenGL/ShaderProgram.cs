using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Habi.Graphics.OpenGL
{
    public class ShaderProgram : IDisposable
    {
        private GL _gl;
        private uint _id;
        private Dictionary<string, int> _uniforms = new Dictionary<string, int>();

        internal ShaderProgram(GL gl)
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

        public uint Compile(string source, ShaderType type)
        {
            uint id = _gl.CreateShader(type);

            _gl.ShaderSource(id, source);
            _gl.CompileShader(id);

            string log = _gl.GetShaderInfoLog(_id);
            if(!string.IsNullOrEmpty(log))
            {
                Console.WriteLine($"Shader compile error [{type.ToString()}]: {log}");
            }

            return id;
        }

        public void Link(uint[] shaderIds)
        {
            for(int i = 0; i < shaderIds.Length; i++)
            {
                _gl.AttachShader(_id, shaderIds[i]);
            }

            _gl.LinkProgram(_id);

            string log = _gl.GetProgramInfoLog(_id);
            if(!string.IsNullOrEmpty(log))
            {
                Console.WriteLine($"Shader link error: {log}");
            }

            for(int i = 0; i < shaderIds.Length; i++)
            {
                _gl.DetachShader(_id, shaderIds[i]);
                _gl.DeleteShader(shaderIds[i]);
            }
        }

        public void Uniforms(string[] uniforms)
        {
            foreach (var uniform in uniforms)
            {
                _uniforms.Add(uniform, _gl.GetUniformLocation(_id, uniform));
            }
        }

        public unsafe void UploadMatrix(string uniformName, Matrix4X4<float> matrix)
        {
            _gl.UniformMatrix4(_uniforms[uniformName], 1, false, (float*)&matrix);
        }

        public unsafe void UploadVector3(string uniformName, Vector3D<float> vector)
        {
            _gl.Uniform3(_uniforms[uniformName], 1, (float*)&vector);
        }

        public unsafe void UploadVector4(string uniformName, Vector4D<float> vector)
        {
            _gl.Uniform4(_uniforms[uniformName], 1, (float*)&vector);
        }
    }
}
