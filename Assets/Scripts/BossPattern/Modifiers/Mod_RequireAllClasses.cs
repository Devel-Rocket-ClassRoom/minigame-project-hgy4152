using UnityEngine;

[CreateAssetMenu(fileName = "Mod_RequireAllClasses", menuName = "Boss/Modifier/RequireAllClasses")]
public class Mod_RequireAllClasses : Modifier
{
    [Min(2)]
    public int requiredClassCount = 3;

    public override void Apply(ChainJudge judge)
    {
        judge.requireAllThreeClasses = judge.classDistribution.Count < requiredClassCount;
    }
}
