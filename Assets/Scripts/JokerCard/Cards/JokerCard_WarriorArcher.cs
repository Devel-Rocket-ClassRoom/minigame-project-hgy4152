using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "jc_WarriorArcher", menuName = "ChainKnights/Joker/WarriorArcher")]
public class JokerCard_WarriorArcher : JokerCard
{
    [SerializeField]
    int bonusPerGroup = 3;

    public override int GetBonus(ChainJudge judge)
    {
        int warrior = judge.classDistribution.GetValueOrDefault(ClassType.Warrior);
        int archer = judge.classDistribution.GetValueOrDefault(ClassType.Archer);
        return (warrior + archer) * bonusPerGroup;
    }

    public override float DeckBonus(ChainJudge judge) => 1f;
}
