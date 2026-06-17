using UnityEngine;

[RequireComponent(typeof(WizardCreator))]
[RequireComponent(typeof(Skill_Archon))]
public class ArchonCharacter : Character
{
    public override ClassType Type => ClassType.Wizard;
    public override Color classColor => Color.blue;

    int _stageGroupCounter;
    public override int StackCount => _stageGroupCounter;

    public override void OnStageStart() => _stageGroupCounter = 0;

    public override object CaptureState() => _stageGroupCounter;

    public override void RestoreState(object state)
    {
        if (state is int counter)
            _stageGroupCounter = counter;
    }

    protected override void OnLastChainHitComplete() => StartBreathing();

    // Arcane Surge: 스테이지 누적 본인 그룹 1개당 데미지 +10% (누적)
    public override float GetChainTypeBonus(ChainJudge judge, ChainGroup group) =>
        group.DominantClass == Type ? 0.1f * _stageGroupCounter : 0f;

    public override void ApplyDebuffPassive(ChainJudge judge, ChainGroup group)
    {
        if (!judge.isPreview && group.DominantClass == Type)
            _stageGroupCounter++;
    }
}
