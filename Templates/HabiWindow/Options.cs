using Habi;

namespace HabiWindow
{
    public struct Options
    {
        public string Title { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public GFX GfxApi { get; set; }

        // Default has no Graphics API initialized
        public static Options Default => new Options("Game", 800, 600, GFX.NoApi);

        // Default OpenGL options
        public static Options DefaultOGL => new Options("Game", 800, 600, GFX.OpenGL);

        public Options(string title, int width, int height, GFX gfxApi)
        {
            Title = title;
            Width = width;
            Height = height;
            GfxApi = gfxApi;
        }

        public override bool Equals(object? obj)
        {
            return obj is Options options &&
                   Title == options.Title &&
                   Width == options.Width &&
                   Height == options.Height &&
                   GfxApi == options.GfxApi;
        }
    }
}
