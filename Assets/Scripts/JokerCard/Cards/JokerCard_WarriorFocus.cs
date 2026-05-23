using UnityEngine;

[CreateAssetMenu(fileName = "jc_WarriorFocus", menuName = "ChainKnights/Joker/WarriorFocus")]
public class JokerCard_WarriorFocus : JokerCard
{
    [SerializeField]
    int minGroups = 2;

    [SerializeField]
    int bonusPerGroup = 4;

    public override int GetBonus(ChainJudge judge)
    {
        int count = judge.classDistribution.GetValueOrDefault(ClassType.Warrior);
        return count >= minGroups ? count * bonusPerGroup : 0;
    }

    public override float DeckBonus(ChainJudge judge) => 1f;
}
