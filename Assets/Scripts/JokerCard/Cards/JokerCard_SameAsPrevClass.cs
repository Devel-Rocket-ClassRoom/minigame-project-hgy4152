using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ChainKnights/JokerCard/SameAsPrevClass")]
public class JokerCard_SameAsPrevClass : JokerCard
{
    public float multiplier = 1.3f;

    public override int GetBonus(ChainJudge judge, ChainGroup group) => 0;

    public override float DeckBonus(ChainJudge judge)
    {
        var cur = judge.classDistribution;
        var prev = judge.prevClassDistribution;
        if (cur.Count == 0 || cur.Count != prev.Count)
            return 1f;
        foreach (var kv in cur)
            if (!prev.TryGetValue(kv.Key, out int v) || v != kv.Value)
                return 1f;
        return multiplier;
    }
}
