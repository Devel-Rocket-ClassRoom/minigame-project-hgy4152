using System;
using UnityEngine;

public enum UnlockConditionType
{
    DefeatEnemy,
    DefeatBoss,
    ClearAdventure,
    ClearWithCharacter,
}

[Serializable]
public class UnlockCondition
{
    public UnlockConditionType type;
    public string targetId;
}
