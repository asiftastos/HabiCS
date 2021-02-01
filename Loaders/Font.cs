using System;
using System.Collections.Generic;
using System.Text;
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

        private void Init(string fontatlas, int clientWidth, int clientHeight)
        {
            // NOTE font atlas has no transparency
            fontTexture = new Loaders.Texture(fontatlas);
        }

        public void Bind()
        {
            fontTexture.Bind();
        }

        public void Unbind()
        {
            fontTexture.Unbind();
        }

        #region DISPOSABLE PATTERN

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    fontTexture.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
