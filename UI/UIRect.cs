using OpenTK.Mathematics;

namespace HabiCS.UI
{
    public struct UIRect
    {
        public Vector2i Position { get; set; }

        public Vector2i Size { get; set; }

        public UIRect(int x, int y, int w, int h)
        {
            Position = new Vector2i(x, y);
            Size = new Vector2i(w, h);
        }
    }
}