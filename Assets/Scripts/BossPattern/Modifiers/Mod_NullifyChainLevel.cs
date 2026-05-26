using UnityEngine;

[CreateAssetMenu(fileName = "Mod_NullifyChainLevel", menuName = "Boss/Modifier/NullifyChainLevel")]
public class Mod_NullifyChainLevel : Modifier
{
    [Range(1, 3)]
    public int chainLength = 1;

    public override void Apply(ChainJudge judge)
    {
        int idx = chainLength - 1;
        if (idx >= 0 && idx < 3)
            judge.chainLevelNullified[idx] = true;
    }
}
