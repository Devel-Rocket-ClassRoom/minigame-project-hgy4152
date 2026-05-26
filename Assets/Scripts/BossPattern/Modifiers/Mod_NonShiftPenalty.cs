using UnityEngine;

[CreateAssetMenu(fileName = "Mod_NonShiftPenalty", menuName = "Boss/Modifier/NonShiftPenalty")]
public class Mod_NonShiftPenalty : Modifier
{
    [Range(0f, 1f)]
    public float factor = 0.5f;

    public override void Apply(ChainJudge judge)
    {
        judge.nonShiftPenaltyMultiplier *= factor;
    }
}
