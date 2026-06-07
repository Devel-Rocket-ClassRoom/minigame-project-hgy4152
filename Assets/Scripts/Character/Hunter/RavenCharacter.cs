using UnityEngine;

[RequireComponent(typeof(HunterCreator))]
[RequireComponent(typeof(Skill_Raven))]
public class RavenCharacter : Character
{
    public override ClassType Type => ClassType.Hunter;
    public override Color classColor => Color.green;

    int _stageGroupCounter;
    public override int StackCount => _stageGroupCounter;

    public override void OnStageStart() => _stageGroupCounter = 0;

    // Predator: 스테이지 누적 본인 그룹 3번째마다 데미지 +50%
    public override float GetChainTypeBonus(ChainJudge judge, ChainGroup group)
    {
        if (judge.isPreview || group.DominantClass != Type)
            return 0f;
        return _stageGroupCounter + 1 == 3 ? 0.5f : 0f;
    }

    public override void ApplyDebuffPassive(ChainJudge judge, ChainGroup group)
    {
        if (judge.isPreview || group.DominantClass != Type)
            return;
        _stageGroupCounter++;
        if (_stageGroupCounter >= 3)
            _stageGroupCounter = 0;
    }
}
