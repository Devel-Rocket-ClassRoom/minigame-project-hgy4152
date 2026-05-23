using UnityEngine;

[CreateAssetMenu(fileName = "jc_Lightning", menuName = "ChainKnights/Joker/Lightning")]
public class JokerCard_Lightning : JokerCard
{
    [SerializeField]
    float timeThreshold = 0.7f;

    [SerializeField]
    float bonus = 1.5f;

    public override int GetBonus(ChainJudge judge) => 0;

    public override float DeckBonus(ChainJudge judge) =>
        judge.remainingTimeRatio > timeThreshold ? bonus : 1f;
}
