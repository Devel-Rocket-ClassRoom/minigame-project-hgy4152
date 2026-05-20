using UnityEngine;

[CreateAssetMenu(fileName = "jc_BlockStateShift", menuName = "ChainKnights/Joker/BlockStateShift")]
public class JokerCard_BlockStateShift : JokerCard
{
    [SerializeField]
    float bonus = 1.2f;

    public override int GetBonus(ChainJudge judge)
    {
        return 0;
    }

    public override float DeckBonus(ChainJudge judge) => judge.isShiftBlock ? bonus : 1;
    
}
