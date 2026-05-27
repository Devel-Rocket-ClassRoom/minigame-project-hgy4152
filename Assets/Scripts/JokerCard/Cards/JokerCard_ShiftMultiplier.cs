using UnityEngine;

[CreateAssetMenu(menuName = "ChainKnights/JokerCard/ShiftMultiplier")]
public class JokerCard_ShiftMultiplier : JokerCard
{
    public float multiplier = 1.3f;

    public override int GetBonus(ChainJudge judge, ChainGroup group) => 0;

    public override float DeckBonus(ChainJudge judge) => judge.isShiftBlock ? multiplier : 1f;
}
