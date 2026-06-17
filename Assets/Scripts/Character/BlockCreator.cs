using System.Collections.Generic;
using UnityEngine;

public abstract class BlockCreator : MonoBehaviour
{
    [SerializeField]
    protected BlockData blockData;

    [SerializeField]
    protected Block blockPrefab;

    readonly Stack<Block> _pool = new();

    public BlockData GetBlockData() => blockData;

    public Block CreateBlock(Transform spawner)
    {
        Block block = null;
        while (_pool.Count > 0 && block == null)
            block = _pool.Pop(); // 파괴된 인스턴스는 건너뜀

        if (block == null)
            block = Instantiate(blockPrefab, spawner);
        else
        {
            block.transform.SetParent(spawner, false);
            block.PrepareForReuse();
            block.gameObject.SetActive(true);
        }

        block.Init(blockData);
        block.owner = GetComponent<Character>();
        return block;
    }

    public void ReleaseBlock(Block block)
    {
        block.CancelAnimations();
        block.OnDiscardRequested = null;
        block.chainGroupId = -1;
        block.gameObject.SetActive(false);
        block.transform.SetParent(transform, false);
        _pool.Push(block);
    }
}
