using UnityEngine;

[CreateAssetMenu(menuName = "ChainKnights/JokerCard/ClassDamage")]
public class JokerCard_ClassDamage : JokerCard
{
    public ClassType targetClass;
    public int bonus;

    public override int GetBonus(ChainJudge judge, ChainGroup group) =>
        group.DominantClass == targetClass ? bonus : 0;

    public override float DeckBonus(ChainJudge judge) => 1f;
}
