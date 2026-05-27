using UnityEngine;

public abstract class Modifier : ScriptableObject, IIdentifiable
{
    public string id;
    public string modName;
    public string description;

    public string Id => id;

    public abstract void Apply(ChainJudge judge);

    public virtual void PreResolve(BlockManager blockMgr) { }

    public virtual void OnTurnStart(BlockManager blockMgr, DrawPhaseTimer dpt) { }
}
