using UnityEngine;

[RequireComponent(typeof(PriestCreator))]
[RequireComponent(typeof(Skill_Rin))]
public class RinCharacter : Character
{
    [SerializeField]
    float buffPerStack = 0.05f;

    [SerializeField]
    float chain3NextBonus = 0.2f;

    int _idleStacks;
    bool _hasPendingChain3Bonus;

    public override ClassType Type => ClassType.Priest;
    public override Color classColor => Color.green;

    public override void OnStageStart()
    {
        _idleStacks = 0;
        _hasPendingChain3Bonus = false;
    }

    // 급급여율령: 블럭 미사용 턴마다 데미지 증가 스택 누적
    public override void OnTurnProcessed(bool wasThisCharacterUsed)
    {
        if (!wasThisCharacterUsed)
            _idleStacks++;
    }

    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        float mult = 1f + _idleStacks * buffPerStack;
        _idleStacks = 0;

        if (_hasPendingChain3Bonus)
        {
            mult += chain3NextBonus;
            _hasPendingChain3Bonus = false;
        }

        if (group.Length == 3)
            _hasPendingChain3Bonus = true;

        return Mathf.RoundToInt(damage * mult);
    }
}
