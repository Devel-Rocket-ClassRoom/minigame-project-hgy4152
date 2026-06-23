using UnityEngine;

[RequireComponent(typeof(PriestCreator))]
[RequireComponent(typeof(Skill_Beatrice))]
public class BeatriceCharacter : Character
{
    public override ClassType Type => ClassType.Priest;
    public override Color classColor => Color.green;

    private int _stackCount;
    public override int StackCount => _stackCount;

    public override void OnStageStart() => _stackCount = 0;

    public override object CaptureState() => _stackCount;

    public override void RestoreState(object state)
    {
        if (state is int count)
            _stackCount = count;
    }

    protected override void OnLastChainHitComplete() => StartBreathing();

    // Sacrifice: 자기 클래스 블럭을 버린 횟수당 데미지 +1%
    public override float GetClassTypeBonus(ChainJudge judge, ChainGroup group)
    {
        int classDiscards = 0;
        judge.stageDiscardsByClass?.TryGetValue(Type, out _stackCount);
        judge.discardsByClass?.TryGetValue(Type, out classDiscards);
        return classDiscards * 0.01f;
    }
}
