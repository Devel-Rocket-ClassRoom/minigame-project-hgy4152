using UnityEngine;

[CreateAssetMenu(fileName = "jc_Chain1Boost", menuName = "ChainKnights/Joker/Chain1Boost")]
public class JokerCard_Chain1Boost : JokerCard
{
    [SerializeField]
    int bonusPerGroup = 5;

    public override int GetBonus(ChainJudge judge) => judge.chain1Count * bonusPerGroup;

    public override float DeckBonus(ChainJudge judge) => 1f;
}
