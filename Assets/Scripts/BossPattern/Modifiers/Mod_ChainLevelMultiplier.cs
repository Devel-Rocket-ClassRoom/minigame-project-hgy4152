using UnityEngine;

[CreateAssetMenu(
    fileName = "Mod_ChainLevelMultiplier",
    menuName = "Boss/Modifier/ChainLevelMultiplier"
)]
public class Mod_ChainLevelMultiplier : Modifier
{
    [Range(1, 3)]
    public int chainLength = 1;

    public float factor = 0.5f;

    public override void Apply(ChainJudge judge)
    {
        int idx = chainLength - 1;
        if (idx >= 0 && idx < 3)
            judge.chainLevelMultiplier[idx] *= factor;
    }
}
