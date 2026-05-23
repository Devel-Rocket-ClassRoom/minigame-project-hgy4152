using UnityEngine;

[CreateAssetMenu(fileName = "jc_Chain23", menuName = "ChainKnights/Joker/Chain23")]
public class JokerCard_Chain23 : JokerCard
{
    [SerializeField]
    int bonusPerGroup = 4;

    public override int GetBonus(ChainJudge judge) =>
        (judge.chain2Count + judge.chain3Count) * bonusPerGroup;

    public override float DeckBonus(ChainJudge judge) => 1f;
}
