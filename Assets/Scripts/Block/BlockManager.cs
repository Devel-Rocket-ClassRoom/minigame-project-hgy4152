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
        Debug.Log($"[BlockManager] Drew {classType} block. Hand: {hand.Count}/{MaxHandSize}");
        return block;
    }

    public void DrawUntilFull()
    {
        while (hand.Count < MaxHandSize)
            DrawBlock();
    }
}
