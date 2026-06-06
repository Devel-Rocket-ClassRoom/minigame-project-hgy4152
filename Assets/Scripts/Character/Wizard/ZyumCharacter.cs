using UnityEngine;

[RequireComponent(typeof(WizardCreator))]
[RequireComponent(typeof(Skill_Zyum))]
public class ZyumCharacter : Character
{
    [SerializeField]
    float bonusPerChain3 = 0.1f;

    Skill_Zyum ZyumSkill => skill as Skill_Zyum;

    public override ClassType Type => ClassType.Wizard;
    public override Color classColor => Color.magenta;
    protected override bool IsSingleCast => true;

    protected override void OnLastChainHitComplete() => StartBreathing();

    // 전장의 악귀: 파티 내 위자드가 3체인 최다 사용 시 체인 수 비례 추가 데미지
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        var byClass = judge.chainCountByClass;
        if (!byClass.TryGetValue(ClassType.Wizard, out var arr))
        {
            ZyumSkill?.SetPassiveActive(false);
            return damage;
        }

        int myCount = arr[2];
        if (myCount == 0)
        {
            ZyumSkill?.SetPassiveActive(false);
            return damage;
        }

        foreach (var kv in byClass)
        {
            if (kv.Key != ClassType.Wizard && kv.Value[2] >= myCount)
            {
                ZyumSkill?.SetPassiveActive(false);
                return damage;
            }
        }

        ZyumSkill?.SetPassiveActive(true);
        return damage + Mathf.RoundToInt(damage * bonusPerChain3 * myCount);
    }
}
