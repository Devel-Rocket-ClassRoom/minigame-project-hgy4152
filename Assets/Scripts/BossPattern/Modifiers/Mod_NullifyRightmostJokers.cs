using UnityEngine;

[CreateAssetMenu(
    fileName = "Mod_NullifyRightmostJokers",
    menuName = "Boss/Modifier/NullifyRightmostJokers"
)]
public class Mod_NullifyRightmostJokers : Modifier
{
    [Min(1)]
    public int count = 2;

    public override void Apply(ChainJudge judge)
    {
        judge.skipRightmostJokers = Mathf.Max(judge.skipRightmostJokers, count);
    }
}
