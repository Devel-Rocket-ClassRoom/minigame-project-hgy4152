using UnityEngine;

[CreateAssetMenu(fileName = "Mod_DiscardLimit", menuName = "Boss/Modifier/DiscardLimit")]
public class Mod_DiscardLimit : Modifier
{
    [Min(0)]
    public int limit = 5;

    public override void Apply(ChainJudge judge) { }

    public override void OnTurnStart(BlockManager blockMgr, DrawPhaseTimer dpt)
    {
        blockMgr.SetDiscardLimit(limit);
    }
}
