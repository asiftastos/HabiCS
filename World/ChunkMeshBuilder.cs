using System.Collections.Generic;
using OpenTK.Mathematics;

namespace HabiCS.World
{
    public class ChunkMeshBuilder
    {
        public ChunkMeshBuilder()
        {

        }

        public void BuildMesh(Chunk chunk, float blockSize)
        {
            List<Vector3> vertices = new List<Vector3>();

            Vector2i chunkWorldCoords = chunk.Position * Chunk.CHUNK_SIZE;
            foreach (var item in chunk.Blocks)
            {
                if(item.Value == 0)
                    continue;
                
                //find the world coords of the block from the chunk' world coords
                float xBlockPos = chunkWorldCoords.X - ((Chunk.CHUNK_SIZE * blockSize) / 2) + item.Key.X;
                float zBlockPos = chunkWorldCoords.Y - ((Chunk.CHUNK_SIZE * blockSize) / 2) + item.Key.Z;
                float yBlockPos = item.Key.Y * blockSize;

                // check neighbours if present {top, front, left, back, right, bottom}
                bool[] neihbours = {false, false, false, false, false, false};
                Vector3i index = new Vector3i(item.Key.X, item.Key.Y + 1, item.Key.Z);
                if(chunk.Blocks.ContainsKey(index) && chunk.Blocks[index] == 1)
                {
                    neihbours[0] = true;
                }
                index.Y = item.Key.Y;
                index.Z = item.Key.Z + 1;
                if(chunk.Blocks.ContainsKey(index) && chunk.Blocks[index] == 1)
                {
                    neihbours[1] = true;
                }
                index.Z = item.Key.Z;
                index.X = item.Key.X + 1;
                if(chunk.Blocks.ContainsKey(index) && chunk.Blocks[index] == 1)
                {
                    neihbours[2] = true;
                }
                index.X = item.Key.X;
                index.Z = item.Key.Z - 1;
                if(chunk.Blocks.ContainsKey(index) && chunk.Blocks[index] == 1)
                {
                    neihbours[3] = true;
                }
                index.Z = item.Key.Z;
                index.X = item.Key.X - 1;
                if(chunk.Blocks.ContainsKey(index) && chunk.Blocks[index] == 1)
                {
                    neihbours[4] = true;
                }
                index.X = item.Key.X;
                index.Y = item.Key.Y - 1;
                if(chunk.Blocks.ContainsKey(index) && chunk.Blocks[index] == 1)
                {
                    neihbours[5] = true;
                }

                bool final = true;
                for (int i = 0; i < 6; i++)
                {
                    if(neihbours[i])
                        continue;
                    final =  neihbours[i];
                }
                
                if(!final)
                    vertices.Add(new Vector3(xBlockPos, yBlockPos, zBlockPos));
            }

            chunk.UpdateMesh(vertices);
        }
    }
}