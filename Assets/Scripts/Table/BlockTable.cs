using UnityEngine;

[CreateAssetMenu(fileName = "BlockTable", menuName = "ChainKnights/Table/BlockTable")]
public class BlockTable : StringTable<BlockData>
{
    private void OnEnable()
    {
        entries.Clear();
        entries.AddRange(Resources.LoadAll<BlockData>("Blocks"));
    }
}
