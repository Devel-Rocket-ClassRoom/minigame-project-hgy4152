using UnityEngine;

[CreateAssetMenu(fileName = "jc_Chain3", menuName = "ChainKnights/Joker/Chain3")]
public class JokerCard_Chain3 : JokerCard
{
    [SerializeField]
    int bonusPerGroup = 8;

    public override int GetBonus(ChainJudge judge) => judge.chain3Count * bonusPerGroup;

    public override float DeckBonus(ChainJudge judge) => 1f;
}
