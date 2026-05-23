using UnityEngine;

[CreateAssetMenu(fileName = "jc_Complete", menuName = "ChainKnights/Joker/Complete")]
public class JokerCard_Complete : JokerCard
{
    [SerializeField]
    float bonus = 1.7f;

    public override int GetBonus(ChainJudge judge) => 0;

    public override float DeckBonus(ChainJudge judge)
    {
        var d = judge.classDistribution;
        bool hasAll =
            d.ContainsKey(ClassType.Warrior)
            && d.ContainsKey(ClassType.Archer)
            && d.ContainsKey(ClassType.Priest);
        return judge.isShiftBlock && hasAll ? bonus : 1f;
    }
}
