using UnityEngine;

[RequireComponent(typeof(WarriorCreator))]
[RequireComponent(typeof(Skill_Cain))]
public class CainCharacter : Character
{
    public override ClassType Type => ClassType.Warrior;
    public override Color classColor => Color.red;

    // 폭주: 3체인 그룹이 3개 이상일 때 데미지 +50%
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        if (judge.chain3Count >= 3)
            return Mathf.RoundToInt(damage * 1.5f);
        return damage;
    }
}
