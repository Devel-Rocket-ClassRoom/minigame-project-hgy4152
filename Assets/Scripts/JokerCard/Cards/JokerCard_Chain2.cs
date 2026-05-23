using UnityEngine;

[CreateAssetMenu(fileName = "jc_Chain2", menuName = "ChainKnights/Joker/Chain2")]
public class JokerCard_Chain2 : JokerCard
{
    [SerializeField]
    int bonusPerGroup = 5;

    public override int GetBonus(ChainJudge judge) => judge.chain2Count * bonusPerGroup;

    public override float DeckBonus(ChainJudge judge) => 1f;
}
