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
    SaveManager saveManager;

    [SerializeField]
    SaveSlotPickerUI saveSlotPickerUI;

    [SerializeField]
    ConfirmDialogUI confirmDialog;

    [SerializeField]
    int maxTurns = 5;

    [SerializeField]
    float stageClearDisplayDuration = 1.5f;

    [SerializeField]
    float perGroupDelay = 0.4f;

    [SerializeField]
    float highlightDuration = 0.2f;

    [SerializeField]
    float highlightScale = 1.2f;

    bool _stageClearPending;
    bool _jokerRewardPending;
    bool _isAdventureMode;
    bool _isBossPlay;
    int _currentTurn;
    int _turnDamageTotal;
    int _handPlaysThisPhase;

    const int MaxHandsPerPhase = 5;

    public bool IsPaused { get; private set; }
    public int TurnDamageTotal => _turnDamageTotal;
    public event System.Action<int> OnTurnDamageChanged;
    public CharacterSet CharacterSet => characterSet;
    public JokerManager JokerManager => jokerManager;

    void OnEnable()
    {
        drawPhaseTimer.OnPhaseEnded += Settle;
        stageManager.OnStageClear += HandleStageClear;
        stageManager.OnAllStagesCleared += HandleAllStagesCleared;
        stageManager.OnStageStart += HandleStageStart;
        if (boss != null)
            boss.OnDamageTaken += HandleBossDamageTaken;
    }

    void OnDisable()
    {
        drawPhaseTimer.OnPhaseEnded -= Settle;
        stageManager.OnStageClear -= HandleStageClear;
        stageManager.OnAllStagesCleared -= HandleAllStagesCleared;
        stageManager.OnStageStart -= HandleStageStart;
        if (boss != null)
            boss.OnDamageTaken -= HandleBossDamageTaken;
    }

    void HandleBossDamageTaken(int amount)
    {
        _turnDamageTotal += amount;
        OnTurnDamageChanged?.Invoke(_turnDamageTotal);
    }

    void Start()
    {
        _isAdventureMode = string.IsNullOrEmpty(BossPartyContext.BossId);
        _isBossPlay =
            !_isAdventureMode && GameStateMachine.Instance?.CurrentState == GameState.BossPlay;

        if (!_isAdventureMode)
            InitFromBossContext();
        else if (AdventurePartyContext.PendingCharacterIds != null)
            characterSet.SetCharactersByIds(AdventurePartyContext.PendingCharacterIds);

        _jokerRewardPending = true;
        stageManager.StartStage();
    }

    void InitFromBossContext()
    {
        if (
            saveManager != null
            && saveManager.TryLoad(BossPartyContext.SaveSlotIndex, out var saveData)
        )
        {
            characterSet.SetCharactersByIds(saveData.characterIds);

            var jokerCards = new JokerCard[saveData.jokerIds.Length];
            for (int i = 0; i < saveData.jokerIds.Length; i++)
            {
                var id = saveData.jokerIds[i];
                jokerCards[i] = string.IsNullOrEmpty(id)
                    ? null
                    : TableRegistry.Instance.JokerCard.Get(id);
            }
            jokerManager.SetHand(jokerCards);
        }

        if (TableRegistry.Instance?.Boss.TryGet(BossPartyContext.BossId, out var bossData) == true)
            stageManager.SetSingleBossStage(bossData);

        BossPartyContext.BossId = null;
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
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // 턴 데미지 없이 넘기기
            BeginBattle();
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            // 슬롯 0 로드
            if (saveManager != null && saveManager.TryLoad(0, out var saveData))
            {
                characterSet.SetCharactersByIds(saveData.characterIds);
                var jokerCards = new JokerCard[saveData.jokerIds.Length];
                for (int i = 0; i < saveData.jokerIds.Length; i++)
                {
                    var id = saveData.jokerIds[i];
                    jokerCards[i] = string.IsNullOrEmpty(id)
                        ? null
                        : TableRegistry.Instance.JokerCard.Get(id);
                }
                jokerManager.SetHand(jokerCards);
                Debug.Log("[Save] 슬롯 0 로드 완료");
            }
        }
    }

    public void SetPaused(bool paused) => IsPaused = paused;

    // 데미지 표기 용
    // 시퀀스랑 합쳐놓으면 턴 인덱스가 시작할 때 증가되기 때문에 2턴에 진행될 정보가 들어가서 틀려짐
    public int[] PreviewGroupDamages(List<ChainGroup> groups)
    {
        var judge = new ChainJudge();
        judge.IngestGroups(groups);
        judge.IngestHand(blockManager.hand);
        judge.remainingTimeRatio = drawPhaseTimer.RemainingRatio;
        judge.discardRemaining = blockManager.DiscardsRemaining;
        judge.discardUsed = blockManager.DiscardsUsed;
        judge.discardsByClass = blockManager.DiscardsByClass;
        judge.bossMaxHp = boss.MaxHp;

        if (bossPatternSystem != null)
        {
            bossPatternSystem.PopulatePrevState(judge);
            bossPatternSystem.ApplyModifiers(judge);
        }
        return CalcDamages(groups, judge);
    }

    int[] CalcDamages(List<ChainGroup> groups, ChainJudge judge)
    {
        // 우측 조커 스킵 인덱스 계산
        var skipJokerIndices = new HashSet<int>();
        if (judge.skipRightmostJokers > 0 && jokerManager != null)
        {
            int skipped = 0;
            for (
                int k = jokerManager.ActiveHand.Length - 1;
                k >= 0 && skipped < judge.skipRightmostJokers;
                k--
            )
            {
                if (jokerManager.ActiveHand[k] != null)
                {
                    skipJokerIndices.Add(k);
                    skipped++;
                }
            }
        }

        float deckBonus = 1f;
        if (jokerManager != null)
            for (int k = 0; k < jokerManager.ActiveHand.Length; k++)
                if (jokerManager.ActiveHand[k] != null && !skipJokerIndices.Contains(k))
                    deckBonus *= jokerManager.ActiveHand[k].DeckBonus(judge);

        var result = new int[groups.Count];
        for (int i = 0; i < groups.Count; i++)
        {
            var group = groups[i];
            var character = characterSet?.GetCharacter(group.DominantClass);

            int groupBonus = 0;
            if (jokerManager != null)
                for (int k = 0; k < jokerManager.ActiveHand.Length; k++)
                    if (jokerManager.ActiveHand[k] != null && !skipJokerIndices.Contains(k))
                        groupBonus += jokerManager.ActiveHand[k].GetBonus(judge, group);

            int dmg = CalcGroupDamage(group, judge);
            dmg -= judge.bossFlatBonus;
            dmg += groupBonus;
            dmg = Mathf.FloorToInt(dmg * (2 - judge.bossDamageMultiplier));

            if (judge.classDiscriminateActive)
            {
                int classBlocks = judge.blockDistribution.GetValueOrDefault(group.DominantClass);
                dmg = Mathf.FloorToInt(
                    dmg * Mathf.Max(0f, 1f - judge.classDiscriminatePerBlock * classBlocks)
                );
            }

            if (!judge.isShiftBlock && judge.nonShiftPenaltyMultiplier != 1f)
                dmg = Mathf.FloorToInt(dmg * judge.nonShiftPenaltyMultiplier);

            dmg = character?.ApplyPassive(judge, group, dmg) ?? dmg;
            dmg = Mathf.FloorToInt(dmg * deckBonus);
            result[i] = dmg;
        }
        return result;
    }

    public void BeginBattle()
    {
        SetPaused(false);
        StartCoroutine(StartTurnRoutine());
    }

    void HandleStageStart(StageManager.StageEntry entry)
    {
        _currentTurn = 0;
        _handPlaysThisPhase = 0;
        blockManager?.ResetStageDiscardCount();
        characterSet?.NotifyStageStart();
    }

    IEnumerator StartTurnRoutine()
    {
        _currentTurn++;
        _turnDamageTotal = 0;
        OnTurnDamageChanged?.Invoke(0);

        // 어드벤처/기존 보스 모드: TURN n / maxTurns 표시
        if (!_isBossPlay && stageIntroUI != null)
            yield return StartCoroutine(stageIntroUI.ShowTurnRoutine(_currentTurn, maxTurns));

        if (stageIntroUI != null && bossPatternSystem != null)
        {
            var p = bossPatternSystem.Current;
            var turnMod =
                p != null && bossPatternSystem.TurnIndex < p.turnModifiers.Length
                    ? p.turnModifiers[bossPatternSystem.TurnIndex]
                    : null;
            if (turnMod != null)
                yield return StartCoroutine(stageIntroUI.ShowBossRoutineRoutine(turnMod));
        }

        drawPhaseTimer.ResetPhaseDuration();
        blockManager.ResetDiscardLimit();
        bossPatternSystem?.ApplyTurnStart(blockManager, drawPhaseTimer);

        if (_isBossPlay)
            drawPhaseTimer.StartDrawPhaseInstant();
        else
            drawPhaseTimer.StartDrawPhase();
    }

    // 보스 플레이 모드: 같은 구간 내 연속 핸드 (연출 없이 즉시 채움)
    IEnumerator ContinueBossHandRoutine()
    {
        _turnDamageTotal = 0;
        OnTurnDamageChanged?.Invoke(0);
        drawPhaseTimer.ResetPhaseDuration();
        blockManager.ResetDiscardLimit();
        bossPatternSystem?.ApplyTurnStart(blockManager, drawPhaseTimer);
        drawPhaseTimer.StartDrawPhaseInstant();
        yield break;
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
        if (_isAdventureMode)
            UnlockManager.OnAdventureClear(characterSet.GetCurrentCharacterIds());
        OpenSaveFlow();
    }

    void OpenSaveFlow()
    {
        if (saveManager == null || saveSlotPickerUI == null)
            return;
        var draft = saveManager.BuildFromCurrentState(this);
        saveSlotPickerUI.Show(
            saveManager,
            draft,
            onSlotPicked: slot =>
            {
                if (saveManager.HasSlot(slot))
                    confirmDialog?.Show(
                        string.Format(Localization.Get("ui_confirm_overwrite"), slot + 1),
                        onYes: () => saveManager.Save(slot, draft),
                        onNo: OpenSaveFlow
                    );
                else
                    saveManager.Save(slot, draft);
            },
            onCanceled: () => { }
        );
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
        // 1. 패시브/턴 modifier가 손패를 정산 전에 변경할 기회 (예: 우측 블록 제거)
        bossPatternSystem?.ApplyPreResolve(blockManager);

        // 2. 체인 그룹 해석
        var groups = ChainResolver.ResolveChains(blockManager.hand);

        // 3. judge 구성
        var judge = new ChainJudge();
        judge.IngestGroups(groups);
        judge.IngestHand(blockManager.hand);
        judge.remainingTimeRatio = drawPhaseTimer.RemainingRatio;
        judge.discardRemaining = blockManager.DiscardsRemaining;
        judge.discardUsed = blockManager.DiscardsUsed;
        judge.stageDiscardsUsed = blockManager.StageDiscardsUsed;
        judge.discardsByClass = blockManager.DiscardsByClass;
        judge.bossMaxHp = boss.MaxHp;

        if (bossPatternSystem != null)
        {
            bossPatternSystem.PopulatePrevState(judge);
            bossPatternSystem.Inject(judge);
            bossPatternSystem.SnapshotForNextTurn(judge);
            // 보스 플레이 모드는 HP 구간 돌파 시 PlayGroupSequence에서 AdvanceTurn 호출
            if (!_isBossPlay)
                bossPatternSystem.AdvanceTurn();
        }

        StartCoroutine(PlayGroupSequence(groups, judge));
    }

    static int[] SplitDamageWeighted(int total, int chainLength)
    {
        var result = new int[chainLength];
        int weightSum = chainLength * (chainLength + 1) / 2;
        int accumulated = 0;
        for (int i = 0; i < chainLength - 1; i++)
        {
            result[i] = Mathf.RoundToInt(total * (i + 1) / (float)weightSum);
            accumulated += result[i];
        }
        result[chainLength - 1] = total - accumulated;
        return result;
    }

    IEnumerator PlayGroupSequence(List<ChainGroup> groups, ChainJudge judge)
    {
        var damages = CalcDamages(groups, judge);
        for (int i = 0; i < groups.Count; i++)
        {
            // 블록 강조 펄스
            foreach (var block in groups[i].Blocks)
                block.StartCoroutine(
                    block.HighlightPulseRoutine(highlightScale, highlightDuration)
                );
            yield return new WaitForSeconds(highlightDuration);

            // 애니메이션
            var character = characterSet?.GetCharacter(groups[i].DominantClass);
            character?.PlayAttack(boss.transform.position);
            var perHitDamages = SplitDamageWeighted(damages[i], groups[i].Length);
            character?.PlaySkillEffect(groups[i].Length, perHitDamages, boss);

            blockManager.RemoveGroup(groups[i]);
            yield return new WaitForSeconds(perGroupDelay + 0.25f * (groups[i].Length - 1));
        }

        if (_stageClearPending)
        {
            _stageClearPending = false;
            yield return StartCoroutine(ShowStageClear());
            stageManager.AdvanceToNext();
        }
        else if (boss.IsAlive)
        {
            if (_isBossPlay)
                HandleBossPlayHandResult();
            else if (_currentTurn >= maxTurns)
                modeClearUI?.Show(this, Color.red, "ui_game_over");
            else
                StartCoroutine(StartTurnRoutine());
        }
    }

    void HandleBossPlayHandResult()
    {
        var pattern = bossPatternSystem?.Current;
        if (pattern == null || pattern.hpThresholds == null || pattern.hpThresholds.Length == 0)
        {
            StartCoroutine(ContinueBossHandRoutine());
            return;
        }

        float hpRatio = (float)boss.CurrentHp / boss.MaxHp;
        int phaseIndex = bossPatternSystem.TurnIndex;
        bool thresholdCrossed =
            phaseIndex < pattern.hpThresholds.Length && hpRatio <= pattern.hpThresholds[phaseIndex];

        if (thresholdCrossed)
        {
            // 구간 돌파 → 다음 구간 시작
            _handPlaysThisPhase = 0;
            bossPatternSystem.AdvanceTurn();
            StartCoroutine(StartTurnRoutine());
        }
        else
        {
            _handPlaysThisPhase++;
            if (_handPlaysThisPhase >= MaxHandsPerPhase)
                modeClearUI?.Show(this, Color.red, "ui_game_over");
            else
                StartCoroutine(ContinueBossHandRoutine());
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

    int CalcGroupDamage(ChainGroup group, ChainJudge judge)
    {
        int idx = group.Length - 1;
        if (idx < 0 || idx > 2)
            return 0;
        if (judge.chainLevelNullified[idx])
            return 0;
        if (judge.classNullified.Contains(group.DominantClass))
            return 0;
        if (judge.requireAllThreeClasses && judge.classDistribution.Count < 3)
            return 0;

        float baseMul = group.Length switch
        {
            1 => 1.1f,
            2 => 1.2f,
            3 => 1.3f,
            _ => 1f,
        };
        baseMul *= judge.chainLevelMultiplier[idx];
        return Mathf.FloorToInt(baseMul * group.Blocks[0].data.attackPower * group.Length);
    }
}
