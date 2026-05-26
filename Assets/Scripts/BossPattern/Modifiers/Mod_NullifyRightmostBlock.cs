using UnityEngine;

[CreateAssetMenu(
    fileName = "Mod_NullifyRightmostBlock",
    menuName = "Boss/Modifier/NullifyRightmostBlock"
)]
public class Mod_NullifyRightmostBlock : Modifier
{
    [Min(1)]
    public int count = 1;

    public override void Apply(ChainJudge judge) { }

    public override void PreResolve(BlockManager blockMgr)
    {
        for (int i = 0; i < count && blockMgr.hand.Count > 0; i++)
        {
            var block = blockMgr.hand[^1];
            blockMgr.hand.RemoveAt(blockMgr.hand.Count - 1);
            Object.Destroy(block.gameObject);
        }
    }
}
