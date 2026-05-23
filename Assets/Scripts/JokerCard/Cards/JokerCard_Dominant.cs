using UnityEngine;

[CreateAssetMenu(fileName = "jc_Dominant", menuName = "ChainKnights/Joker/Dominant")]
public class JokerCard_Dominant : JokerCard
{
    [SerializeField]
    int minGroupCount = 3;

    [SerializeField]
    float bonus = 1.3f;

    public override int GetBonus(ChainJudge judge) => 0;

    public override float DeckBonus(ChainJudge judge)
    {
        int maxCount = 0;
        foreach (var v in judge.classDistribution.Values)
            if (v > maxCount)
                maxCount = v;
        return maxCount >= minGroupCount ? bonus : 1f;
    }
}
