using System;
using System.IO;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using Utilities.StbImageSharp;

namespace LGL.Loaders
{
    class Texture : IDisposable
    {
        public static Texture Load(string filename)
        {
            ImageResult image;
            using(var stream = File.OpenRead(filename))
            {
                image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            }

            Texture tex = new Texture();
            tex.Width = image.Width;
            tex.Height = image.Height;

            tex.ID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, tex.ID);
            int[] texparam = new int[]
            {
                (int)TextureMinFilter.Linear,
                (int)TextureMagFilter.Linear
            };
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ref texparam[0]);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ref texparam[1]);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, 
                OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            return tex;
        }

        private int width;
        private int height;

        private bool disposedValue;

        public int ID { get; set; }

        public int Width { get { return width; } set { width = value; } }
        public int Height { get { return height; } set { height = value; } }

        public Texture()
        {
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, ID);
        }

        public void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    GL.DeleteTexture(ID);
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
    }
}
