using UnityEngine;

[RequireComponent(typeof(HunterCreator))]
[RequireComponent(typeof(Skill_Mkq))]
public class MkqCharacter : Character
{
    public override ClassType Type => ClassType.Hunter;
    public override Color classColor => Color.cyan;

    Skill_Mkq MkqSkill => skill as Skill_Mkq;

    public override void OnStageStart() => MkqSkill?.SetBonusMode(false);

    // 아티피셜 에이전트: 1=2=3체인 횟수 모두 동일 시 추가공격 4회
    public override int GetBonusAttackCount(ChainJudge judge, ChainGroup group)
    {
        MkqSkill?.SetBonusMode(false);
        int c1 = judge.chain1Count,
            c2 = judge.chain2Count,
            c3 = judge.chain3Count;
        if (c1 > 0 && c1 == c2 && c2 == c3)
        {
            MkqSkill?.SetBonusMode(true);
            return 4;
        }
        return 0;
    }
}
