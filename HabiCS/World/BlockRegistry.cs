using System.Collections.Generic;

namespace HabiCS.World
{
    public class BlockRegistry
    {
        private Dictionary<string, Block> blockRegistry;

        public BlockRegistry()
        {
            blockRegistry = new Dictionary<string, Block>();
        }

        public void Register(Block block)
        {
            blockRegistry.Add(block.Name, block);
        }

        public Block GetBlock(string name)
        {
            return blockRegistry[name];
        }
    }
}