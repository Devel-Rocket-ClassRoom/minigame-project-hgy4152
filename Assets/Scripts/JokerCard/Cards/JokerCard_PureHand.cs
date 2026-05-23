using UnityEngine;

[CreateAssetMenu(fileName = "jc_PureHand", menuName = "ChainKnights/Joker/PureHand")]
public class JokerCard_PureHand : JokerCard
{
    [SerializeField]
    float bonus = 1.3f;

    public override int GetBonus(ChainJudge judge) => 0;

    public override float DeckBonus(ChainJudge judge) => judge.discardUsed == 0 ? bonus : 1f;
}
