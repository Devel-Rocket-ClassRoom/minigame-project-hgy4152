using UnityEngine;

[CreateAssetMenu(menuName = "ChainKnights/JokerCard/ClassDiscardBonus")]
public class JokerCard_ClassDiscardBonus : JokerCard
{
    public int perBlock = 1;

    public override int GetBonus(ChainJudge judge, ChainGroup group)
    {
        if (judge.discardsByClass == null)
            return 0;
        judge.discardsByClass.TryGetValue(group.DominantClass, out int count);
        return count * perBlock;
    }

    public override float DeckBonus(ChainJudge judge) => 1f;
}
