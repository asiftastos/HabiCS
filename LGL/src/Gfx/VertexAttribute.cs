using System;
using System.Collections.Generic;
using System.Text;

namespace LGL.Gfx
{
    public struct VertexAttribute
    {
        public int Index;
        public int ElementsCount;
        public int Stride;
        public int Offset;

        public VertexAttribute(int index, int elementsCount, int stride, int offset)
        {
            Index = index;
            ElementsCount = elementsCount;
            Stride = stride;
            Offset = offset;
        }
    }
}
