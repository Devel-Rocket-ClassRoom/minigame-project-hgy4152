using UnityEngine;

[CreateAssetMenu(fileName = "jc_SpeedBonus", menuName = "ChainKnights/Joker/SpeedBonus")]
public class JokerCard_SpeedBonus : JokerCard
{
    [SerializeField]
    float timeThreshold = 0.5f;

    [SerializeField]
    float bonus = 1.3f;

    public override int GetBonus(ChainJudge judge) => 0;

    public override float DeckBonus(ChainJudge judge) =>
        judge.remainingTimeRatio > timeThreshold ? bonus : 1f;
}
