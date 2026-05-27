using UnityEngine;

[CreateAssetMenu(menuName = "ChainKnights/JokerCard/AllChainsMultiplier")]
public class JokerCard_AllChainsMultiplier : JokerCard
{
    public float multiplier = 1.5f;

    public override int GetBonus(ChainJudge judge, ChainGroup group) => 0;

    public override float DeckBonus(ChainJudge judge) =>
        judge.chain1Count >= 1 && judge.chain2Count >= 1 && judge.chain3Count >= 1
            ? multiplier
            : 1f;
}
