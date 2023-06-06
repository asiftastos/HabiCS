using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

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

            Texture tex = new Texture();
            tex.Width = image.Width;
            tex.Height = image.Height;

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            GL.CreateTextures(TextureTarget.Texture2D, 1, out tex.ID);

            int[] texparam = new int[]
            {
                (int)TextureMinFilter.Linear,
                (int)TextureMagFilter.Linear
            };
            GL.TextureParameterI(tex.ID, TextureParameterName.TextureMinFilter, ref texparam[0]);
            GL.TextureParameterI(tex.ID, TextureParameterName.TextureMagFilter, ref texparam[1]);

            GL.TextureStorage2D(tex.ID, 1, SizedInternalFormat.Rgba8, image.Width, image.Height);
            GL.TextureSubImage2D(tex.ID, 0, 0, 0, image.Width, image.Height, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);

            return tex;
        }

        private int width;
        private int height;

        private bool disposedValue;

        public int ID;

        public int Width { get { return width; } set { width = value; } }
        public int Height { get { return height; } set { height = value; } }

        public Texture()
        {
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
    }
}
