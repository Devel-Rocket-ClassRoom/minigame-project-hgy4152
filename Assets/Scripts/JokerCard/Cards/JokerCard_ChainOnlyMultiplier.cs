using UnityEngine;

[CreateAssetMenu(menuName = "ChainKnights/JokerCard/ChainOnlyMultiplier")]
public class JokerCard_ChainOnlyMultiplier : JokerCard
{
    public int chainLength;
    public float multiplier = 1.2f;

    public override int GetBonus(ChainJudge judge, ChainGroup group) => 0;

    public override float DeckBonus(ChainJudge judge)
    {
        bool hasTarget = ChainCount(judge, chainLength) >= 1;
        bool hasOthers = false;
        for (int i = 1; i <= 3; i++)
            if (i != chainLength && ChainCount(judge, i) > 0)
                hasOthers = true;
        return hasTarget && !hasOthers ? multiplier : 1f;
    }

    static int ChainCount(ChainJudge j, int len) =>
        len == 1 ? j.chain1Count
        : len == 2 ? j.chain2Count
        : j.chain3Count;
}
