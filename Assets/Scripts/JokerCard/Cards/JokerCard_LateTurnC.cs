using UnityEngine;

[CreateAssetMenu(fileName = "jc_LateTurnC", menuName = "ChainKnights/Joker/LateTurnC")]
public class JokerCard_LateTurnC : JokerCard
{
    [SerializeField]
    int turnThreshold = 4;

    [SerializeField]
    float bonus = 1.2f;

    public override int GetBonus(ChainJudge judge) => 0;

    public override float DeckBonus(ChainJudge judge) =>
        judge.turnIndex >= turnThreshold ? bonus : 1f;
}
