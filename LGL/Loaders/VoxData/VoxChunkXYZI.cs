using System;
using System.Collections.Generic;
using System.Text;

namespace LGL.Loaders.VoxData
{
    public class VoxChunkXYZI : VoxChunk
    {
        public VoxChunkXYZI(string id, int contentsize, int childrencount, long datastart) : base(id, contentsize, childrencount, datastart)
        {
        }

        public override void Content(Vox v, byte[] data)
        {
            base.Content(v, data);

            int voxelCount = BitConverter.ToInt32(data, 0);

            for (int i = 0; i < voxelCount; i++)
            {
                int index = i * 4;
                byte vx = data[index + 4];
                byte vy = data[index + 5];
                byte vz = data[index + 6];
                byte vi = data[index + 7];

                v.AddVoxel(vx, vz, vy, vi);
            }
        }
    }
}
