using System;
using UnityEngine;

public enum UnlockConditionType
{
    AdventureClear,
    BossModeClear,
    Chain1Used,
    Chain2Used,
    Chain3Used,
    BlocksDiscarded,
    UnlockedJokerCount,
    UnlockedCharacterCount,
    ClearWithClass,
}

[Serializable]
public class UnlockCondition
{
    public UnlockConditionType type;
    public int count;
    public ClassType classType; // ClearWithClass 전용
}
