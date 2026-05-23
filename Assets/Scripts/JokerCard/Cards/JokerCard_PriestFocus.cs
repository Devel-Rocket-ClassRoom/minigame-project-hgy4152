using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "jc_PriestFocus", menuName = "ChainKnights/Joker/PriestFocus")]
public class JokerCard_PriestFocus : JokerCard
{
    [SerializeField]
    int minGroups = 2;

    [SerializeField]
    int bonusPerGroup = 4;

    public override int GetBonus(ChainJudge judge)
    {
        int count = judge.classDistribution.GetValueOrDefault(ClassType.Priest);
        return count >= minGroups ? count * bonusPerGroup : 0;
    }

    public override float DeckBonus(ChainJudge judge) => 1f;
}
