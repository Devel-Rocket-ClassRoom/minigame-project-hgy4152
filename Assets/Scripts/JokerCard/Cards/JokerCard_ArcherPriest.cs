using UnityEngine;

[CreateAssetMenu(fileName = "jc_ArcherPriest", menuName = "ChainKnights/Joker/ArcherPriest")]
public class JokerCard_ArcherPriest : JokerCard
{
    [SerializeField]
    int bonusPerGroup = 3;

    public override int GetBonus(ChainJudge judge)
    {
        int archer = judge.classDistribution.GetValueOrDefault(ClassType.Archer);
        int priest = judge.classDistribution.GetValueOrDefault(ClassType.Priest);
        return (archer + priest) * bonusPerGroup;
    }

    public override float DeckBonus(ChainJudge judge) => 1f;
}
