using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace LGL.Loaders
{
    /*
     * A shader program compiled and linked or a seperate shader program compiled and linked to support pipeline object
     */
    public class Shader : IDisposable
    {
        public static Shader Load(string name, int numOfShaders,string vertexfile, string fragmentfile, bool linkSeperate)
        {
            Shader shader = new Shader(name, numOfShaders);
            shader.CompileVertexFromFile(vertexfile);
            shader.CompileFragmentFromFile(fragmentfile);
            shader.CreateProgram(linkSeperate);
            return shader;
        }

        private string name;
        private int[] shaderObjects; //number of shader objects to store.Index 0 is always the program object

        private Dictionary<string, int> uniformLocations;

        public string Name { get { return name; } private set { } }
        public int ShaderID { get { return shaderObjects[0]; } private set { } }

        public Shader(string name, int numOfShaders)
        {
            this.name = name;
            shaderObjects = new int[numOfShaders + 1]; //For example 2 shader objects (vertex and fragment) + 1 for the shader program object.
            uniformLocations = new Dictionary<string, int>();
        }

        public void CompileVertexFromSource(string source)
        {
            var handle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(handle, source);
            GL.CompileShader(handle);

            var shaderLog = GL.GetShaderInfoLog(handle);
            if (!string.IsNullOrWhiteSpace(shaderLog))
                Console.WriteLine($"Vertex shader error [{name}]: {shaderLog}");

            shaderObjects[1] = handle;
        }

        public void CompileVertexFromFile(string filepath)
        {
            try
            {
                using(StreamReader sr = new StreamReader(filepath))
                {
                    CompileVertexFromSource(sr.ReadToEnd());
                }
            }catch(IOException e)
            {
                Console.WriteLine($"File loading shader error [{name}]: {e.Message}");
            }
        }

        public void CompileFragmentFromSource(string source)
        {
            var handle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(handle, source);
            GL.CompileShader(handle);

            var shaderLog = GL.GetShaderInfoLog(handle);
            if (!string.IsNullOrWhiteSpace(shaderLog))
                Console.WriteLine($"Fragment shader error [{name}]: {shaderLog}");

            shaderObjects[2] = handle;
        }

        public void CompileFragmentFromFile(string filepath)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filepath))
                {
                    CompileFragmentFromSource(sr.ReadToEnd());
                }
            }
            catch (IOException e)
            {
                Console.WriteLine($"File loading shader error [{name}]: {e.Message}");
            }
        }

        public void CreateProgram(bool linkSeperate)
        {
            var handle = GL.CreateProgram();
            GL.AttachShader(handle, shaderObjects[1]);
            GL.AttachShader(handle, shaderObjects[2]);
            if (linkSeperate)
                GL.ProgramParameter(handle, ProgramParameterName.ProgramSeparable, 1);
            GL.LinkProgram(handle);

            var programLog = GL.GetProgramInfoLog(handle);
            if (!string.IsNullOrWhiteSpace(programLog))
                Console.WriteLine($"Program shader error [{name}]: {programLog}");

            GL.DeleteShader(shaderObjects[1]);
            GL.DeleteShader(shaderObjects[2]);

            shaderObjects[0] = handle;
        }
        
        public void Enable()
        {
            GL.UseProgram(ShaderID);
        }

        public void Disable()
        { 
            GL.UseProgram(0); 
        }

        public void SetupUniforms(string[] names)
        {
            foreach (var name in names)
            {
                int loc = GL.GetProgramResourceLocation(ShaderID, ProgramInterface.Uniform, name);
                if(loc == -1)
                {
                    Console.WriteLine($"Uniform {name} not found.");
                    return;
                }
                uniformLocations.Add(name, loc);
            }
        }
        public void UploadMatrix(string name, ref Matrix4 m)
        {
            if(uniformLocations.ContainsKey(name))
                GL.ProgramUniformMatrix4(ShaderID, uniformLocations[name], false, ref m);
        }

        public void UploadColor(string name, Color4 c)
        {
            if(uniformLocations.ContainsKey(name))
                GL.ProgramUniform4(ShaderID, uniformLocations[name], c);
        }

        public void UploadBool(string name, bool value)
        {
            if(uniformLocations.ContainsKey(name))
            {
                int b = 0;
                if(value)
                    b = 1;
                GL.ProgramUniform1(ShaderID, uniformLocations[name], b);
            }
        }

        public void UploadFloat(string name, float value)
        {
            if(uniformLocations.ContainsKey(name))
                GL.ProgramUniform1(ShaderID, uniformLocations[name], value);
        }

        public void UploadVector3(string name, Vector3 value)
        {
            if(uniformLocations.ContainsKey(name))
                GL.ProgramUniform3(ShaderID, uniformLocations[name], value);
        }

        #region DISPOSABLE PATTERN

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    GL.DeleteProgram(ShaderID);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Shader()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
