using UnityEngine;

[CreateAssetMenu(fileName = "jc_PrevChain", menuName = "ChainKnights/Joker/PrevChain")]
public class JokerCard_PrevChain : JokerCard
{
    [SerializeField]
    int bonusPerGroup = 6;

    public override int GetBonus(ChainJudge judge) => judge.prevChainCounts[2] * bonusPerGroup;

    public override float DeckBonus(ChainJudge judge) => 1f;
}
