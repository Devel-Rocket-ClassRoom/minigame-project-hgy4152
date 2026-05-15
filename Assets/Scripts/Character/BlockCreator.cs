using UnityEngine;

public abstract class BlockCreator : MonoBehaviour
{
    [SerializeField]
    protected BlockData blockData;

    [SerializeField]
    protected Block blockPrefab;

    public BlockData GetBlockData() => blockData;

    public Block CreateBlock()
    {
        Block block = Instantiate(blockPrefab);
        block.Init(blockData);
        return block;
    }

    public Block CreateChainedBlock()
    {
        Block block = Instantiate(blockPrefab);
        block.Init(blockData);
        OnChained(block);
        return block;
    }

    // 체이닝 발생 시 직업별 추가 처리가 필요하면 오버라이드
    protected virtual void OnChained(Block block) { }
}
