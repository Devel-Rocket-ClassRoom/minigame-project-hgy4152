using UnityEngine;

[CreateAssetMenu(menuName = "ChainKnights/JokerCard/SpeedFlatBonus")]
public class JokerCard_SpeedFlatBonus : JokerCard
{
    public float minRatio = 0.7f;
    public int bonus;

    public override int GetBonus(ChainJudge judge, ChainGroup group) =>
        judge.remainingTimeRatio >= minRatio ? bonus : 0;

    public override float DeckBonus(ChainJudge judge) => 1f;
}
