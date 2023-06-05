using System;
using System.Collections.Generic;
using System.Text;

namespace LGL.Loaders.VoxData
{
    public class VoxChunk
    {
        private string _id;
        private int _contentSize;
        private int _childrenCount;
        private long _dataStartIndex;

        public long StartDataIndex { get { return _dataStartIndex; } }

        public int ContentSize { get { return _contentSize; } }

        public VoxChunk(string id, int contentsize, int childrencount, long datastart) 
        {
            _id = id;
            _contentSize = contentsize;
            _childrenCount = childrencount;
            _dataStartIndex = datastart;
        }

        public virtual void Content(Vox v, byte[] data){}

        public virtual void Children() { }
    }
}
