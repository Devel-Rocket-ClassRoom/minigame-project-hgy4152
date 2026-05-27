using UnityEngine;

[CreateAssetMenu(menuName = "ChainKnights/JokerCard/FirstTurnBonus")]
public class JokerCard_FirstTurnBonus : JokerCard
{
    public int targetTurnIndex;
    public int bonus;

    public override int GetBonus(ChainJudge judge, ChainGroup group) =>
        judge.turnIndex == targetTurnIndex ? bonus : 0;

    public override float DeckBonus(ChainJudge judge) => 1f;
}
