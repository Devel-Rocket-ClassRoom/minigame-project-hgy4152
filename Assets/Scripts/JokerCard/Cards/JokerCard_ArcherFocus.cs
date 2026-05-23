using UnityEngine;

[CreateAssetMenu(fileName = "jc_ArcherFocus", menuName = "ChainKnights/Joker/ArcherFocus")]
public class JokerCard_ArcherFocus : JokerCard
{
    [SerializeField]
    int minGroups = 2;

    [SerializeField]
    int bonusPerGroup = 4;

    public override int GetBonus(ChainJudge judge)
    {
        int count = judge.classDistribution.GetValueOrDefault(ClassType.Archer);
        return count >= minGroups ? count * bonusPerGroup : 0;
    }

    public override float DeckBonus(ChainJudge judge) => 1f;
}
