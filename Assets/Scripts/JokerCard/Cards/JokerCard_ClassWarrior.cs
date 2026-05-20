using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "jc_ClassWarrior", menuName = "ChainKnights/Joker/ClassWarrior")]
public class JokerCard_ClassWarrior : JokerCard
{
    [SerializeField]
    int bonusPerGroup = 2;

    // 판단용으로 현재 구조 사용. classDistribution의 value 값은 항상 1임
    public override int GetBonus(ChainJudge judge) =>
        judge.classDistribution.GetValueOrDefault(ClassType.Warrior) * bonusPerGroup;

    public override float DeckBonus(ChainJudge judge)
    {
        return 1;
    }
}
