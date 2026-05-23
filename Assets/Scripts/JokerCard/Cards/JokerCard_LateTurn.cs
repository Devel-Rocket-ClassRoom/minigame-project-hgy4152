using UnityEngine;

[CreateAssetMenu(fileName = "jc_LateTurn", menuName = "ChainKnights/Joker/LateTurn")]
public class JokerCard_LateTurn : JokerCard
{
    [SerializeField]
    int turnThreshold = 4;

    [SerializeField]
    float bonus = 1.3f;

    public override int GetBonus(ChainJudge judge) => 0;

    public override float DeckBonus(ChainJudge judge) =>
        judge.turnIndex >= turnThreshold ? bonus : 1f;
}
