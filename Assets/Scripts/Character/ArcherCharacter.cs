using UnityEngine;

public class ArcherCharacter : Character
{
    public override ClassType Type => ClassType.Archer;
    public override Color classColor => Color.yellow;

    public override void PlaySkillEffect(int chainCount)
    {
        // TODO: chain 1/2/3별 화살 이펙트 prefab 재생
        Debug.Log($"[Archer] SkillEffect chain{chainCount}");
    }

    // Penetrating Shot: 보스 flat 방어력 무시 (이미 차감된 bossFlatBonus를 되돌림)
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        return damage + judge.bossFlatBonus;
    }
}
