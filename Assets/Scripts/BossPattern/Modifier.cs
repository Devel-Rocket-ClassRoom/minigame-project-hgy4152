using UnityEngine;

public abstract class Modifier : ScriptableObject
{
    public string modName;
    public string description;
    public abstract void Apply(ChainJudge judge);
}
