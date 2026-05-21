using UnityEngine;

public class WarriorCharacter : Character
{
    public override ClassType Type => ClassType.Warrior;
    public override Color classColor => Color.red;

    public override void PlaySkillEffect(int chainCount)
    {
        // TODO: chain 1/2/3별 검 이펙트 prefab 재생
        Debug.Log($"[Warrior] SkillEffect chain{chainCount}");
    }

    // Chain Mastery: 체인 2 이상이면 데미지 +30%
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        if (group.Length >= 2)
            return Mathf.RoundToInt(damage * 1.3f);
        return damage;
    }
}
