using UnityEngine;

[CreateAssetMenu(fileName = "jc_GroupBonus", menuName = "ChainKnights/Joker/GroupBonus")]
public class JokerCard_GroupBonus : JokerCard
{
    [SerializeField]
    int bonusPerGroup = 2;

    public override int GetBonus(ChainJudge judge) =>
        (judge.chain1Count + judge.chain2Count + judge.chain3Count) * bonusPerGroup;

    public override float DeckBonus(ChainJudge judge) => 1f;
}
