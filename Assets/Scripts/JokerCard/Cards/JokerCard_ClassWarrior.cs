using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "jc_ClassWarrior", menuName = "ChainKnights/Joker/ClassWarrior")]
public class JokerCard_ClassWarrior : JokerCard
{
    [SerializeField]
    int bonusPerBlock = 2;

    public override int GetBonus(ChainJudge judge) =>
        judge.classDistribution.GetValueOrDefault(ClassType.Warrior) * bonusPerBlock;
}
