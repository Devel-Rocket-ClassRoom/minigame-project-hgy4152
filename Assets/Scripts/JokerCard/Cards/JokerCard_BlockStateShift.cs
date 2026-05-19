using UnityEngine;

[CreateAssetMenu(fileName = "jc_BlockStateShift", menuName = "ChainKnights/Joker/BlockStateShift")]
public class JokerCard_BlockStateShift : JokerCard
{
    [SerializeField]
    int bonus = 10;

    public override int GetBonus(ChainJudge judge) => judge.isShiftBlock ? bonus : 0;
}
