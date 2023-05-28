using OpenTK.Mathematics;
using System;

namespace LGL.Loaders.VoxData
{
    public class VoxChunkSize : VoxChunk
    {
        public VoxChunkSize(string id, int contentsize, int childrencount, long datastart) : base(id, contentsize, childrencount, datastart)
        {
        }

        public override void Content(Vox v, byte[] data)
        {
            base.Content(v, data);

            int x = BitConverter.ToInt32(data, 0);
            int y = BitConverter.ToInt32(data, 4);
            int z = BitConverter.ToInt32(data, 8);

            v.Count = new Vector3((float)x, (float)y, (float)z);
        }
    }
}
