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
            StartCoroutine(CheatAllClear());
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
        judge.isPreview = true;
        judge.IngestGroups(groups);
        judge.IngestHand(blockManager.hand);
        judge.remainingTimeRatio = drawPhaseTimer.RemainingRatio;
        judge.discardRemaining = blockManager.DiscardsRemaining;
        judge.discardUsed = blockManager.DiscardsUsed;
        judge.stageDiscardsUsed = blockManager.StageDiscardsUsed;
        judge.discardsByClass = blockManager.DiscardsByClass;
        judge.stageDiscardsByClass = blockManager.StageDiscardsByClass;
        judge.bossMaxHp = boss.MaxHp;

        if (bossPatternSystem != null)
        {
            bossPatternSystem.PopulatePrevState(judge);
            bossPatternSystem.ApplyModifiers(judge);
        }
        return CalcDamages(groups, judge);
    }

    (HashSet<int> skipJokerIndices, float deckBonus) BuildJokerContext(ChainJudge judge)
    {
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

        return (skipJokerIndices, deckBonus);
    }

    // boss 배율·시프트 페널티는 CalcSingleGroupDamage에서 ApplyPassive 이전 적용 (Hikari flat 보너스 수치 보존)
    float CalcPartyBonus(ChainJudge judge, float deckBonus)
    {
        bool protection = false;
        float partyPassiveBonus = 0f;

        if (characterSet != null)
            foreach (var c in characterSet.GetInstances())
            {
                if (c.IsProtectionPassive(judge))
                    protection = true;
                else
                    partyPassiveBonus += c.GetPartyBonus(judge);
            }

        if (protection)
            judge.ClearDebuffs();

        return (1f + partyPassiveBonus) * deckBonus;
    }

    int CalcSingleGroupDamage(
        ChainGroup group,
        ChainJudge judge,
        HashSet<int> skipJokerIndices,
        float partyBonus
    )
    {
        var character = group.DominantCharacter;

        int jokerGroupBonus = 0;
        if (jokerManager != null)
            for (int k = 0; k < jokerManager.ActiveHand.Length; k++)
                if (jokerManager.ActiveHand[k] != null && !skipJokerIndices.Contains(k))
                    jokerGroupBonus += jokerManager.ActiveHand[k].GetBonus(judge, group);

        int dmg = CalcGroupDamage(group, judge);
        dmg -= judge.bossFlatBonus;
        dmg += jokerGroupBonus;

        float chainBonus = character?.GetChainTypeBonus(judge, group) ?? 0f;
        float classBonus = character?.GetClassTypeBonus(judge, group) ?? 0f;
        foreach (var m in judge.activeModifiers)
        {
            chainBonus -= m.GetChainBonusPenalty(group);
            classBonus -= m.GetClassBonusPenalty(group);
        }
        if (chainBonus != 0f || classBonus != 0f)
            dmg = Mathf.RoundToInt(dmg * (1f + chainBonus + classBonus));

        if (judge.classDiscriminateActive)
        {
            int classBlocks = judge.blockDistribution.GetValueOrDefault(group.DominantClass);
            dmg = Mathf.FloorToInt(
                dmg * Mathf.Max(0f, 1f - judge.classDiscriminatePerBlock * classBlocks)
            );
        }

        // 보스 데미지 배율·시프트 페널티: ApplyPassive 이전 적용 (flat 보너스 패시브 수치 보존)
        dmg = Mathf.FloorToInt(dmg * (2f - judge.bossDamageMultiplier));
        if (!judge.isShiftBlock && judge.nonShiftPenaltyMultiplier != 1f)
            dmg = Mathf.FloorToInt(dmg * judge.nonShiftPenaltyMultiplier);

        // 미이관 패시브 (ApplyPassive 유지: Hikari, AhnMansik)
        dmg = character?.ApplyPassive(judge, group, dmg) ?? dmg;

        int rawDmg = dmg;
        dmg = Mathf.FloorToInt(dmg * partyBonus);

        character?.ApplyDebuffPassive(judge, group);
        characterSet?.NotifyAnyGroupDamage(rawDmg, dmg);
        return dmg;
    }

    int[] CalcDamages(List<ChainGroup> groups, ChainJudge judge)
    {
        var (skipJokerIndices, deckBonus) = BuildJokerContext(judge);
        float partyBonus = CalcPartyBonus(judge, deckBonus);
        var result = new int[groups.Count];
        for (int i = 0; i < groups.Count; i++)
            result[i] = CalcSingleGroupDamage(groups[i], judge, skipJokerIndices, partyBonus);
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
        OnHandPlayCountChanged?.Invoke(1, MaxHandsPerPhase);
        blockManager?.ResetStageDiscardCount();
        characterSet?.NotifyStageStart();
    }

    IEnumerator StartTurnRoutine()
    {
        _currentTurn++;
        _turnDamageTotal = 0;
        OnTurnDamageChanged?.Invoke(0);
        totalDmgObject?.SetActive(false);

        // 어드벤처/기존 보스 모드: TURN n / maxTurns 표시
        if (!_isBossPlay && stageIntroUI != null)
            yield return StartCoroutine(stageIntroUI.ShowTurnRoutine(_currentTurn, maxTurns));

        if (stageIntroUI != null && bossPatternSystem != null)
        {
            var p = bossPatternSystem.Current;
            var phaseMod =
                p != null && bossPatternSystem.PhaseIndex < p.phaseModifiers.Length
                    ? p.phaseModifiers[bossPatternSystem.PhaseIndex]
                    : null;
            if (phaseMod != null)
                yield return StartCoroutine(stageIntroUI.ShowBossRoutineRoutine(phaseMod));
        }

        drawPhaseTimer.ResetPhaseDuration();
        blockManager.ResetDiscardLimit();
        bossPatternSystem?.ApplyPhaseStart(blockManager, drawPhaseTimer);

        if (_isBossPlay)
            drawPhaseTimer.StartDrawPhaseInstant();
        else
            drawPhaseTimer.StartDrawPhase();
    }

    // 보스 플레이 모드: 같은 구간 내 연속 핸드 (연출 없이 즉시 채움)
    IEnumerator ContinueBossHandRoutine()
    {
        OnHandPlayCountChanged?.Invoke(_handPlaysThisPhase + 1, MaxHandsPerPhase);
        _turnDamageTotal = 0;
        OnTurnDamageChanged?.Invoke(0);
        totalDmgObject?.SetActive(false);
        drawPhaseTimer.ResetPhaseDuration();
        blockManager.ResetDiscardLimit();
        bossPatternSystem?.ApplyPhaseStart(blockManager, drawPhaseTimer);
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
        _allStagesClearedPending = true;
    }

    void OpenSaveFlow()
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
        else if (_isBossPlay && _currentTurn == 0)
        {
            StartCoroutine(BossPassiveIntroRoutine());
        }
        else
        {
            BeginBattle();
        }
    }

    IEnumerator BossPassiveIntroRoutine()
    {
        if (stageIntroUI != null && bossPatternSystem?.Current != null)
        {
            var passives = bossPatternSystem.Current.passive;
            if (passives != null)
                foreach (var passive in passives)
                    if (passive != null)
                        yield return StartCoroutine(stageIntroUI.ShowBossRoutineRoutine(passive));
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
        var judge = new ChainJudge();
        judge.IngestGroups(groups);
        judge.IngestHand(blockManager.hand);
        judge.remainingTimeRatio = drawPhaseTimer.RemainingRatio;
        judge.discardRemaining = blockManager.DiscardsRemaining;
        judge.discardUsed = blockManager.DiscardsUsed;
        judge.stageDiscardsUsed = blockManager.StageDiscardsUsed;
        judge.discardsByClass = blockManager.DiscardsByClass;
        judge.stageDiscardsByClass = blockManager.StageDiscardsByClass;
        judge.bossMaxHp = boss.MaxHp;

        if (bossPatternSystem != null)
        {
            bossPatternSystem.PopulatePrevState(judge);
            bossPatternSystem.Inject(judge);
            bossPatternSystem.SnapshotForNextTurn(judge);
            // 보스 플레이 모드는 HP 구간 돌파 시 PlayGroupSequence에서 AdvancePhase 호출
            if (!_isBossPlay)
                bossPatternSystem.AdvancePhase();
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
        var (skipJokerIndices, deckBonus) = BuildJokerContext(judge);
        float partyBonus = CalcPartyBonus(judge, deckBonus);
        for (int i = 0; i < groups.Count; i++)
        {
            // 블록 강조 펄스
            foreach (var block in groups[i].Blocks)
                block.StartCoroutine(
                    block.HighlightPulseRoutine(highlightScale, highlightDuration)
                );
            yield return new WaitForSeconds(highlightDuration);

            // 애니메이션
            var character = groups[i].DominantCharacter;
            characterSet?.NotifyAnyGroupAttackStart(groups[i], boss);
            character?.PlayAttack(boss.transform.position);
            int dmg = CalcSingleGroupDamage(groups[i], judge, skipJokerIndices, partyBonus);
            var perHitDamages = SplitDamageWeighted(dmg, groups[i].Length);
            character?.PlaySkillEffect(groups[i].Length, perHitDamages, boss);

            characterSet?.NotifyTurnProcessed(groups[i].DominantCharacter);
            characterSet?.NotifyAfterGroupPlayed(groups[i]);

            int bonusCount = character?.GetBonusAttackCount(judge, groups[i]) ?? 0;
            for (int b = 0; b < bonusCount; b++)
            {
                yield return new WaitForSeconds(0.2f);
                int bonusDmg = dmg / Mathf.Max(1, bonusCount);
                character?.PlaySkillEffect(1, new[] { bonusDmg }, boss);
            }

            blockManager.RemoveGroup(groups[i]);
            yield return new WaitForSeconds(perGroupDelay + 0.25f * (groups[i].Length - 1));
        }

        characterSet?.NotifyTurnSequenceEnd();

        if (_stageClearPending)
        {
            _stageClearPending = false;
            yield return StartCoroutine(ShowStageClear());
            stageManager.AdvanceToNext();
        }
        else if (_allStagesClearedPending)
        {
            _allStagesClearedPending = false;
            yield return StartCoroutine(ShowStageClear());
            modeClearUI?.Show(this, Color.green);
            if (_isAdventureMode)
                UnlockManager.OnAdventureClear(characterSet.GetCurrentCharacterIds());
            if (!_isBossPlay)
                OpenSaveFlow();
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

        // 이번 핸드에서 돌파된 모든 구간 수집
        float hpRatio = (float)boss.CurrentHp / boss.MaxHp;
        var crossedMods = new System.Collections.Generic.List<Modifier>();
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
            StartCoroutine(PhaseTransitionRoutine(crossedMods));
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

    IEnumerator PhaseTransitionRoutine(List<Modifier> mods)
    {
        blockManager.ResetStageDiscardCount();
        characterSet?.NotifyStageStart();
        foreach (var mod in mods)
        {
            // 보스 말풍선 대사
            if (bossSpeechBubbleUI != null && !string.IsNullOrEmpty(mod.dialogueKey))
                yield return StartCoroutine(
                    bossSpeechBubbleUI.Show(Localization.Get(mod.dialogueKey))
                );

            // 디버프 텍스트 (항상)
            if (stageIntroUI != null)
                yield return StartCoroutine(stageIntroUI.ShowBossRoutineRoutine(mod));

            // 이펙트 + 플로팅 텍스트 + 아이콘 갱신
            if (phaseEffectSpawner != null && mod.effectPrefab != null)
                yield return StartCoroutine(
                    phaseEffectSpawner.PlayEffect(mod.effectPrefab, Localization.Get(mod.modName))
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
        drawPhaseTimer.StartDrawPhaseInstant();
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

    IEnumerator CheatAllClear()
    {
        yield return StartCoroutine(ShowStageClear());
        modeClearUI?.Show(this, Color.green);
        if (_isAdventureMode)
            UnlockManager.OnAdventureClear(characterSet.GetCurrentCharacterIds());
        if (!_isBossPlay)
            OpenSaveFlow();
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
            2 => 1.1f,
            3 => 1.2f,
            _ => 1f,
        };
        baseMul *= judge.chainLevelMultiplier[idx];
        int ap = characterSet?.GetDef(group.DominantCharacter)?.attackPower ?? 10;
        return Mathf.FloorToInt(baseMul * ap * group.Length);
    }
}
