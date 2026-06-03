using UnityEngine;

[RequireComponent(typeof(PriestCreator))]
[RequireComponent(typeof(Skill_Selmu))]
public class SelmuCharacter : Character
{
    [SerializeField]
    float accumulationRatio = 0.3f;

    int _accumulatedBonus;

    Skill_Selmu SelmuSkill => skill as Skill_Selmu;

    public override ClassType Type => ClassType.Priest;
    public override Color classColor => Color.green;

    public override void OnStageStart()
    {
        _accumulatedBonus = 0;
        SelmuSkill?.SpawnTotemBehind();
    }

    // 성소: 파티원 피해 감소분의 일정 비율 누적
    public override void OnAnyGroupDamageApplied(int rawDamage, int finalDamage)
    {
        int reduced = rawDamage - finalDamage;
        if (reduced > 0)
            _accumulatedBonus += Mathf.RoundToInt(reduced * accumulationRatio);

        SelmuSkill?.SetTotemActive(_accumulatedBonus > 0);
    }

    // 자신 블럭 사용 시 누적 보너스 소비
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        int bonus = _accumulatedBonus;
        _accumulatedBonus = 0;
        SelmuSkill?.SetPassivePSActive(false);
        SelmuSkill?.PlayConsumeEffect(_targetPos, scaleFactor);
        return damage + bonus;
    }
}
