using System.Collections.Generic;

public static class ChainResolver
{
    const int MaxChainLength = 3;

    public static List<ChainGroup> ResolveChains(List<Block> hand)
    {
        var groups = new List<ChainGroup>();
        int i = 0;

        while (i < hand.Count)
        {
            // Skip null blocks
            if (hand[i] == null || hand[i].data == null)
            {
                i++;
                continue;
            }

            var group = new ChainGroup();
            group.Add(hand[i]);
            i++;

            while (
                i < hand.Count
                && hand[i] != null
                && hand[i].data != null
                && hand[i].owner == group.Blocks[0].owner
                && group.Length < MaxChainLength
            )
            {
                group.Add(hand[i]);
                i++;
            }

            groups.Add(group);
        }

        AssignGroupIds(groups);
        return groups;
    }

    static void AssignGroupIds(List<ChainGroup> groups)
    {
        for (int g = 0; g < groups.Count; g++)
        {
            foreach (var block in groups[g].Blocks)
                block.chainGroupId = g;
        }
    }
}
