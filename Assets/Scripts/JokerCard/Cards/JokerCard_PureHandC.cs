using UnityEngine;

[CreateAssetMenu(fileName = "jc_PureHandC", menuName = "ChainKnights/Joker/PureHandC")]
public class JokerCard_PureHandC : JokerCard
{
    [SerializeField]
    float bonus = 1.15f;

    public override int GetBonus(ChainJudge judge) => 0;

    public override float DeckBonus(ChainJudge judge) => judge.discardUsed == 0 ? bonus : 1f;
}
