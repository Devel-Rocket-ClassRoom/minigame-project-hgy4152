using UnityEngine;

[CreateAssetMenu(fileName = "Mod_DrawPhaseDuration", menuName = "Boss/Modifier/DrawPhaseDuration")]
public class Mod_DrawPhaseDuration : Modifier
{
    [Min(1f)]
    public float seconds = 14f;

    public override void Apply(ChainJudge judge) { }

    public override void OnTurnStart(BlockManager blockMgr, DrawPhaseTimer dpt)
    {
        dpt.SetPhaseDuration(seconds);
    }
}
