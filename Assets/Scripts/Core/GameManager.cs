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
    BossPatternSystem bossPatternSystem;

    [SerializeField]
    JokerRewardUI jokerRewardUI;

    [SerializeField]
    GameObject clearTextObject;

    [SerializeField]
    ModeClearUI modeClearUI;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            jokerRewardUI.Show();
        }
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
        modeClearUI?.Show();
    }

    void Settle()
    {
        var groups = ChainResolver.ResolveChains(blockManager.hand);

        // 전체 그룹 조사
        var judge = new ChainJudge();
        judge.IngestGroups(groups);
        judge.remainingTimeRatio = drawPhaseTimer.RemainingRatio;
        judge.discardRemaining = blockManager.DiscardsRemaining;

        if (bossPatternSystem != null)
        {
            bossPatternSystem.Inject(judge);
            bossPatternSystem.AdvanceTurn();
        }

        StartCoroutine(PlayGroupSequence(groups, judge));
    }

    IEnumerator PlayGroupSequence(List<ChainGroup> groups, ChainJudge judge)
    {
        float deckBonus = 1f;
        if (jokerManager != null)
            foreach (var card in jokerManager.ActiveHand)
                if (card != null)
                    deckBonus *= card.DeckBonus(judge);

        foreach (var group in groups)
        {
            // 애니메이션 재생
            var character = characterSet?.GetCharacter(group.DominantClass);
            character?.PlayAttack();

            // 그룹별 형태 조사 후 조커 카드 보너스 부여
            var groupJudge = new ChainJudge();
            groupJudge.IngestGroup(group);
            int groupBonus = 0;
            if (jokerManager != null)
            {
                foreach (var card in jokerManager.ActiveHand)
                {
                    if (card != null)
                    {
                        groupBonus += card.GetBonus(groupJudge);
                    }
                }
            }

            int groupDmg = CalcGroupDamage(group);
            groupDmg -= judge.bossFlatBonus;
            groupDmg = Mathf.FloorToInt(groupDmg * (2 - judge.bossDamageMultiplier));
            groupDmg += groupBonus;
            groupDmg = character?.ApplyPassive(judge, groupDmg) ?? groupDmg;
            groupDmg = Mathf.FloorToInt(groupDmg * deckBonus);

            boss.TakeDamage(groupDmg);
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
