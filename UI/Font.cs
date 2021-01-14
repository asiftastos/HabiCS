using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Text.Json;
using System.IO;

namespace HabiCS.UI
{
    public class Font : IDisposable
    {
        public struct Glyph
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int OriginX { get; set; }
            public int OriginY { get; set; }
            public int Advance { get; set; }
        }

        public static Font Load(string filename, int width, int height)
        {
            Font f;
            using(StreamReader sr = new StreamReader(filename))
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                string jsontext = sr.ReadToEnd();
                
                Utf8JsonReader jsonReader = new Utf8JsonReader(new ReadOnlySpan<byte>(Encoding.UTF8.GetBytes(jsontext)));
                f = JsonSerializer.Deserialize<Font>(ref jsonReader, jsonOptions);
            }

            string fileatlas = Path.ChangeExtension(filename, ".png");
            f.Init(fileatlas, width, height);

            return f;
        }

        private Dictionary<char, Glyph> fontCharacters;
        private Loaders.Texture fontTexture;
        private Loaders.Shader shader;

        // NOTE: This should be private.Maybe move it to a general UI class.Not needed here.
        public int vao;
        private Matrix4 orthoProj;
        private int orthoLocation;
        private int txtScaleLocation;

        private bool disposedValue;

        #region JSON PROPERTIES
        public string Name { get; set; }
        public int Size { get; set; }
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Dictionary<char, Glyph> Characters { get { return fontCharacters; } set { fontCharacters = value; } }
        #endregion


        public Font()
        {
        }

        public void Init(string fontatlas, int clientWidth, int clientHeight)
        {
            // NOTE font atlas has no transparency
            fontTexture = new Loaders.Texture(fontatlas);
            shader = new Loaders.Shader("Font", 2);
            shader.CompileVertexFromFile("Assets/Shaders/font.vert");
            shader.CompileFragmentFromFile("Assets/Shaders/font.frag");
            shader.CreateProgram();
            shader.Use();
            orthoLocation = GL.GetUniformLocation(shader.ShaderID, "projTrans");
            txtScaleLocation = GL.GetUniformLocation(shader.ShaderID, "scale");

            vao = GL.GenVertexArray();
            orthoProj = Matrix4.CreateOrthographicOffCenter(0.0f, (float)clientWidth, 0.0f, (float)clientHeight, 0.1f, 10.0f);
        }

        public void Bind(Matrix4 textScale)
        {
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            shader.Use();
            GL.UniformMatrix4(orthoLocation, false, ref orthoProj);
            GL.UniformMatrix4(txtScaleLocation, false, ref textScale);
            fontTexture.Bind();
            GL.BindVertexArray(vao);
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
            fontTexture.Unbind();
            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
        }

        #region DISPOSABLE PATTERN

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    fontTexture.Dispose();
                    shader.Dispose();
                    //GL.DeleteBuffer(vbo);
                    //GL.DeleteBuffer(ebo);
                    GL.DeleteVertexArray(vao);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Font()
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
