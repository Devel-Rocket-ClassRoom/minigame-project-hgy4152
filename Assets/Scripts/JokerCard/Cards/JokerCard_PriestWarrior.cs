using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "jc_PriestWarrior", menuName = "ChainKnights/Joker/PriestWarrior")]
public class JokerCard_PriestWarrior : JokerCard
{
    [SerializeField]
    int bonusPerGroup = 3;

    public override int GetBonus(ChainJudge judge)
    {
        int priest = judge.classDistribution.GetValueOrDefault(ClassType.Priest);
        int warrior = judge.classDistribution.GetValueOrDefault(ClassType.Warrior);
        return (priest + warrior) * bonusPerGroup;
    }

    public override float DeckBonus(ChainJudge judge) => 1f;
}
