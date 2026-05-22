using UnityEngine;

public class ArcherCharacter : Character
{
    public override ClassType Type => ClassType.Archer;
    public override Color classColor => Color.yellow;

    // Snipe: 적 최대 체력의 5% 추가 데미지
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        return damage + Mathf.RoundToInt(judge.bossMaxHp * 0.05f);
    }
}
