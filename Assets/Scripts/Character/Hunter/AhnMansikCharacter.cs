using UnityEngine;

[RequireComponent(typeof(HunterCreator))]
[RequireComponent(typeof(Skill_AhnMansik))]
public class AhnMansikCharacter : Character
{
    public override ClassType Type => ClassType.Hunter;
    public override Color classColor => Color.green;

    int _effectiveChainCount;

    protected override void OnLastChainHitComplete() => StartBreathing();

    // 명령 불복종: 1체인 → 33% 확률로 3체인, 3체인 → 33% 확률로 1체인
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        _effectiveChainCount = group.Length;

        if (group.Length == 1 && Random.value < 0.33f)
            _effectiveChainCount = 3;
        else if (group.Length == 3 && Random.value < 0.33f)
            _effectiveChainCount = 1;

        if (_effectiveChainCount != group.Length)
            damage = Mathf.RoundToInt(damage * (float)_effectiveChainCount / group.Length);

        return damage;
    }

    public override void PlaySkillEffect(
        int chainCount,
        int[] perHitDamages = null,
        EnemyController target = null
    )
    {
        int effective = _effectiveChainCount > 0 ? _effectiveChainCount : chainCount;

        // 체인 수가 변경됐으면 총 데미지를 새 체인 수로 재분배
        if (perHitDamages != null && effective != chainCount)
        {
            int total = 0;
            foreach (int d in perHitDamages)
                total += d;

            var reSplit = new int[effective];
            int weightSum = effective * (effective + 1) / 2;
            int accumulated = 0;
            for (int i = 0; i < effective - 1; i++)
            {
                reSplit[i] = Mathf.RoundToInt(total * (i + 1) / (float)weightSum);
                accumulated += reSplit[i];
            }
            reSplit[effective - 1] = total - accumulated;
            perHitDamages = reSplit;
        }

        base.PlaySkillEffect(effective, perHitDamages, target);
    }
}
