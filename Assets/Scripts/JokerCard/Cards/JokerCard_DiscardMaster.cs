using UnityEngine;

[CreateAssetMenu(fileName = "jc_DiscardMaster", menuName = "ChainKnights/Joker/DiscardMaster")]
public class JokerCard_DiscardMaster : JokerCard
{
    [SerializeField]
    int minDiscards = 2;

    [SerializeField]
    float bonus = 1.3f;

    public override int GetBonus(ChainJudge judge) => 0;

    public override float DeckBonus(ChainJudge judge) =>
        judge.discardUsed >= minDiscards ? bonus : 1f;
}
