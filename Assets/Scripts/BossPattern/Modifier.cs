using UnityEngine;

public abstract class Modifier : ScriptableObject, IIdentifiable
{
    public string id;
    public string modName;
    public string description;
    public Sprite icon;
    public string dialogueKey;
    public GameObject effectPrefab;

    public string Id => id;

    public abstract void Apply(ChainJudge judge);

    public virtual float GetChainBonusPenalty(ChainGroup group) => 0f;

    public virtual float GetClassBonusPenalty(ChainGroup group) => 0f;

    public virtual void PreResolve(BlockManager blockMgr) { }

    public virtual void OnTurnStart(BlockManager blockMgr, DrawPhaseTimer dpt) { }
}
