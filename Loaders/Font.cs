﻿using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Text.Json;
using System.IO;

namespace HabiCS.Loaders
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

        private Matrix4 orthoProj;
        private int orthoLocation;
        
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
            shader = Shader.Load("Font", 2, "Assets/Shaders/font.vert", "Assets/Shaders/font.frag");
            shader.Use();
            orthoLocation = GL.GetUniformLocation(shader.ShaderID, "projTrans");
        
            orthoProj = Matrix4.CreateOrthographicOffCenter(0.0f, (float)clientWidth, 0.0f, (float)clientHeight, 0.1f, 1.0f);
        }

        public void Bind()
        {
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            shader.Use();
            GL.UniformMatrix4(orthoLocation, false, ref orthoProj);
            fontTexture.Bind();
            
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
