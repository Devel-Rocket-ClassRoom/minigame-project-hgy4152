using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    const int MaxHandSize = 12;

    [SerializeField]
    int discardLimit = 5;
    int _discardsUsed;

    [SerializeField]
    CharacterSet characterSet;

    [SerializeField]
    List<Slot> slots;

    [SerializeField]
    HandUI handUI;

    public List<Block> hand = new();
    public bool IsHandFull => hand.Count >= MaxHandSize;

    static readonly ClassType[] _allTypes = (ClassType[])System.Enum.GetValues(typeof(ClassType));

    public Block DrawBlock()
    {
        if (hand.Count >= MaxHandSize)
            return null;

        int slotIndex = hand.Count;
        if (slots == null || slotIndex >= slots.Count || slots[slotIndex] == null)
        {
            Debug.LogError($"[BlockManager] slots[{slotIndex}] is not assigned in the Inspector.");
            return null;
        }

        ClassType classType = _allTypes[Random.Range(1, _allTypes.Length)];
        Block block = characterSet.CreateBlock(classType, slots[slotIndex].transform);

        if (block == null)
        {
            Debug.LogWarning($"[BlockManager] Failed to create block for {classType}.");
            return null;
        }

        hand.Add(block);
        block.OnDiscardRequested = Discard;
        AssignChainGroup(block);
        slots[slotIndex].PlaceBlock(block, RefreshConnectors);
        RefreshAllBlockVisuals();

        Debug.Log(
            $"[BlockManager] Drew {classType} block (group {block.chainGroupId}). Hand: {hand.Count}/{MaxHandSize}"
        );
        return block;
    }

    public void Discard(Block block)
    {
        int idx = hand.IndexOf(block);
        if (idx < 0)
            return;

        hand.RemoveAt(idx);
        Destroy(block.gameObject);

        Slot empty = slots[idx];
        empty.Clear();
        empty.transform.SetAsLastSibling();
        slots.RemoveAt(idx);
        slots.Add(empty);

        _discardsUsed++;
        DrawBlock();
        RefreshAllBlockVisuals();
        RefreshConnectors();
    }

    public int DiscardsRemaining => Mathf.Max(0, discardLimit - _discardsUsed);

    public void ResetDiscardCount() => _discardsUsed = 0;

    public void RefreshAllBlockVisuals()
    {
        var groups = ChainResolver.ResolveChains(hand);
        foreach (var group in groups)
        {
            for (int i = 0; i < group.Blocks.Count; i++)
                group.Blocks[i].SetChainVisual(group.Length);
        }
        handUI?.Refresh(groups);
    }

    void RefreshConnectors()
    {
        for (int i = 0; i < slots.Count - 1; i++)
        {
            if (slots[i] == null || slots[i + 1] == null)
                continue;

            bool sameGroup =
                slots[i].Block != null
                && slots[i + 1].Block != null
                && slots[i].Block.chainGroupId == slots[i + 1].Block.chainGroupId;

            Color color = sameGroup ? slots[i].Block.data.blockColor : Color.clear;
            slots[i].SetConnector(sameGroup, color);
        }
    }

    public void DrawUntilFull()
    {
        while (hand.Count < MaxHandSize)
            DrawBlock();
    }

    public void RemoveGroup(ChainGroup group)
    {
        foreach (var block in group.Blocks)
        {
            int idx = hand.IndexOf(block);
            if (idx < 0)
                continue;

            hand.RemoveAt(idx);
            Destroy(block.gameObject);

            Slot empty = slots[idx];
            empty.Clear();
            empty.transform.SetAsLastSibling();
            slots.RemoveAt(idx);
            slots.Add(empty);
        }
        RefreshAllBlockVisuals();
    }

    public void DisableDiscard()
    {
        foreach (var block in hand)
            block.OnDiscardRequested = null;
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
