using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    const int MaxHandSize = 12;

    [SerializeField]
    int discardLimit = 5;
    int _discardsUsed;
    int _runtimeDiscardLimit = -1;
    bool _swapPending;

    int EffectiveLimit => _runtimeDiscardLimit >= 0 ? _runtimeDiscardLimit : discardLimit;

    public void SetDiscardLimit(int n) => _runtimeDiscardLimit = n;

    public void ResetDiscardLimit() => _runtimeDiscardLimit = -1;

    public event Action OnDrawBlocked;

    [SerializeField]
    CharacterSet characterSet;

    [SerializeField]
    List<Slot> slots;

    [SerializeField]
    HandUI handUI;

    public List<Block> hand = new();
    public bool IsHandFull => hand.Count >= MaxHandSize;

    public Block DrawBlock()
    {
        if (hand.Count >= MaxHandSize)
        {
            if (!_swapPending)
            {
                _swapPending = true;
                OnDrawBlocked?.Invoke();
            }
            return null;
        }

        int slotIndex = hand.Count;
        if (slots == null || slotIndex >= slots.Count || slots[slotIndex] == null)
        {
            Debug.LogError($"[BlockManager] slots[{slotIndex}] is not assigned in the Inspector.");
            return null;
        }

        var deployedTypes = characterSet.GetDeployedClassTypes();
        if (deployedTypes.Length == 0)
        {
            Debug.LogError("[BlockManager] 배포된 캐릭터가 없습니다.");
            return null;
        }
        ClassType classType = deployedTypes[UnityEngine.Random.Range(0, deployedTypes.Length)];
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

        return block;
    }

    public void Discard(Block block)
    {
        _swapPending = false;
        int idx = hand.IndexOf(block);
        if (idx < 0)
            return;
        if (_discardsUsed >= EffectiveLimit)
            return;

        hand.RemoveAt(idx);
        Destroy(block.gameObject);
        slots[idx].Clear();
        _discardsUsed++;

        // 오른쪽 블록들을 슬롯 고정 상태에서 왼쪽으로 슬라이드
        for (int i = idx; i < hand.Count; i++)
        {
            slots[i + 1].Clear();
            bool isLast = i == hand.Count - 1;
            slots[i].ShiftBlock(hand[i], isLast ? RefreshConnectors : null);
        }

        // 체인 그룹 재배정 (idx부터 끝까지)
        for (int i = idx; i < hand.Count; i++)
            AssignChainGroupAt(i);

        DrawBlock();
        RefreshAllBlockVisuals();
    }

    public int DiscardsRemaining => Mathf.Max(0, EffectiveLimit - _discardsUsed);
    public int DiscardsUsed => _discardsUsed;

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
        RefreshConnectors();
    }

    public void DisableDiscard()
    {
        foreach (var block in hand)
            block.OnDiscardRequested = null;
    }

    void AssignChainGroup(Block block) => AssignChainGroupAt(hand.Count - 1);

    void AssignChainGroupAt(int i)
    {
        if (i == 0)
        {
            hand[i].chainGroupId = 0;
            return;
        }

        Block cur = hand[i];
        Block prev = hand[i - 1];
        if (prev.data.id != cur.data.id)
        {
            cur.chainGroupId = prev.chainGroupId + 1;
            return;
        }

        int groupSize = 0;
        for (int j = i - 1; j >= 0 && hand[j].chainGroupId == prev.chainGroupId; j--)
            groupSize++;

        cur.chainGroupId = groupSize < 3 ? prev.chainGroupId : prev.chainGroupId + 1;
    }
}
