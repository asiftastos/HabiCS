using Silk.NET.OpenGL;

namespace Habi.Graphics.OpenGL
{
    public class ShaderProgram : IDisposable
    {
        private GL _gl;
        private uint _id;
        private Dictionary<string, int> _uniforms = new Dictionary<string, int>();

        public static ShaderProgram LoadFromFile(GL gl, string vertexFile, string fragmentFile, string[] uniforms)
        {
            string[] shaderSources = new string[2];
            if(!string.IsNullOrEmpty(vertexFile))
            {
                shaderSources[0] = File.ReadAllText(vertexFile);
            }
            if(!string.IsNullOrEmpty (fragmentFile))
            {
                shaderSources[1] = File.ReadAllText(fragmentFile);
            }

            return LoadFromSource(gl, shaderSources[0], shaderSources[1], uniforms);
        }

        public static ShaderProgram LoadFromSource(GL gL, string vertexSource, string fragmentSource, string[] uniforms)
        {
            ShaderProgram sh = new ShaderProgram(gL);

            uint vert = sh.Compile(vertexSource, ShaderType.VertexShader);
            uint frag = sh.Compile(fragmentSource, ShaderType.FragmentShader);

            sh.Link(new uint[] { vert, frag });

            sh.Uniforms(uniforms);

            return sh;
        }

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
    }
}
