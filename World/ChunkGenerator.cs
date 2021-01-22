using System.Collections.Generic;
using OpenTK.Mathematics;

namespace HabiCS.World
{
    public class ChunkGenerator
    {
        FastNoiseLite noise;

        public ChunkGenerator(int seed)
        {
            noise = new FastNoiseLite(seed);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFrequency(0.004f);
        }

        public void Generate(Chunk chunk, float blockSize)
        {
            // Get the chunk's coordinates in the world
            Vector2i worldCoords = chunk.Position * Chunk.CHUNK_SIZE;

            //List<Vector3> vertices = new List<Vector3>();
            //start the chunk half to the right of the center of the chunk
            // this is basically the block position in the chunk, local not the world
            float xBlockPos = worldCoords.X - ((Chunk.CHUNK_SIZE * blockSize) / 2);
            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
            {
                float zBlockPos = worldCoords.Y - ((Chunk.CHUNK_SIZE * blockSize) / 2);
                for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
                {
                    
                    //get the height from the noise

                    //with octaves is more detailed the terrain, like hills
                    double octave1 = noise.GetNoise((float)xBlockPos, (float)zBlockPos);
                    double octave2 = 0.5 * noise.GetNoise((float)xBlockPos * 2, (float)zBlockPos * 2);
                    double octave3 = 0.25 * noise.GetNoise((float)xBlockPos * 4, (float)zBlockPos * 4);
                    double yheight = MathHelper.Floor( (octave1 + octave2 + octave3) * (float)Chunk.CHUNK_HEIGHT);
                    
                    for(int y = 0; y < Chunk.CHUNK_HEIGHT; y++)
                    {
                        if(y <= yheight)
                        {
                            chunk.Blocks.Add(new Vector3i(x, y, z), 1); //solid
                        } else {
                            chunk.Blocks.Add(new Vector3i(x, y, z), 0); //air
                        }
                    }

                    // more smooth terrain, like plains
                    //double yheight = MathHelper.Floor( noise.GetSimplex((float)xBlockPos, (float)zBlockPos) * (float)Chunk.CHUNK_HEIGHT);

                    // Add a column of points up to the height generated from noise.
                    //float yBlockPos = 0.0f; // start from ground 0 and go up
                    //for (int y = 0; y < yheight; y++)
                    //{
              //          vertices.Add(new Vector3(xBlockPos, yBlockPos, zBlockPos)); //for now render points on the block positions
                    //    yBlockPos += blockSize;
                    //}

                    // add a point to the height generated from noise
                    //vertices.Add(new Vector3(xBlockPos, (float)yheight, zBlockPos));

                    zBlockPos += blockSize;
                }
                xBlockPos += blockSize;
            }

            //chunk.UpdateMesh(vertices);
        }

        public void GenerateFlat(Chunk chunk, float blockSize, int height)
        {
            Vector2i worldCoords = chunk.Position * Chunk.CHUNK_SIZE;
            
            float xBlockPos = worldCoords.X - ((Chunk.CHUNK_SIZE * blockSize) / 2);
            for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
            {
                float zBlockPos = worldCoords.Y - ((Chunk.CHUNK_SIZE * blockSize) / 2);
                for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
                {
                    if(height < 0)
                        height = 0;
                    
                    float yBlockPos = 0.0f; // start from ground 0 and go up to height parameter
                    for(int y = 0; y <= Chunk.CHUNK_HEIGHT; y++)
                    {
                        if(y <= height) {
                            chunk.Blocks.Add(new Vector3i(x, y, z), 1);
                        } else {
                            chunk.Blocks.Add(new Vector3i(x, y, z), 0);
                        }
                        yBlockPos += blockSize;
                    }
                    zBlockPos += blockSize;
                }
                xBlockPos += blockSize;
            }
        }
    }
}
