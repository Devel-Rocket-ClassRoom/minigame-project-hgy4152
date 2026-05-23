using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "jc_ClassPriest", menuName = "ChainKnights/Joker/ClassPriest")]
public class JokerCard_ClassPriest : JokerCard
{
    [SerializeField]
    int bonusPerGroup = 2;

    public override int GetBonus(ChainJudge judge) =>
        judge.classDistribution.GetValueOrDefault(ClassType.Priest) * bonusPerGroup;

    public override float DeckBonus(ChainJudge judge) => 1f;
}
