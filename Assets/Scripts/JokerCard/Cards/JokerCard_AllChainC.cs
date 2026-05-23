using UnityEngine;

[CreateAssetMenu(fileName = "jc_AllChainC", menuName = "ChainKnights/Joker/AllChainC")]
public class JokerCard_AllChainC : JokerCard
{
    [SerializeField]
    int flatBonus = 15;

    public override int GetBonus(ChainJudge judge) =>
        judge.chain1Count > 0 && judge.chain2Count > 0 && judge.chain3Count > 0 ? flatBonus : 0;

    public override float DeckBonus(ChainJudge judge) => 1f;
}
