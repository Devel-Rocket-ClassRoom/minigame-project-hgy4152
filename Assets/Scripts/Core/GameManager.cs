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
    StageIntroUI stageIntroUI;

    [SerializeField]
    GameObject clearTextObject;

    [SerializeField]
    ModeClearUI modeClearUI;

    [SerializeField]
    int maxTurns = 5;

    [SerializeField]
    float stageClearDisplayDuration = 1.5f;

    [SerializeField]
    float perGroupDelay = 0.4f;

    bool _stageClearPending;
    bool _jokerRewardPending;
    int _currentTurn;

    public bool IsPaused { get; private set; }
    public CharacterSet CharacterSet => characterSet;
    public JokerManager JokerManager => jokerManager;

    void OnEnable()
    {
        drawPhaseTimer.OnPhaseEnded += Settle;
        stageManager.OnStageClear += HandleStageClear;
        stageManager.OnAllStagesCleared += HandleAllStagesCleared;
        stageManager.OnStageStart += HandleStageStart;
    }

    void OnDisable()
    {
        drawPhaseTimer.OnPhaseEnded -= Settle;
        stageManager.OnStageClear -= HandleStageClear;
        stageManager.OnAllStagesCleared -= HandleAllStagesCleared;
        stageManager.OnStageStart -= HandleStageStart;
    }

    void Start()
    {
        _jokerRewardPending = true;
        stageManager.StartStage();
    }

    private void Update()
    {
        // 치트키
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // 조커 획득
            jokerRewardUI.Show();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // 즉시 모드 클리어
            HandleAllStagesCleared();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // 즉시 스테이지 클리어
            // 스테이지 클리어 시 활성화 될 것들 확인
            drawPhaseTimer.StopDrawPhase();
            SetPaused(true);
            _jokerRewardPending = true;
            StartCoroutine(CheatStageClear());
        }
    }

    public void SetPaused(bool paused) => IsPaused = paused;

    public void BeginBattle()
    {
        SetPaused(false);
        StartCoroutine(StartTurnRoutine());
    }

    void HandleStageStart(StageManager.StageEntry entry)
    {
        _currentTurn = 0;
    }

    IEnumerator StartTurnRoutine()
    {
        _currentTurn++;
        if (stageIntroUI != null)
            yield return StartCoroutine(stageIntroUI.ShowTurnRoutine(_currentTurn, maxTurns));
        drawPhaseTimer.StartDrawPhase();
    }

    void HandleStageClear(StageManager.StageEntry entry)
    {
        drawPhaseTimer.StopDrawPhase();
        SetPaused(true);
        _stageClearPending = true;
        _jokerRewardPending = true;
    }

    void HandleAllStagesCleared()
    {
        drawPhaseTimer.StopDrawPhase();
        modeClearUI?.Show(this, Color.green);
    }

    public void OnStageIntroComplete()
    {
        if (_jokerRewardPending)
        {
            _jokerRewardPending = false;
            jokerRewardUI.Show();
        }
        else
        {
            BeginBattle();
        }
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
            groupDmg += groupBonus;
            groupDmg = Mathf.FloorToInt(groupDmg * (2 - judge.bossDamageMultiplier));
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
            stageManager.AdvanceToNext();
        }
        else if (boss.IsAlive)
        {
            if (_currentTurn >= maxTurns)
                modeClearUI?.Show(this, Color.red, "게임 오버");
            else
                StartCoroutine(StartTurnRoutine());
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

    IEnumerator CheatStageClear()
    {
        yield return StartCoroutine(ShowStageClear());
        stageManager.AdvanceToNext();
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
