using OpenTK.Mathematics;

namespace HabiCS.UI
{
    public class UIBackground
    {
        public Color4 Normal { get; set; }

        public Color4 Hover { get; set; }

        public Color4 Inderact { get; set; }

        public UIBackground(Color4 normal, Color4 hover, Color4 inderact)
        {
            Normal = normal;
            Hover = hover;
            Inderact = inderact;
        }
    }
}