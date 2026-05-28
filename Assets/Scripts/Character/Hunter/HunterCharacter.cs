using UnityEngine;

public class HunterCharacter : Character
{
    public override ClassType Type => ClassType.Hunter;
    public override Color classColor => Color.green;

    int _stageGroupCounter;

    public override void OnStageStart() => _stageGroupCounter = 0;

    // Predator: 스테이지 누적 본인 그룹 3번째마다 데미지 +50%
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        _stageGroupCounter++;
        if (_stageGroupCounter % 3 == 0)
            return Mathf.RoundToInt(damage * 1.5f);
        return damage;
    }
}
