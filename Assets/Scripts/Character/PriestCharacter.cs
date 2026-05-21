using UnityEngine;

public class PriestCharacter : Character
{
    public override ClassType Type => ClassType.Priest;
    public override Color classColor => Color.green;

    public override void PlaySkillEffect(int chainCount)
    {
        // TODO: chain 1/2/3별 빛 이펙트 prefab 재생
        Debug.Log($"[Priest] SkillEffect chain{chainCount}");
    }

    // Divine Timing: 버리기 횟수가 남아있으면 데미지 +25%
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        if (judge.discardRemaining > 0)
            return Mathf.RoundToInt(damage * 1.25f);
        return damage;
    }
}
