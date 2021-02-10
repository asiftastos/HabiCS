using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace HabiCS.Loaders
{
    class Texture : IDisposable
    {
        private int width;
        private int height;

        private bool disposedValue;

        public int ID { get; private set; }

        public int Width { get { return width; } }
        public int Height { get { return height; } }

        public Texture(string filename)
        {
            Image<Rgba32> image = Image.Load<Rgba32>(filename);

            width = image.Width;
            height = image.Height;

            ID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ID);
            int[] texparam = new int[]
            {
                (int)TextureMinFilter.Linear,
                (int)TextureMagFilter.Linear
            };
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ref texparam[0]);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ref texparam[1]);

            var pixels = new List<byte>(4 * image.Width * image.Height);
            for (int y = 0; y < image.Height; y++) {
	            var row = image.GetPixelRowSpan(y);

	            for (int x = 0; x < image.Width; x++) {
		            pixels.Add(row[x].R);
		            pixels.Add(row[x].G);
		            pixels.Add(row[x].B);
		            pixels.Add(row[x].A);
	            }
            }

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, 
                OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, PixelType.UnsignedByte, pixels.ToArray());

            image.Dispose();
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
