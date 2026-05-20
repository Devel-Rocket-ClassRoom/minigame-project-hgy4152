using UnityEngine;

[CreateAssetMenu(fileName = "jc_Chain1", menuName = "ChainKnights/Joker/Chain1")]
public class JokerCard_Chain1 : JokerCard
{
    [SerializeField]
    int bonusPerGroup = 3;

    public override float DeckBonus(ChainJudge judge)
    {
        return 1;
    }

    public override int GetBonus(ChainJudge judge) => judge.chain1Count * bonusPerGroup;
}
