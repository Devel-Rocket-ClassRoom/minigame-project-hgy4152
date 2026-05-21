using UnityEngine;

public class BossPatternSystem : MonoBehaviour
{
    [SerializeField]
    StageManager stageManager;

    BossPattern current;
    int turnIndex;

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
        current = entry.enemyData != null ? entry.enemyData.bossPattern : null;
        turnIndex = 0;
        OnInjected?.Invoke();
    }

    public void ApplyModifiers(ChainJudge judge)
    {
        judge.activeModifiers.Clear();
        judge.bossPattern = current;
        judge.turnIndex = turnIndex;
        if (current == null)
            return;

        foreach (var m in current.passive)
            if (m != null)
                judge.activeModifiers.Add(m);

        if (turnIndex < current.turnModifiers.Length)
        {
            var tm = current.turnModifiers[turnIndex];
            if (tm != null)
                judge.activeModifiers.Add(tm);
        }

        foreach (var m in judge.activeModifiers)
            m.Apply(judge);
    }

    public void Inject(ChainJudge judge)
    {
        ApplyModifiers(judge);
        OnInjected?.Invoke();
    }

    public void AdvanceTurn() => turnIndex++;
}
