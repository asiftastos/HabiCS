using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenTK.Mathematics;
using LGL.Loaders.VoxData;

namespace LGL.Loaders
{
    public class Vox
    {
        private string _id;
        private int _version;
        private Vector3 _size;
        private List<Voxel> _voxels;
        private Color4[] _colors;

        public Vector3 Count { get { return _size; } set { _size = value; } }
        public int Width { get { return (int)_size.X; } }
        public int Height { get { return (int)_size.Y; } }
        public int Depth { get {  return (int)_size.Z; } }

        public List<Voxel> Voxels { get { return _voxels; } }

        public Color4[] Pallete { get { return _colors; } }

        public Vox(string id, int version)
        {
            _id = id;
            _version = version;
            _voxels = new List<Voxel>();
            _colors = new Color4[256];
        }

        public static Vox Load(string path)
        {
            if(!File.Exists(path))
            {
                throw new FileNotFoundException("File not found", path);
            }

            List<VoxChunk> chunks = new List<VoxChunk>();

            FileStream fs = File.OpenRead(path);
            long sizebytes = fs.Length;
            using(BinaryReader br = new BinaryReader(fs))
            {
                //Should be "VOX "
                string id = Encoding.UTF8.GetString(br.ReadBytes(4));
                sizebytes -= 4;
                //Console.WriteLine($"ID: {Encoding.UTF8.GetString(id)}");
                
                //Should be 150
                int version = br.ReadInt32();
                sizebytes -= 4;
                //Console.WriteLine($"Version: {version}");

                while(sizebytes > 0)
                {
                    string chunkid = Encoding.UTF8.GetString(br.ReadBytes(4));
                    int chunkContentSize = br.ReadInt32();
                    int childrenChunkCount = br.ReadInt32();
                    sizebytes -= 12 + chunkContentSize;
                    //Console.WriteLine($"ID: {chunkid}, Content: {chunkContentSize} bytes, Children: {childrenChunkCount} bytes, {sizebytes} bytes remaining");

                    switch (chunkid)
                    {
                        case "MAIN":
                            chunks.Add(new VoxChunk(chunkid, chunkContentSize, childrenChunkCount, br.BaseStream.Position));
                            break;
                        case "SIZE":
                            chunks.Add(new VoxChunkSize(chunkid, chunkContentSize, childrenChunkCount, br.BaseStream.Position));
                            br.ReadBytes(chunkContentSize);
                            break;
                        case "XYZI":
                            chunks.Add(new VoxChunkXYZI(id, chunkContentSize, childrenChunkCount, br.BaseStream.Position));
                            br.ReadBytes(chunkContentSize);
                            break;
                        case "RGBA":
                            chunks.Add(new VoxChunkRGBA(id, chunkContentSize, childrenChunkCount, br.BaseStream.Position));
                            br.ReadBytes(chunkContentSize);
                            break;
                        default:
                            br.ReadBytes(chunkContentSize);
                            break;
                    }
                }

                Vox v = new Vox(id, version);

                br.BaseStream.Position = 0;

                foreach (var c in chunks)
                {
                    br.BaseStream.Position = c.StartDataIndex;
                    c.Content(v, br.ReadBytes(c.ContentSize));
                }

                return v;
            }
        }

        public void AddVoxel(byte x, byte y, byte z, byte c)
        {
            _voxels.Add(new Voxel(x, y, z, c));
        }

        public struct Voxel
        {
            public byte X;
            public byte Y;
            public byte Z;
            public byte C;

            public Voxel(byte x, byte y, byte z, byte c)
            {
                X = x;
                Y = y;
                Z = z;
                C = c;
            }
        }
    }
}
