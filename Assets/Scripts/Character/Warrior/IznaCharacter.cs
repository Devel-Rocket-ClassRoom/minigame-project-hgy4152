using UnityEngine;

[RequireComponent(typeof(WarriorCreator))]
[RequireComponent(typeof(Skill_Izna))]
public class IznaCharacter : Character
{
    public override ClassType Type => ClassType.Warrior;
    public override Color classColor => Color.red;

    // 연격: 1·2·3체인 모두 있을 때 데미지 +40%
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        if (judge.chain1Count > 0 && judge.chain2Count > 0 && judge.chain3Count > 0)
            return Mathf.RoundToInt(damage * 1.4f);
        return damage;
    }
}
