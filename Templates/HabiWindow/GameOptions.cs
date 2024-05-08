using Habi;

namespace HabiWindow
{
    public struct GameOptions
    {
        public string Title { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public GFX GfxApi { get; set; }

        // Default options with no Graphics API defined
        public static GameOptions Default => new GameOptions("Game", 800, 600, GFX.NoApi);

        // Default OpenGL options
        public static GameOptions DefaultOGL => new GameOptions("Game", 800, 600, GFX.OpenGL);

        // Default Vulkan options
        public static GameOptions DefaultVulkan => new GameOptions("Game", 800, 600, GFX.Vulkan);

        // Default DirectX12 options
        public static GameOptions DefaultDX12 => new GameOptions("Game", 800, 600, GFX.NoApi);

        // Default WebGPU options
        public static GameOptions DefaultWGPU => new GameOptions("Game", 800, 600, GFX.NoApi);

        public GameOptions(string title, int width, int height, GFX gfxApi)
        {
            Title = title;
            Width = width;
            Height = height;
            GfxApi = gfxApi;
        }

        public override bool Equals(object? obj)
        {
            return obj is GameOptions options &&
                   Title == options.Title &&
                   Width == options.Width &&
                   Height == options.Height &&
                   GfxApi == options.GfxApi;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Title, Width, Height, GfxApi);
        }

        public override string? ToString()
        {
            return $"Game: {this.Title}, Size: {this.Width} - {this.Height}, API: {this.GfxApi.ToString()}";
        }
    }
}
