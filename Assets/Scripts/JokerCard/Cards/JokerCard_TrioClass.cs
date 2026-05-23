using UnityEngine;

[CreateAssetMenu(fileName = "jc_TrioClass", menuName = "ChainKnights/Joker/TrioClass")]
public class JokerCard_TrioClass : JokerCard
{
    [SerializeField]
    float bonus = 1.5f;

    public override int GetBonus(ChainJudge judge) => 0;

    public override float DeckBonus(ChainJudge judge)
    {
        var d = judge.classDistribution;
        bool hasAll =
            d.ContainsKey(ClassType.Warrior)
            && d.ContainsKey(ClassType.Archer)
            && d.ContainsKey(ClassType.Priest);
        return hasAll ? bonus : 1f;
    }
}
