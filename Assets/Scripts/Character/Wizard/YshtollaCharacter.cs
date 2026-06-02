using UnityEngine;

[RequireComponent(typeof(WizardCreator))]
[RequireComponent(typeof(Skill_Yshtolla))]
public class YshtollaCharacter : Character
{
    [SerializeField]
    float doubleBonus = 0.5f;

    Skill_Yshtolla YshtollaSkill => skill as Skill_Yshtolla;

    public override ClassType Type => ClassType.Wizard;
    public override Color classColor => Color.magenta;

    // 더블 캐스팅: 이번 턴 위자드 동일 체인 수 그룹이 정확히 2개일 때 데미지 증가
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        bool doubleActive = false;
        if (judge.chainCountByClass.TryGetValue(ClassType.Wizard, out var arr))
            doubleActive = arr[0] == 2 || arr[1] == 2 || arr[2] == 2;

        YshtollaSkill?.SetPassiveActive(doubleActive);

        if (doubleActive)
            return Mathf.RoundToInt(damage * (1f + doubleBonus));
        return damage;
    }
}
