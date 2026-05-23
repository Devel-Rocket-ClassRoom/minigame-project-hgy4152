using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "jc_ClassArcher", menuName = "ChainKnights/Joker/ClassArcher")]
public class JokerCard_ClassArcher : JokerCard
{
    [SerializeField]
    int bonusPerGroup = 2;

    public override int GetBonus(ChainJudge judge) =>
        judge.classDistribution.GetValueOrDefault(ClassType.Archer) * bonusPerGroup;

    public override float DeckBonus(ChainJudge judge) => 1f;
}
