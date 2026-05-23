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
        if (AdventurePartyContext.PendingCharacterIds != null)
            characterSet.SetCharactersByIds(AdventurePartyContext.PendingCharacterIds);

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
        judge.remainingTimeRatio = drawPhaseTimer.RemainingRatio;
        judge.discardRemaining = blockManager.DiscardsRemaining;

        judge.discardUsed = blockManager.DiscardsUsed;
        judge.bossMaxHp = boss.MaxHp;

        if (bossPatternSystem != null)
            bossPatternSystem.ApplyModifiers(judge);
        return CalcDamages(groups, judge);
    }

    int[] CalcDamages(List<ChainGroup> groups, ChainJudge judge)
    {
        float deckBonus = 1f;
        if (jokerManager != null)
            foreach (var card in jokerManager.ActiveHand)
                if (card != null)
                    deckBonus *= card.DeckBonus(judge);

        var result = new int[groups.Count];
        for (int i = 0; i < groups.Count; i++)
        {
            var group = groups[i];
            var character = characterSet?.GetCharacter(group.DominantClass);

            var groupJudge = new ChainJudge();
            groupJudge.IngestGroup(group);
            int groupBonus = 0;
            if (jokerManager != null)
                foreach (var card in jokerManager.ActiveHand)
                    if (card != null)
                        groupBonus += card.GetBonus(groupJudge);

            int dmg = CalcGroupDamage(group);
            dmg -= judge.bossFlatBonus;
            dmg += groupBonus;
            dmg = Mathf.FloorToInt(dmg * (2 - judge.bossDamageMultiplier));
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
                        $"슬롯 {slot + 1} 덮어쓸까요?",
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
        var groups = ChainResolver.ResolveChains(blockManager.hand);

        // 전체 그룹 조사
        var judge = new ChainJudge();
        judge.IngestGroups(groups);
        judge.remainingTimeRatio = drawPhaseTimer.RemainingRatio;
        judge.discardRemaining = blockManager.DiscardsRemaining;
        judge.discardUsed = blockManager.DiscardsUsed;
        judge.bossMaxHp = boss.MaxHp;

        if (bossPatternSystem != null)
        {
            bossPatternSystem.Inject(judge);
            bossPatternSystem.AdvanceTurn();
        }

        StartCoroutine(PlayGroupSequence(groups, judge));
    }

    IEnumerator PlayGroupSequence(List<ChainGroup> groups, ChainJudge judge)
    {
        var damages = CalcDamages(groups, judge);
        for (int i = 0; i < groups.Count; i++)
        {
            // 애니메이션
            var character = characterSet?.GetCharacter(groups[i].DominantClass);
            character?.PlayAttack();
            character?.PlaySkillEffect(groups[i].Length, boss.transform.position);

            boss.TakeDamage(damages[i], character.classColor);
            blockManager.RemoveGroup(groups[i]);
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
