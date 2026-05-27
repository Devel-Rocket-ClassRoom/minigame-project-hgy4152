using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ChainKnights/JokerCard/PrevChainSequence")]
public class JokerCard_PrevChainSequence : JokerCard
{
    public int bonus;

    public override int GetBonus(ChainJudge judge, ChainGroup group)
    {
        var cur = judge.chainSequence;
        var prev = judge.prevChainSequence;
        if (cur == null || prev == null || cur.Length == 0 || cur.Length != prev.Length)
            return 0;
        return cur.SequenceEqual(prev) ? bonus : 0;
    }

    public override float DeckBonus(ChainJudge judge) => 1f;
}
