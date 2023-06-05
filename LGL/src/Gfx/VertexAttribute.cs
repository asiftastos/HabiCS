namespace LGL.Gfx
{
    public struct VertexAttribute
    {
        public int Index;
        public int ElementsCount;
        public int Offset;

        public VertexAttribute(int index, int elementsCount, int offset)
        {
            Index = index;
            ElementsCount = elementsCount;
            Offset = offset;
        }
    }
}
