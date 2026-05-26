using UnityEngine;

[CreateAssetMenu(fileName = "jc_DiscardBonus", menuName = "ChainKnights/Joker/DiscardBonus")]
public class JokerCard_DiscardBonus : JokerCard
{
    [SerializeField]
    int bonusPerDiscard = 4;

    public override int GetBonus(ChainJudge judge) =>
        judge.discardBonusDisabled ? 0 : judge.discardUsed * bonusPerDiscard;

    public override float DeckBonus(ChainJudge judge) => 1f;
}
