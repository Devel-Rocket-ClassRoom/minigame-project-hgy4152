using System.Collections.Generic;
using UnityEngine;

public class BossPatternSystem : MonoBehaviour
{
    [SerializeField]
    StageManager stageManager;

    BossPattern current;
    int turnIndex;

    int[] _prevChainCounts = new int[3];
    Dictionary<ClassType, int> _prevClassDist = new();
    int[] _prevChainSequence;

    public BossPattern Current => current;
    public int TurnIndex => turnIndex;
    public event System.Action OnInjected;

    void OnEnable()
    {
        stageManager.OnStageStart += HandleStageStart;
    }

    void OnDisable()
    {
        stageManager.OnStageStart -= HandleStageStart;
    }

    void HandleStageStart(StageManager.StageEntry entry)
    {
        current = entry.bossData?.bossPattern ?? entry.enemyData?.bossPattern;
        turnIndex = 0;
        _prevChainCounts = new int[3];
        _prevClassDist = new Dictionary<ClassType, int>();
        _prevChainSequence = null;
        OnInjected?.Invoke();
    }

    IEnumerable<Modifier> GetActiveModifiers()
    {
        if (current == null)
            yield break;
        foreach (var m in current.passive)
            if (m != null)
                yield return m;
        if (turnIndex < current.turnModifiers.Length)
        {
            var tm = current.turnModifiers[turnIndex];
            if (tm != null)
                yield return tm;
        }
    }

    public void ApplyModifiers(ChainJudge judge)
    {
        judge.activeModifiers.Clear();
        judge.bossPattern = current;
        judge.turnIndex = turnIndex;
        if (current == null)
            return;

        foreach (var m in GetActiveModifiers())
        {
            judge.activeModifiers.Add(m);
            m.Apply(judge);
        }
    }

    public void ApplyPreResolve(BlockManager blockMgr)
    {
        if (current == null)
            return;
        foreach (var m in GetActiveModifiers())
            m.PreResolve(blockMgr);
    }

    public void ApplyTurnStart(BlockManager blockMgr, DrawPhaseTimer dpt)
    {
        if (current == null)
            return;
        foreach (var m in GetActiveModifiers())
            m.OnTurnStart(blockMgr, dpt);
    }

    public void PopulatePrevState(ChainJudge judge)
    {
        for (int i = 0; i < 3; i++)
            judge.prevChainCounts[i] = _prevChainCounts[i];
        foreach (var kv in _prevClassDist)
            judge.prevClassDistribution[kv.Key] = kv.Value;
        judge.prevChainSequence = _prevChainSequence;
    }

    public void SnapshotForNextTurn(ChainJudge judge)
    {
        _prevChainCounts = new[] { judge.chain1Count, judge.chain2Count, judge.chain3Count };
        _prevClassDist = new Dictionary<ClassType, int>(judge.classDistribution);
        _prevChainSequence = judge.chainSequence;
    }

    public void Inject(ChainJudge judge)
    {
        ApplyModifiers(judge);
        OnInjected?.Invoke();
    }

    public void AdvanceTurn() => turnIndex++;
}
