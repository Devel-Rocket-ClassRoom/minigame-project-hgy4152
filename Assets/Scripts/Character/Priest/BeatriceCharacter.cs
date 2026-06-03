using UnityEngine;

[RequireComponent(typeof(PriestCreator))]
[RequireComponent(typeof(Skill_Beatrice))]
public class BeatriceCharacter : Character
{
    public override ClassType Type => ClassType.Priest;
    public override Color classColor => Color.green;

    // Sacrifice: 버린 블럭 횟수당 데미지 +1%
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        return Mathf.RoundToInt(damage * (1f + judge.discardUsed * 0.01f));
    }
}
