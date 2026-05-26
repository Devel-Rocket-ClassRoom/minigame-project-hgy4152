using UnityEngine;

[CreateAssetMenu(
    fileName = "Mod_NullifyPrevTurnClasses",
    menuName = "Boss/Modifier/NullifyPrevTurnClasses"
)]
public class Mod_NullifyPrevTurnClasses : Modifier
{
    public override void Apply(ChainJudge judge)
    {
        foreach (var cls in judge.prevClassDistribution.Keys)
            judge.classNullified.Add(cls);
    }
}
