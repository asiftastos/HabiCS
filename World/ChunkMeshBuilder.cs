using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace HabiCS.World
{
    public class ChunkMeshBuilder
    {
        public ChunkMeshBuilder()
        {

        }

#region  BLOCK FACES

        private void AddTopBlockFace(List<Vector3> verts, float blockSize, float xWorldCoords, float yWorldCoords, float zWorldCoords)
        {
            verts.Add(new Vector3(xWorldCoords, yWorldCoords + blockSize, zWorldCoords + blockSize));
            verts.Add(new Vector3(xWorldCoords + blockSize, yWorldCoords + blockSize, zWorldCoords));
            verts.Add(new Vector3(xWorldCoords, yWorldCoords + blockSize, zWorldCoords));
            verts.Add(new Vector3(xWorldCoords + blockSize, yWorldCoords + blockSize, zWorldCoords));
            verts.Add(new Vector3(xWorldCoords, yWorldCoords + blockSize, zWorldCoords + blockSize));
            verts.Add(new Vector3(xWorldCoords + blockSize, yWorldCoords + blockSize, zWorldCoords + blockSize));
        }

        private void AddFrontBlockFace(List<Vector3> verts, float blockSize, float xWorldCoords, float yWorldCoords, float zWorldCoords)
        {
            verts.Add(new Vector3(xWorldCoords, yWorldCoords, zWorldCoords + blockSize));
            verts.Add(new Vector3(xWorldCoords + blockSize, yWorldCoords + blockSize, zWorldCoords + blockSize));
            verts.Add(new Vector3(xWorldCoords, yWorldCoords + blockSize, zWorldCoords + blockSize));
            verts.Add(new Vector3(xWorldCoords + blockSize, yWorldCoords + blockSize, zWorldCoords + blockSize));
            verts.Add(new Vector3(xWorldCoords, yWorldCoords, zWorldCoords + blockSize));
            verts.Add(new Vector3(xWorldCoords + blockSize, yWorldCoords, zWorldCoords + blockSize));
        }
        private void AddRightBlockFace(List<Vector3> verts, float blockSize, float xWorldCoords, float yWorldCoords, float zWorldCoords)
        {
            verts.Add(new Vector3(xWorldCoords + blockSize, yWorldCoords, zWorldCoords + blockSize));
            verts.Add(new Vector3(xWorldCoords + blockSize, yWorldCoords + blockSize, zWorldCoords));
            verts.Add(new Vector3(xWorldCoords + blockSize, yWorldCoords + blockSize, zWorldCoords + blockSize));
            verts.Add(new Vector3(xWorldCoords + blockSize, yWorldCoords + blockSize, zWorldCoords));
            verts.Add(new Vector3(xWorldCoords + blockSize, yWorldCoords, zWorldCoords + blockSize));
            verts.Add(new Vector3(xWorldCoords + blockSize, yWorldCoords, zWorldCoords));
        }
        private void AddBackBlockFace(List<Vector3> verts, float blockSize, float xWorldCoords, float yWorldCoords, float zWorldCoords)
        {
            verts.Add(new Vector3(xWorldCoords + blockSize, yWorldCoords + blockSize, zWorldCoords));
            verts.Add(new Vector3(xWorldCoords, yWorldCoords, zWorldCoords));
            verts.Add(new Vector3(xWorldCoords, yWorldCoords + blockSize, zWorldCoords));
            verts.Add(new Vector3(xWorldCoords + blockSize, yWorldCoords + blockSize, zWorldCoords));
            verts.Add(new Vector3(xWorldCoords + blockSize, yWorldCoords, zWorldCoords));
            verts.Add(new Vector3(xWorldCoords, yWorldCoords, zWorldCoords));
        }

        private void AddLeftBlockFace(List<Vector3> verts, float blockSize, float xWorldCoords, float yWorldCoords, float zWorldCoords)
        {
            verts.Add(new Vector3(xWorldCoords, yWorldCoords, zWorldCoords));
            verts.Add(new Vector3(xWorldCoords, yWorldCoords + blockSize, zWorldCoords + blockSize));
            verts.Add(new Vector3(xWorldCoords, yWorldCoords + blockSize, zWorldCoords));
            verts.Add(new Vector3(xWorldCoords, yWorldCoords, zWorldCoords));
            verts.Add(new Vector3(xWorldCoords, yWorldCoords, zWorldCoords + blockSize));
            verts.Add(new Vector3(xWorldCoords, yWorldCoords + blockSize, zWorldCoords + blockSize));
        }

        private void AddBottomBlockFace(List<Vector3> verts, float blockSize, float xWorldCoords, float yWorldCoords, float zWorldCoords)
        {
            verts.Add(new Vector3(xWorldCoords, yWorldCoords, zWorldCoords + blockSize));
            verts.Add(new Vector3(xWorldCoords, yWorldCoords, zWorldCoords));
            verts.Add(new Vector3(xWorldCoords + blockSize, yWorldCoords, zWorldCoords));
            verts.Add(new Vector3(xWorldCoords, yWorldCoords, zWorldCoords + blockSize));
            verts.Add(new Vector3(xWorldCoords + blockSize, yWorldCoords, zWorldCoords));
            verts.Add(new Vector3(xWorldCoords + blockSize, yWorldCoords, zWorldCoords + blockSize));
        }

#endregion

        public void BuildMeshCubes(Map map, Chunk chunk)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> colors = new List<Vector3>();

            // world coordinates of the chunk
            Vector2i chunkWorldCoords = chunk.Position * Chunk.CHUNK_SIZE;

            foreach (var item in chunk.Blocks)
            {
                //its air move on
                if(item.Value == 0)
                    continue;
                
                //find the world coords of the block from the chunk's world coords
                float xBlockPos = chunkWorldCoords.X + item.Key.X;
                float zBlockPos = chunkWorldCoords.Y + item.Key.Z;
                float yBlockPos = item.Key.Y * Block.BlockSize;

                Vector3i localPos = item.Key;
                Vector3i yPos = localPos + new Vector3i(0, 1, 0); //get the block above this if any
                if(!chunk.IsSolid(yPos))
                {
                    AddTopBlockFace(vertices, Block.BlockSize, xBlockPos, yBlockPos, zBlockPos);
                    for (int i = 0; i < 6; i++)
                    {
                        colors.Add(new Vector3(0.0f, 0.6f, 0.0f));
                    }
                }
                
                //z positive
                bool toAdd = false;
                if(localPos.Z == Chunk.CHUNK_SIZE - 1)
                {
                    //Its in the endge of the chunk
                    if(!chunk.Neighbours.Get(0))
                    {
                        //It's in the edge of the map
                        toAdd = true;
                    }else
                    {
                        //it has a neighbour chunk.we want the oposite block if any
                        Chunk neighbour = map.GetChunk(new Vector2i(chunk.Position.X, chunk.Position.Y + 1));
                        //this should not needed
                        if(neighbour.Position.X != -1)
                        {
                            //in the axis X and Y the block should have the same coords
                            Vector3i nextBlockPos = new Vector3i(localPos.X, localPos.Y, 0);
                            if(!neighbour.IsSolid(nextBlockPos))
                                toAdd = true;
                        }
                    }
                }else
                {
                    //It's in the inner of the chunk, only checking neighbour blocks of this chunk
                    Vector3i zPos = localPos + new Vector3i(0, 0, 1);
                    if(!chunk.IsSolid(zPos))
                        toAdd = true;
                }
                if(toAdd)
                {
                    AddFrontBlockFace(vertices, Block.BlockSize, xBlockPos, yBlockPos, zBlockPos);
                    for (int i = 0; i < 6; i++)
                    {
                        colors.Add(new Vector3(0.7f, 0.3f, 0.0f));
                    }
                    toAdd = false;
                }

                // x positive
                if(localPos.X == Chunk.CHUNK_SIZE - 1)
                {
                    if(!chunk.Neighbours.Get(1))
                    {
                        //It's in the edge of the map
                        toAdd = true;
                    }else
                    {
                        //it has a neighbour chunk.we want the oposite block if any
                        Chunk neighbour = map.GetChunk(new Vector2i(chunk.Position.X + 1, chunk.Position.Y));
                        //this should not needed
                        if(neighbour.Position.X != -1)
                        {
                            //in the axis X and Y the block should have the same coords
                            Vector3i nextBlockPos = new Vector3i(0, localPos.Y, localPos.Z);
                            if(!neighbour.IsSolid(nextBlockPos))
                                toAdd = true;
                        }
                    }
                }else
                {
                    Vector3i xPos = localPos + new Vector3i(1, 0, 0);
                    if(!chunk.IsSolid(xPos))
                        toAdd = true;
                }
                if(toAdd)
                {
                    AddRightBlockFace(vertices, Block.BlockSize, xBlockPos, yBlockPos, zBlockPos);
                    for (int i = 0; i < 6; i++)
                    {
                        colors.Add(new Vector3(0.7f, 0.3f, 0.0f));
                    }
                    toAdd = false;
                }

                // z negative
                if(localPos.Z == 0)
                {
                    if(!chunk.Neighbours.Get(2))
                    {
                        //It's in the edge of the map
                        toAdd = true;
                    }else
                    {
                        //it has a neighbour chunk.we want the oposite block if any
                        Chunk neighbour = map.GetChunk(new Vector2i(chunk.Position.X, chunk.Position.Y - 1));
                        //this should not needed
                        if(neighbour.Position.X != -1)
                        {
                            Vector3i nextBlockPos = new Vector3i(localPos.X, localPos.Y, Chunk.CHUNK_SIZE - 1);
                            if(!neighbour.IsSolid(nextBlockPos))
                                toAdd = true;
                        }
                    }
                }else
                {
                    Vector3i zNeg = localPos - new Vector3i(0, 0, 1);
                    if(!chunk.IsSolid(zNeg))
                        toAdd = true;
                }
                if(toAdd)
                {
                    Vector3i zNeg = localPos - new Vector3i(0, 0, 1);
                    AddBackBlockFace(vertices, Block.BlockSize, xBlockPos, yBlockPos, zBlockPos);
                    for (int i = 0; i < 6; i++)
                    {
                        colors.Add(new Vector3(0.7f, 0.3f, 0.0f));
                    }
                    toAdd = false;
                }

                // x negative
                if(localPos.X == 0)
                {
                    if(!chunk.Neighbours.Get(3))
                    {
                        //It's in the edge of the map
                        toAdd = true;
                    }else
                    {
                        //it has a neighbour chunk.we want the oposite block if any
                        Chunk neighbour = map.GetChunk(new Vector2i(chunk.Position.X - 1, chunk.Position.Y));
                        //this should not needed
                        if(neighbour.Position.X != -1)
                        {
                            Vector3i nextBlockPos = new Vector3i(Chunk.CHUNK_SIZE - 1, localPos.Y, localPos.Z);
                            if(!neighbour.IsSolid(nextBlockPos))
                                toAdd = true;
                        }
                    }
                }else
                {
                    Vector3i xNeg = localPos - new Vector3i(1, 0, 0);
                    if(!chunk.IsSolid(xNeg))
                        toAdd = true;
                }
                if(toAdd)
                {
                    Vector3i xNeg = localPos - new Vector3i(1, 0, 0);
                    AddLeftBlockFace(vertices, Block.BlockSize, xBlockPos, yBlockPos, zBlockPos);
                    for (int i = 0; i < 6; i++)
                    {
                        colors.Add(new Vector3(0.7f, 0.3f, 0.0f));
                    }
                }

                Vector3i yNeg = localPos + new Vector3i(0, -1, 0);
                if(chunk.Blocks.ContainsKey(yNeg) && chunk.Blocks[yNeg] == 0) 
                {
                    AddBottomBlockFace(vertices, Block.BlockSize, xBlockPos, yBlockPos, zBlockPos);
                    for (int i = 0; i < 6; i++)
                    {
                        colors.Add(new Vector3(0.7f, 0.6f, 0.7f));
                    }
                }
            }

            chunk.UpdateMesh(vertices, colors);
        }

        public void BuildMesh(Chunk chunk)
        {
            List<Vector3> vertices = new List<Vector3>();

            Vector2i chunkWorldCoords = chunk.Position * Chunk.CHUNK_SIZE;
            foreach (var item in chunk.Blocks)
            {
                if(item.Value == 0)
                    continue;
                
                //find the world coords of the block from the chunk' world coords
                float xBlockPos = chunkWorldCoords.X - ((Chunk.CHUNK_SIZE * Block.BlockSize) / 2) + item.Key.X;
                float zBlockPos = chunkWorldCoords.Y - ((Chunk.CHUNK_SIZE * Block.BlockSize) / 2) + item.Key.Z;
                float yBlockPos = item.Key.Y * Block.BlockSize;

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

            //chunk.UpdateMesh(vertices);
        }
    }
}