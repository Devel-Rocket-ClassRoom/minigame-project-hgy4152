using UnityEngine;

public abstract class Modifier : ScriptableObject
{
    public string modName;
    public string description;

    public abstract void Apply(ChainJudge judge);

    public virtual void PreResolve(BlockManager blockMgr) { }

    public virtual void OnTurnStart(BlockManager blockMgr, DrawPhaseTimer dpt) { }
}
