using System;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.Drawing.Imaging;

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
            Bitmap image = new Bitmap(filename);

            width = image.Width;
            height = image.Height;

            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            ID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ID);
            int[] texparam = new int[]
            {
                (int)TextureMinFilter.Linear,
                (int)TextureMagFilter.Linear
            };
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ref texparam[0]);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ref texparam[1]);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, 
                OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, PixelType.UnsignedByte, data.Scan0);

            image.UnlockBits(data);

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

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Texture()
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
    }
}
