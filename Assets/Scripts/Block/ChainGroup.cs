using System.Collections.Generic;

public class ChainGroup
{
    public List<Block> Blocks { get; } = new();
    public int Length => Blocks.Count;
    public ClassType DominantClass => Blocks[0].data.ownerClass;
    public Character DominantCharacter => Blocks[0].owner;

    public void Add(Block block) => Blocks.Add(block);
}
