using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;
using static System.Net.Mime.MediaTypeNames;

namespace LGL.Loaders
{
    public class Texture : IDisposable
    {
        public static Texture Load(string filename)
        {
            ImageResult image;
            using(var stream = File.OpenRead(filename))
            {
                image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            }

            Texture tex = new Texture(image.Width, image.Height);

            tex.MinFilter(TextureMinFilter.Linear);
            tex.MagFilter(TextureMagFilter.Linear);

            tex.SetPixels(0, 0, image.Width, image.Height, image.Data);

            return tex;
        }

        private int width;
        private int height;

        private bool disposedValue;

        public int ID;

        public int Width =>  width;
        public int Height => height;

        public Texture(int w, int h)
        {
            this.width = w;
            this.height = h;

            GL.CreateTextures(TextureTarget.Texture2D, 1, out ID);
            GL.TextureStorage2D(ID, 1, SizedInternalFormat.Rgba8, this.width, this.height);
        }

        public void Bind()
        {
            GL.BindTextureUnit(0, ID);
        }

        public void Bind(TextureUnit texUnit)
        {
            GL.ActiveTexture(texUnit);
            GL.BindTextureUnit((int)texUnit, ID);
        }

        public void Unbind()
        {
            GL.BindTextureUnit(0, 0);
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

        public void MinFilter(TextureMinFilter filter)
        {
            GL.TextureParameterI(ID, TextureParameterName.TextureMinFilter, new int[] { (int)filter });
        }

        public void MagFilter(TextureMagFilter filter)
        {
            GL.TextureParameterI(ID, TextureParameterName.TextureMagFilter, new int[] { (int)filter });
        }

        public void SetPixels(int xoffset, int yoffset, int w, int h, byte[] data)
        {
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            GL.TextureSubImage2D(ID, 0, xoffset, yoffset, w, h, PixelFormat.Rgba, PixelType.UnsignedByte, data);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);
        }
    }
}
