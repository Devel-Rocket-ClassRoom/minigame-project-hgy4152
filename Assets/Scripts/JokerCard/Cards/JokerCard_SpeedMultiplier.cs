using UnityEngine;

[CreateAssetMenu(menuName = "ChainKnights/JokerCard/SpeedMultiplier")]
public class JokerCard_SpeedMultiplier : JokerCard
{
    public float minRatio = 0.7f;
    public float multiplier = 1.3f;

    public override int GetBonus(ChainJudge judge, ChainGroup group) => 0;

    public override float DeckBonus(ChainJudge judge) =>
        judge.remainingTimeRatio >= minRatio ? multiplier : 1f;
}
