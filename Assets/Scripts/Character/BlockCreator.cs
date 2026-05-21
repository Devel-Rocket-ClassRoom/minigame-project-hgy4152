using UnityEngine;

public abstract class BlockCreator : MonoBehaviour
{
    [SerializeField]
    protected BlockData blockData;

    [SerializeField]
    protected Block blockPrefab;

    // 0=chain1, 1=chain2, 2=chain3 — 에셋 준비 후 Inspector에서 연결
    [SerializeField]
    protected GameObject[] chainEffectPrefabs;

    public BlockData GetBlockData() => blockData;

    public Block CreateBlock(Transform spawner)
    {
        Block block = Instantiate(blockPrefab, spawner);
        block.Init(blockData);
        return block;
    }

    public virtual void PlayEffect(int chainCount, Transform effectRoot)
    {
        if (chainEffectPrefabs == null || chainEffectPrefabs.Length == 0)
            return;
        int idx = Mathf.Clamp(chainCount - 1, 0, chainEffectPrefabs.Length - 1);
        if (chainEffectPrefabs[idx] != null)
            Instantiate(chainEffectPrefabs[idx], effectRoot.position, Quaternion.identity);
    }
}
