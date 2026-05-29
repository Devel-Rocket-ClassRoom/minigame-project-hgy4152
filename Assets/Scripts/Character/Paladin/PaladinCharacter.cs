using UnityEngine;

public class PaladinCharacter : Character
{
    public override ClassType Type => ClassType.Paladin;
    public override Color classColor => Color.white;

    bool _stageImmunityUsed;

    public override void OnStageStart() => _stageImmunityUsed = false;

    // Divine Shield: 스테이지 누적 디스카드 20개 도달 시 이번 턴 디버프 전부 무효화 (스테이지당 1회)
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        if (!_stageImmunityUsed && judge.stageDiscardsUsed >= 20)
        {
            judge.ClearDebuffs();
            _stageImmunityUsed = true;
            var prefab = (skill as Skill_Victor_HolySlash)?.passiveEffectPrefab;
            if (prefab != null)
            {
                var go = Instantiate(prefab, transform.position, Quaternion.identity);
                Destroy(go, 0.5f);
            }
        }
        return damage;
    }
}
