using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    DrawPhaseTimer drawPhaseTimer;

    [SerializeField]
    BlockManager blockManager;

    [SerializeField]
    EnemyController boss;

    [SerializeField]
    JokerManager jokerManager;

    [SerializeField]
    CharacterSet characterSet;

    [SerializeField]
    StageManager stageManager;

    [SerializeField]
    JokerRewardUI jokerRewardUI;

    [SerializeField]
    GameObject clearTextObject;

    [SerializeField]
    float stageClearDisplayDuration = 1.5f;

    [SerializeField]
    float perGroupDelay = 0.4f;

    bool _stageClearPending;

    public bool IsPaused { get; private set; }

    void OnEnable()
    {
        drawPhaseTimer.OnPhaseEnded += Settle;
        stageManager.OnStageClear += HandleStageClear;
        stageManager.OnAllStagesCleared += HandleAllStagesCleared;
    }

    void OnDisable()
    {
        drawPhaseTimer.OnPhaseEnded -= Settle;
        stageManager.OnStageClear -= HandleStageClear;
        stageManager.OnAllStagesCleared -= HandleAllStagesCleared;
    }

    void Start()
    {
        stageManager.StartStage();
    }

    public void SetPaused(bool paused) => IsPaused = paused;

    public void BeginBattle()
    {
        SetPaused(false);
        drawPhaseTimer.StartDrawPhase();
    }

    void HandleStageClear(StageManager.StageEntry entry)
    {
        drawPhaseTimer.StopDrawPhase();
        SetPaused(true);
        _stageClearPending = true;
    }

    void HandleAllStagesCleared()
    {
        drawPhaseTimer.StopDrawPhase();
        if (clearTextObject != null)
            clearTextObject.SetActive(true);
    }

    void Settle()
    {
        var groups = ChainResolver.ResolveChains(blockManager.hand);

        var judge = new ChainJudge();
        judge.IngestGroups(groups);
        judge.remainingTimeRatio = drawPhaseTimer.RemainingRatio;
        judge.discardRemaining = blockManager.DiscardsRemaining;

        int jokerBonus = 0;
        if (jokerManager != null)
            foreach (var card in jokerManager.ActiveHand)
                if (card != null)
                    jokerBonus += card.GetBonus(judge);

        StartCoroutine(PlayGroupSequence(groups, judge, jokerBonus));
    }

    IEnumerator PlayGroupSequence(List<ChainGroup> groups, ChainJudge judge, int jokerBonus)
    {
        int totalGroups = Mathf.Max(1, groups.Count);
        foreach (var group in groups)
        {
            var character = characterSet?.GetCharacter(group.DominantClass);
            character?.PlayAttack();

            int groupDmg = CalcGroupDamage(group);
            groupDmg += jokerBonus / totalGroups;
            groupDmg = character?.ApplyPassive(judge, groupDmg) ?? groupDmg;

            boss.TakeDamage(groupDmg);
            Debug.Log($"[GameManager] Group {group.DominantClass} x{group.Length}: {groupDmg} dmg");

            blockManager.RemoveGroup(group);
            yield return new WaitForSeconds(perGroupDelay);
        }

        if (_stageClearPending)
        {
            _stageClearPending = false;
            yield return StartCoroutine(ShowStageClear());
            jokerRewardUI.Show();
        }
        else if (boss.IsAlive)
        {
            drawPhaseTimer.StartDrawPhase();
        }
    }

    IEnumerator ShowStageClear()
    {
        if (clearTextObject != null)
            clearTextObject.SetActive(true);
        yield return new WaitForSeconds(stageClearDisplayDuration);
        if (clearTextObject != null)
            clearTextObject.SetActive(false);
    }

    int CalcGroupDamage(ChainGroup group)
    {
        float chainMul = group.Length switch
        {
            1 => 1.1f,
            2 => 1.2f,
            3 => 1.3f,
            _ => 1f,
        };
        return (int)(chainMul * group.Blocks[0].data.attackPower * group.Length);
    }
}
