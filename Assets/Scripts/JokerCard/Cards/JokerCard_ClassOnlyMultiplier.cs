using UnityEngine;

[CreateAssetMenu(menuName = "ChainKnights/JokerCard/ClassOnlyMultiplier")]
public class JokerCard_ClassOnlyMultiplier : JokerCard
{
    public ClassType targetClass;
    public float multiplier = 1.2f;

    public override int GetBonus(ChainJudge judge, ChainGroup group) => 0;

    public override float DeckBonus(ChainJudge judge)
    {
        if (judge.classDistribution.Count != 1)
            return 1f;
        return judge.classDistribution.ContainsKey(targetClass) ? multiplier : 1f;
    }
}
