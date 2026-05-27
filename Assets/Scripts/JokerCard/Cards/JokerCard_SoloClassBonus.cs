using UnityEngine;

[CreateAssetMenu(menuName = "ChainKnights/JokerCard/SoloClassBonus")]
public class JokerCard_SoloClassBonus : JokerCard
{
    public int bonus;

    public override int GetBonus(ChainJudge judge, ChainGroup group) =>
        judge.classDistribution.Count == 1 ? bonus : 0;

    public override float DeckBonus(ChainJudge judge) => 1f;
}
