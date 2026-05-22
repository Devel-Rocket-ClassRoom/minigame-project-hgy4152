using UnityEngine;

public abstract class BlockCreator : MonoBehaviour
{
    [SerializeField]
    protected BlockData blockData;

    [SerializeField]
    protected Block blockPrefab;

    public BlockData GetBlockData() => blockData;

    public Block CreateBlock(Transform spawner)
    {
        Block block = Instantiate(blockPrefab, spawner);
        block.Init(blockData);
        return block;
    }
}
