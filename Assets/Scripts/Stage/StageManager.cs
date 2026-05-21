using System;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Serializable]
    public struct StageEntry
    {
        public int chapter;
        public int stage;
        public bool isBoss;
        public EnemyData enemyData;
    }

    [SerializeField]
    StageEntry[] stages = new StageEntry[9];

    [SerializeField]
    EnemyController enemy;

    int currentIndex;

    public StageEntry Current => stages[currentIndex];
    public bool IsLastStage => currentIndex >= stages.Length - 1;

    public event Action<StageEntry> OnStageStart;
    public event Action<StageEntry> OnStageClear;
    public event Action OnAllStagesCleared;

    void OnEnable()
    {
        if (enemy != null)
            enemy.OnDefeated += HandleEnemyDefeated;
    }

    void OnDisable()
    {
        if (enemy != null)
            enemy.OnDefeated -= HandleEnemyDefeated;
    }


    public void StartStage()
    {
        var entry = stages[currentIndex];
        enemy.Init(entry.enemyData);
        OnStageStart?.Invoke(entry);
    }

    void HandleEnemyDefeated()
    {
        if (IsLastStage)
            OnAllStagesCleared?.Invoke();
        else
            OnStageClear?.Invoke(stages[currentIndex]);
    }

    public void AdvanceToNext()
    {
        currentIndex++;
        if (currentIndex >= stages.Length)
        {
            OnAllStagesCleared?.Invoke();
            return;
        }
        StartStage();
    }
}
