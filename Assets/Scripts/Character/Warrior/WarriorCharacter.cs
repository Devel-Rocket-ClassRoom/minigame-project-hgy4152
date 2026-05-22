using UnityEngine;

public class WarriorCharacter : Character
{
    public override ClassType Type => ClassType.Warrior;
    public override Color classColor => Color.red;

    // Chain Mastery: 3체인 시 데미지 +30%
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        if (group.Length >= 3)
            return Mathf.RoundToInt(damage * 1.3f);
        return damage;
    }
}
