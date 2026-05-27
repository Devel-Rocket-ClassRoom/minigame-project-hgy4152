using UnityEngine;

[CreateAssetMenu(menuName = "ChainKnights/JokerCard/ChainDamage")]
public class JokerCard_ChainDamage : JokerCard
{
    public int chainLength;
    public int bonus;

    public override int GetBonus(ChainJudge judge, ChainGroup group) =>
        group.Length == chainLength ? bonus : 0;

    public override float DeckBonus(ChainJudge judge) => 1f;
}
