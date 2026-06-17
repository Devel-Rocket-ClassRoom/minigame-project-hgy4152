using UnityEngine;

[RequireComponent(typeof(WizardCreator))]
[RequireComponent(typeof(Skill_Zyum))]
public class ZyumCharacter : Character
{
    [SerializeField]
    float bonusPerChain3 = 0.1f;

    int _lastChain3Count;
    public override int StackCount => _lastChain3Count;

    Skill_Zyum ZyumSkill => skill as Skill_Zyum;

    public override void OnStageStart() => _lastChain3Count = 0;

    public override object CaptureState() => _lastChain3Count;

    public override void RestoreState(object state)
    {
        if (state is int count)
            _lastChain3Count = count;
    }

    public override ClassType Type => ClassType.Wizard;
    public override Color classColor => Color.magenta;
    protected override bool IsSingleCast => true;

    protected override void OnLastChainHitComplete() => StartBreathing();

    // 전장의 악귀: 핸드 내 3체인이 자신밖에 없을 경우 자신의 모든 공격에 체인 수 비례 추가 데미지
    public override float GetChainTypeBonus(ChainJudge judge, ChainGroup group)
    {
        var byClass = judge.chainCountByClass;
        if (!byClass.TryGetValue(ClassType.Wizard, out var arr))
        {
            ZyumSkill?.SetPassiveActive(false);
            return 0f;
        }

        int myCount = arr[2];
        if (myCount == 0)
        {
            ZyumSkill?.SetPassiveActive(false);
            return 0f;
        }

        foreach (var kv in byClass)
        {
            if (kv.Key != ClassType.Wizard && kv.Value[2] > 0)
            {
                ZyumSkill?.SetPassiveActive(false);
                return 0f;
            }
        }

        if (!judge.isPreview)
            _lastChain3Count = myCount;
        ZyumSkill?.SetPassiveActive(true);
        return bonusPerChain3 * myCount;
    }

    public override void OnTurnSequenceEnd() => _lastChain3Count = 0;
}
