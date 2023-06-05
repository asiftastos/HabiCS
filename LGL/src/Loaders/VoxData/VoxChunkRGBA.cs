using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace LGL.Loaders.VoxData
{
    public class VoxChunkRGBA : VoxChunk
    {
        public VoxChunkRGBA(string id, int contentsize, int childrencount, long datastart) : base(id, contentsize, childrencount, datastart)
        {
        }

        public override void Content(Vox v, byte[] data)
        {
            base.Content(v, data);

            int dataindex = 0;
            for (int i = 0; i < 256; i++)
            {
                v.Pallete[i] = new Color4(data[dataindex], data[dataindex + 1], data[dataindex + 2], data[dataindex + 3]);
                dataindex += 4;
            }
        }
    }
}
