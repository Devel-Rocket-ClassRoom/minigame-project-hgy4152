using UnityEngine;

[RequireComponent(typeof(PriestCreator))]
[RequireComponent(typeof(Skill_Selmu))]
public class SelmuCharacter : Character
{
    [SerializeField]
    float accumulationRatio = 0.3f;

    int _accumulatedBonus;
    bool _thresholdReached;
    bool _pendingConsume;

    Skill_Selmu SelmuSkill => skill as Skill_Selmu;

    public override ClassType Type => ClassType.Priest;
    public override Color classColor => Color.green;

    public override void OnStageStart()
    {
        _accumulatedBonus = 0;
        _thresholdReached = false;
        _pendingConsume = false;
        SelmuSkill?.SpawnTotemBehind();
    }

    // 핸드 플레이 공격마다 누적 (CalcDamages 단계에서 호출)
    public override void OnAnyGroupDamageApplied(int rawDamage, int finalDamage)
    {
        if (_pendingConsume)
            return;

        _accumulatedBonus += Mathf.RoundToInt(finalDamage * accumulationRatio);
        SelmuSkill?.PlayAccumulationPS();

        if (!_thresholdReached && _accumulatedBonus >= 1000)
        {
            _thresholdReached = true;
            SelmuSkill?.SetThresholdEffectActive(true);
        }
    }

    // 턴 시퀀스 종료: 1000 이상이면 다음 턴 소비 대기로 이관
    public override void OnTurnSequenceEnd()
    {
        if (_thresholdReached)
        {
            _pendingConsume = true;
            _thresholdReached = false;
        }
    }

    // 다음 턴 첫 공격 시 소비
    public override void OnAnyGroupAttackStart(ChainGroup group, EnemyController target)
    {
        if (!_pendingConsume)
            return;

        _pendingConsume = false;
        int damage = _accumulatedBonus;
        _accumulatedBonus = 0;
        SelmuSkill?.SetThresholdEffectActive(false);
        SelmuSkill?.PlayConsumeEffect(target.transform.position, 1f);
        target.TakeDamage(damage, classColor);
    }
}
