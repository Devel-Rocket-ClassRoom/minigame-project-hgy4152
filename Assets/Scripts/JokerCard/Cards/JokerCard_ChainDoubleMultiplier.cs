using UnityEngine;

[CreateAssetMenu(menuName = "ChainKnights/JokerCard/ChainDoubleMultiplier")]
public class JokerCard_ChainDoubleMultiplier : JokerCard
{
    public int chainA;
    public int chainB;
    public float multiplier = 1.2f;

    public override int GetBonus(ChainJudge judge, ChainGroup group) => 0;

    public override float DeckBonus(ChainJudge judge)
    {
        bool hasA = ChainCount(judge, chainA) >= 1;
        bool hasB = ChainCount(judge, chainB) >= 1;
        if (!hasA || !hasB)
            return 1f;
        for (int i = 1; i <= 3; i++)
            if (i != chainA && i != chainB && ChainCount(judge, i) > 0)
                return 1f;
        return multiplier;
    }

    static int ChainCount(ChainJudge j, int len) =>
        len == 1 ? j.chain1Count
        : len == 2 ? j.chain2Count
        : j.chain3Count;
}
