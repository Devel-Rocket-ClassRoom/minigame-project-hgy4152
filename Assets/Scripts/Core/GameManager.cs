using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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
    BossSpeechBubbleUI bossSpeechBubbleUI;

    [SerializeField]
    PhaseEffectSpawner phaseEffectSpawner;

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
    GameObject totalDmgObject;

    [Header("Debug (씬 직접 실행 시)")]
    [SerializeField]
    BossData debugBossData;

    [SerializeField]
    int maxTurns = 5;

    [SerializeField]
    int rewindLimitPerSegment = 1; // 보스 플레이 구간당 핸드 되돌리기 허용 횟수

    [SerializeField]
    float stageClearDisplayDuration = 1.5f;

    [SerializeField]
    float perGroupDelay = 0.4f;

    [SerializeField]
    float highlightDuration = 0.2f;

    [SerializeField]
    float highlightScale = 1.2f;

    bool _stageClearPending;
    bool _allStagesClearedPending;
    bool _jokerRewardPending;
    bool _isAdventureMode;
    bool _isBossPlay;
    int _currentTurn;
    int _turnDamageTotal;
    int _handPlaysThisPhase;

    const int MaxHandsPerPhase = 5;

    public bool IsPaused { get; private set; }
    public bool IsBossPlay => _isBossPlay;
    public int TurnDamageTotal => _turnDamageTotal;
    public event System.Action<int> OnTurnDamageChanged;
    public event System.Action<int, int> OnHandPlayCountChanged;
    public CharacterSet CharacterSet => characterSet;
    public JokerManager JokerManager => jokerManager;

    // 전투 진행 상태 (상태 패턴 — 흩어진 pending bool 분기의 단일 전이 지점)
    public BattlePhase Phase { get; private set; } = BattlePhase.None;
    public event System.Action<BattlePhase> OnBattlePhaseChanged;

    void SetPhase(BattlePhase phase)
    {
        if (Phase == phase)
            return;
        Phase = phase;
        OnBattlePhaseChanged?.Invoke(phase);
    }

    DamageCalculator _damage;
    CancellationToken DestroyToken => this.GetCancellationTokenOnDestroy();

    // 보스 플레이 핸드 되돌리기 (커맨드 패턴)
    readonly CommandHistory _handHistory = new();
    int _rewindsUsedThisSegment;

    void Awake()
    {
        _damage = new DamageCalculator(jokerManager, characterSet);
    }

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

        if (totalDmgObject != null && !totalDmgObject.activeSelf)
            totalDmgObject.SetActive(true);
    }

    void Start()
    {
        _isAdventureMode = string.IsNullOrEmpty(BossPartyContext.BossId);
        _isBossPlay =
            !_isAdventureMode && GameStateMachine.Instance?.CurrentState == GameState.BossPlay;

        if (!_isAdventureMode)
            InitFromBossContext();
        else if (debugBossData != null)
        {
            _isAdventureMode = false;
            _isBossPlay = true;
            stageManager.SetSingleBossStage(debugBossData);
        }
        else if (AdventurePartyContext.PendingCharacterIds != null)
            characterSet.SetCharactersByIds(AdventurePartyContext.PendingCharacterIds);

        if (bossPatternSystem != null)
            bossPatternSystem.AccumulateModifiers = _isBossPlay;

        _jokerRewardPending = !_isBossPlay;

        if (_isBossPlay)
        {
            var canvas = totalDmgObject != null
                ? totalDmgObject.GetComponentInParent<Canvas>(true)
                : null;
            if (canvas != null)
                HandRewindButtonUI.Create(this, canvas);
        }

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
            drawPhaseTimer.StopDrawPhase();
            SetPaused(true);
            CheatAllClearAsync(DestroyToken).Forget();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // 즉시 스테이지 클리어
            // 스테이지 클리어 시 활성화 될 것들 확인
            drawPhaseTimer.StopDrawPhase();
            SetPaused(true);
            _jokerRewardPending = true;
            CheatStageClearAsync(DestroyToken).Forget();
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
        var judge = BuildJudge(groups, isPreview: true);

        if (bossPatternSystem != null)
        {
            bossPatternSystem.PopulatePrevState(judge);
            bossPatternSystem.ApplyModifiers(judge);
        }
        return _damage.CalcDamages(groups, judge);
    }

    ChainJudge BuildJudge(List<ChainGroup> groups, bool isPreview)
    {
        var judge = new ChainJudge();
        judge.isPreview = isPreview;
        judge.IngestGroups(groups);
        judge.IngestHand(blockManager.hand);
        judge.remainingTimeRatio = drawPhaseTimer.RemainingRatio;
        judge.discardRemaining = blockManager.DiscardsRemaining;
        judge.discardUsed = blockManager.DiscardsUsed;
        judge.stageDiscardsUsed = blockManager.StageDiscardsUsed;
        judge.discardsByClass = blockManager.DiscardsByClass;
        judge.stageDiscardsByClass = blockManager.StageDiscardsByClass;
        judge.bossMaxHp = boss.MaxHp;
        return judge;
    }

    public void BeginBattle()
    {
        SetPaused(false);
        StartTurnAsync(DestroyToken).Forget();
    }

    void HandleStageStart(StageManager.StageEntry entry)
    {
        SetPhase(BattlePhase.StageIntro);
        _currentTurn = 0;
        _handPlaysThisPhase = 0;
        _handHistory.Clear();
        _rewindsUsedThisSegment = 0;
        OnHandPlayCountChanged?.Invoke(1, MaxHandsPerPhase);
        blockManager?.ResetStageDiscardCount();
        characterSet?.NotifyStageStart();
    }

    async UniTask StartTurnAsync(CancellationToken ct)
    {
        _currentTurn++;
        _turnDamageTotal = 0;
        OnTurnDamageChanged?.Invoke(0);
        totalDmgObject?.SetActive(false);

        // 어드벤처/기존 보스 모드: TURN n / maxTurns 표시
        if (!_isBossPlay && stageIntroUI != null)
            await stageIntroUI.ShowTurnAsync(_currentTurn, maxTurns);

        if (stageIntroUI != null && bossPatternSystem != null)
        {
            var p = bossPatternSystem.Current;
            var phaseMod =
                p != null && bossPatternSystem.PhaseIndex < p.phaseModifiers.Length
                    ? p.phaseModifiers[bossPatternSystem.PhaseIndex]
                    : null;
            if (phaseMod != null)
                await stageIntroUI.ShowBossRoutineAsync(phaseMod);
        }

        drawPhaseTimer.ResetPhaseDuration();
        blockManager.ResetDiscardLimit();
        bossPatternSystem?.ApplyPhaseStart(blockManager, drawPhaseTimer);

        if (_isBossPlay)
        {
            RecordBossHandStart();
            drawPhaseTimer.StartDrawPhaseInstant();
        }
        else
            drawPhaseTimer.StartDrawPhase();
        SetPhase(BattlePhase.DrawPhase);
    }

    // 보스 플레이 모드: 같은 구간 내 연속 핸드 (연출 없이 즉시 채움)
    void ContinueBossHand()
    {
        OnHandPlayCountChanged?.Invoke(_handPlaysThisPhase + 1, MaxHandsPerPhase);
        _turnDamageTotal = 0;
        OnTurnDamageChanged?.Invoke(0);
        totalDmgObject?.SetActive(false);
        drawPhaseTimer.ResetPhaseDuration();
        blockManager.ResetDiscardLimit();
        bossPatternSystem?.ApplyPhaseStart(blockManager, drawPhaseTimer);
        RecordBossHandStart();
        drawPhaseTimer.StartDrawPhaseInstant();
        SetPhase(BattlePhase.DrawPhase);
    }

    // 핸드 시작 직전 상태를 커맨드로 기록 (되돌리기의 Undo 단위)
    void RecordBossHandStart()
    {
        if (!_isBossPlay)
            return;
        _handHistory.Push(new HandPlayCommand(boss, blockManager, bossPatternSystem, characterSet));
    }

    public bool CanRewindHand =>
        _isBossPlay
        && Phase == BattlePhase.DrawPhase
        && _handPlaysThisPhase >= 2
        && _rewindsUsedThisSegment < rewindLimitPerSegment
        && _handHistory.Count > 0;

    // 현재 HP 구간의 첫 핸드 직전 상태로 복원 (커맨드 스택 일괄 Undo)
    public void RewindToSegmentStart()
    {
        if (!CanRewindHand)
            return;

        _rewindsUsedThisSegment++;
        drawPhaseTimer.StopDrawPhase();
        blockManager.ClearHand();
        _handHistory.UndoAll();
        _handPlaysThisPhase = 0;
        ContinueBossHand();
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
        _allStagesClearedPending = true;
    }

    // 슬롯 선택 → 덮어쓰기 확인 → 거절 시 재선택을 선형 async 흐름으로 처리
    async UniTaskVoid OpenSaveFlowAsync()
    {
        if (saveManager == null)
            return;

        int emptySlot = saveManager.FindFirstEmptySlot();
        if (emptySlot >= 0)
        {
            saveManager.Save(emptySlot, saveManager.BuildFromCurrentState(this));
            return;
        }

        // 모든 슬롯이 채워진 경우 수동 선택 UI
        if (saveSlotPickerUI == null)
            return;
        var draft = saveManager.BuildFromCurrentState(this);

        while (true)
        {
            int slot = await saveSlotPickerUI.ShowAsync(saveManager, draft);
            if (slot < 0)
                return; // 취소

            if (saveManager.HasSlot(slot))
            {
                if (confirmDialog == null)
                    return;
                bool overwrite = await confirmDialog.ShowAsync(
                    string.Format(Localization.Get("ui_confirm_overwrite"), slot + 1)
                );
                if (!overwrite)
                    continue; // 다시 슬롯 선택
            }

            saveManager.Save(slot, draft);
            return;
        }
    }

    public void OnStageIntroComplete()
    {
        if (_jokerRewardPending)
        {
            _jokerRewardPending = false;
            SetPhase(BattlePhase.JokerReward);
            jokerRewardUI.Show();
        }
        else if (_isBossPlay && _currentTurn == 0)
        {
            BossPassiveIntroAsync().Forget();
        }
        else
        {
            BeginBattle();
        }
    }

    async UniTaskVoid BossPassiveIntroAsync()
    {
        if (stageIntroUI != null && bossPatternSystem?.Current != null)
        {
            var passives = bossPatternSystem.Current.passive;
            if (passives != null)
                foreach (var passive in passives)
                    if (passive != null)
                        await stageIntroUI.ShowBossRoutineAsync(passive);
        }
        BeginBattle();
    }

    void Settle()
    {
        // 1. 패시브/턴 modifier가 손패를 정산 전에 변경할 기회 (예: 우측 블록 제거)
        bossPatternSystem?.ApplyPreResolve(blockManager);

        // 2. 체인 그룹 해석
        var groups = ChainResolver.ResolveChains(blockManager.hand);

        // 3. judge 구성
        var judge = BuildJudge(groups, isPreview: false);

        if (bossPatternSystem != null)
        {
            bossPatternSystem.PopulatePrevState(judge);
            bossPatternSystem.Inject(judge);
            bossPatternSystem.SnapshotForNextTurn(judge);
            // 보스 플레이 모드는 HP 구간 돌파 시 PlayGroupSequence에서 AdvancePhase 호출
            if (!_isBossPlay)
                bossPatternSystem.AdvancePhase();
        }

        PlayGroupSequenceAsync(groups, judge, DestroyToken).Forget();
    }

    async UniTask PlayGroupSequenceAsync(
        List<ChainGroup> groups,
        ChainJudge judge,
        CancellationToken ct
    )
    {
        SetPhase(BattlePhase.Resolving);
        var (skipJokerIndices, deckBonus) = _damage.BuildJokerContext(judge);
        float partyBonus = _damage.CalcPartyBonus(judge, deckBonus);
        for (int i = 0; i < groups.Count; i++)
        {
            // 블록 강조 펄스
            foreach (var block in groups[i].Blocks)
                block.HighlightPulse(highlightScale, highlightDuration);
            await UniTask.Delay(TimeSpan.FromSeconds(highlightDuration), cancellationToken: ct);

            // 애니메이션
            var character = groups[i].DominantCharacter;
            characterSet?.NotifyAnyGroupAttackStart(groups[i], boss);
            character?.PlayAttack(boss.transform.position);
            int dmg = _damage.CalcSingleGroupDamage(groups[i], judge, skipJokerIndices, partyBonus);
            DamageLog.Group(i, groups[i], dmg);
            var perHitDamages = DamageCalculator.SplitDamageWeighted(dmg, groups[i].Length);
            character?.PlaySkillEffect(groups[i].Length, perHitDamages, boss);

            characterSet?.NotifyTurnProcessed(groups[i].DominantCharacter);
            characterSet?.NotifyAfterGroupPlayed(groups[i]);

            int bonusCount = character?.GetBonusAttackCount(judge, groups[i]) ?? 0;
            for (int b = 0; b < bonusCount; b++)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: ct);
                int bonusDmg = dmg / Mathf.Max(1, bonusCount);
                character?.PlaySkillEffect(1, new[] { bonusDmg }, boss);
            }

            blockManager.RemoveGroup(groups[i]);
            await UniTask.Delay(
                TimeSpan.FromSeconds(perGroupDelay + 0.25f * (groups[i].Length - 1)),
                cancellationToken: ct
            );
        }

        characterSet?.NotifyTurnSequenceEnd();

        await NextPhaseAfterResolveAsync(ct);
    }

    // 정산 종료 후 다음 전투 단계 결정 — 전이 분기의 단일 지점
    async UniTask NextPhaseAfterResolveAsync(CancellationToken ct)
    {
        if (_stageClearPending)
        {
            _stageClearPending = false;
            await ShowStageClearAsync(ct);
            stageManager.AdvanceToNext();
        }
        else if (_allStagesClearedPending)
        {
            _allStagesClearedPending = false;
            await ShowStageClearAsync(ct);
            ShowModeClear();
        }
        else if (boss.IsAlive)
        {
            if (_isBossPlay)
                await HandleBossPlayHandResultAsync(ct);
            else if (_currentTurn >= maxTurns)
                ShowGameOver();
            else
                await StartTurnAsync(ct);
        }
    }

    void ShowModeClear()
    {
        SetPhase(BattlePhase.ModeClear);
        modeClearUI?.Show(this, Color.green);
        if (_isAdventureMode)
            UnlockManager.OnAdventureClear(characterSet.GetCurrentCharacterIds());
        if (!_isBossPlay)
            OpenSaveFlowAsync().Forget();
    }

    void ShowGameOver()
    {
        SetPhase(BattlePhase.GameOver);
        modeClearUI?.Show(this, Color.red, "ui_game_over");
    }

    async UniTask HandleBossPlayHandResultAsync(CancellationToken ct)
    {
        var pattern = bossPatternSystem?.Current;
        if (pattern == null || pattern.hpThresholds == null || pattern.hpThresholds.Length == 0)
        {
            ContinueBossHand();
            return;
        }

        // 이번 핸드에서 돌파된 모든 구간 수집
        float hpRatio = (float)boss.CurrentHp / boss.MaxHp;
        var crossedMods = new List<Modifier>();
        while (
            bossPatternSystem.PhaseIndex < pattern.hpThresholds.Length
            && hpRatio <= pattern.hpThresholds[bossPatternSystem.PhaseIndex]
        )
        {
            int idx = bossPatternSystem.PhaseIndex;
            if (idx < pattern.phaseModifiers.Length && pattern.phaseModifiers[idx] != null)
                crossedMods.Add(pattern.phaseModifiers[idx]);
            bossPatternSystem.AdvancePhase();
        }

        if (crossedMods.Count > 0)
        {
            _handPlaysThisPhase = 0;
            await PhaseTransitionAsync(crossedMods, ct);
        }
        else
        {
            _handPlaysThisPhase++;
            if (_handPlaysThisPhase >= MaxHandsPerPhase)
                ShowGameOver();
            else
                ContinueBossHand();
        }
    }

    async UniTask PhaseTransitionAsync(List<Modifier> mods, CancellationToken ct)
    {
        SetPhase(BattlePhase.PhaseTransition);
        blockManager.ResetStageDiscardCount();
        characterSet?.NotifyStageStart();
        foreach (var mod in mods)
        {
            // 보스 말풍선 대사
            if (bossSpeechBubbleUI != null && !string.IsNullOrEmpty(mod.dialogueKey))
                await bossSpeechBubbleUI.ShowAsync(Localization.Get(mod.dialogueKey));

            // 디버프 텍스트 (항상)
            if (stageIntroUI != null)
                await stageIntroUI.ShowBossRoutineAsync(mod);

            // 이펙트 + 플로팅 텍스트 + 아이콘 갱신
            if (phaseEffectSpawner != null && mod.effectPrefab != null)
                await phaseEffectSpawner.PlayEffectAsync(
                    mod.effectPrefab,
                    Localization.Get(mod.modName)
                );
        }

        _turnDamageTotal = 0;
        OnTurnDamageChanged?.Invoke(0);
        totalDmgObject?.SetActive(false);
        drawPhaseTimer.ResetPhaseDuration();
        blockManager.ResetDiscardLimit();
        bossPatternSystem?.CommitPhase();
        bossPatternSystem?.ApplyPhaseStart(blockManager, drawPhaseTimer);
        OnHandPlayCountChanged?.Invoke(1, MaxHandsPerPhase);
        _handHistory.Clear(); // 새 HP 구간 — 이전 구간으로의 되돌리기 차단
        _rewindsUsedThisSegment = 0;
        RecordBossHandStart();
        drawPhaseTimer.StartDrawPhaseInstant();
        SetPhase(BattlePhase.DrawPhase);
    }

    async UniTask ShowStageClearAsync(CancellationToken ct)
    {
        SetPhase(BattlePhase.StageClear);
        if (clearTextObject != null)
            clearTextObject.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(stageClearDisplayDuration), cancellationToken: ct);
        if (clearTextObject != null)
            clearTextObject.SetActive(false);
    }

    async UniTaskVoid CheatStageClearAsync(CancellationToken ct)
    {
        await ShowStageClearAsync(ct);
        stageManager.AdvanceToNext();
    }

    async UniTaskVoid CheatAllClearAsync(CancellationToken ct)
    {
        await ShowStageClearAsync(ct);
        ShowModeClear();
    }
}
