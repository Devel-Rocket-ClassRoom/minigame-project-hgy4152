using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    const int MaxHandSize = 12;

    [SerializeField]
    CharacterSet characterSet;

    public List<Block> hand = new();

    static readonly ClassType[] _allTypes = (ClassType[])System.Enum.GetValues(typeof(ClassType));

    public Block DrawBlock()
    {
        if (hand.Count >= MaxHandSize)
            return null;

        ClassType classType = _allTypes[Random.Range(0, _allTypes.Length)];
        Block block = characterSet.CreateBlock(classType);
        hand.Add(block);
        AssignChainGroup(block);
        Debug.Log(
            $"[BlockManager] Drew {classType} block (group {block.chainGroupId}). Hand: {hand.Count}/{MaxHandSize}"
        );
        return block;
    }

    public void DrawUntilFull()
    {
        while (hand.Count < MaxHandSize)
            DrawBlock();
    }

    void AssignChainGroup(Block block)
    {
        int last = hand.Count - 1;
        if (last == 0)
        {
            block.chainGroupId = 0;
            return;
        }

        Block prev = hand[last - 1];
        if (prev.data.id != block.data.id)
        {
            block.chainGroupId = prev.chainGroupId + 1;
            return;
        }

        int groupSize = 0;
        for (int i = last - 1; i >= 0 && hand[i].chainGroupId == prev.chainGroupId; i--)
            groupSize++;

        block.chainGroupId = groupSize < 3 ? prev.chainGroupId : prev.chainGroupId + 1;
    }
}
