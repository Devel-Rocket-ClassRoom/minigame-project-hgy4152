using UnityEngine;

[CreateAssetMenu(fileName = "jc_EarlyTurn", menuName = "ChainKnights/Joker/EarlyTurn")]
public class JokerCard_EarlyTurn : JokerCard
{
    [SerializeField]
    float bonus = 1.2f;

    public override int GetBonus(ChainJudge judge) => 0;

    public override float DeckBonus(ChainJudge judge) => judge.turnIndex == 0 ? bonus : 1f;
}
