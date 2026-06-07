using UnityEngine;

[RequireComponent(typeof(WarriorCreator))]
[RequireComponent(typeof(Skill_Izna))]
public class IznaCharacter : Character
{
    public override ClassType Type => ClassType.Warrior;
    public override Color classColor => Color.red;

    // 연격: 1·2·3체인 모두 있을 때 데미지 +40%
    public override float GetChainTypeBonus(ChainJudge judge, ChainGroup group) =>
        judge.chain1Count > 0 && judge.chain2Count > 0 && judge.chain3Count > 0 ? 0.4f : 0f;
}
