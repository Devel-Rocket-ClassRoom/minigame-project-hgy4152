using UnityEngine;

[RequireComponent(typeof(WizardCreator))]
[RequireComponent(typeof(Skill_Archon))]
public class ArchonCharacter : Character
{
    public override ClassType Type => ClassType.Wizard;
    public override Color classColor => Color.blue;

    int _stageGroupCounter;
    public int StackCount => _stageGroupCounter;

    public override void OnStageStart() => _stageGroupCounter = 0;

    // Arcane Surge: 스테이지 누적 본인 그룹 1개당 데미지 +10% (누적)
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        float mult = 1f + 0.10f * _stageGroupCounter;
        _stageGroupCounter++;
        return Mathf.RoundToInt(damage * mult);
    }
}
