namespace HabiWindow
{
    public enum GFX
    {
        None,
        OpenGL,
        Vulkan,
        DirectX12,
        WebGpu
    }

    public struct HabiOptions
    {
        public string title;
        public int width;
        public int height;
        public GFX gfx;

        public HabiOptions(string title, int width = 800, int height = 600, GFX fX = GFX.None)
        {
            this.title = title;
            this.width = width;
            this.height = height;
            this.gfx = fX;
        }
    }
}
