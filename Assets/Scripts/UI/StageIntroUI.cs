using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageIntroUI : MonoBehaviour
{
    [SerializeField]
    GameObject panel;

    [SerializeField]
    TMP_Text introText;

    [SerializeField]
    TMP_Text TurnText;

    [SerializeField]
    TMP_Text stageText;

    [SerializeField]
    TMP_Text chapterText;

    [SerializeField]
    GameObject bossRoutinePanel;

    [SerializeField]
    TMP_Text bossRoutineNameText;

    [SerializeField]
    TMP_Text bossRoutineDescText;

    [SerializeField]
    float introDuration = 2f;

    [SerializeField]
    float turnDisplayDuration = 1f;

    [SerializeField]
    float bossRoutineDuration = 1.5f;

    [SerializeField]
    StageManager stageManager;

    [SerializeField]
    GameManager gameManager;

    void OnEnable()
    {
        stageManager.OnStageStart += HandleStageStart;
    }

    void OnDisable()
    {
        stageManager.OnStageStart -= HandleStageStart;
    }

    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
        if (bossRoutinePanel != null)
            bossRoutinePanel.SetActive(false);
    }

    void HandleStageStart(StageManager.StageEntry entry)
    {
        if (gameManager.IsBossPlay)
        {
            gameManager.OnStageIntroComplete();
            return;
        }

        gameManager.SetPaused(true);
        introText.text =
            $"{entry.chapter}{Localization.Get("ui_chapter")} {entry.stage}{Localization.Get("ui_stage")}";

        chapterText.text = $"{entry.chapter} {Localization.Get("ui_chapter")}";
        stageText.text = $"{entry.stage} {Localization.Get("ui_stage")}";

        panel.SetActive(true);
        IntroAsync().Forget();
    }

    async UniTaskVoid IntroAsync()
    {
        await UniTask.Delay(
            TimeSpan.FromSeconds(introDuration),
            cancellationToken: this.GetCancellationTokenOnDestroy()
        );
        panel.SetActive(false);
        gameManager.OnStageIntroComplete();
    }

    public async UniTask ShowBossRoutineAsync(Modifier mod)
    {
        if (mod == null)
            return;
        if (bossRoutineNameText != null)
            bossRoutineNameText.text = Localization.Get(mod.modName);
        if (bossRoutineDescText != null)
            bossRoutineDescText.text = Localization.Get(mod.description);
        if (bossRoutinePanel != null)
            bossRoutinePanel.SetActive(true);
        await UniTask.Delay(
            TimeSpan.FromSeconds(bossRoutineDuration),
            cancellationToken: this.GetCancellationTokenOnDestroy()
        );
        if (bossRoutinePanel != null)
            bossRoutinePanel.SetActive(false);
    }

    public async UniTask ShowMultipleRoutinesAsync(
        System.Collections.Generic.IEnumerable<Modifier> mods
    )
    {
        foreach (var mod in mods)
            await ShowBossRoutineAsync(mod);
    }

    public async UniTask ShowTurnAsync(int turn, int maxTurns)
    {
        introText.text = $"TURN {turn} / {maxTurns}";
        TurnText.text = $"TURN {turn} / {maxTurns}";
        panel.SetActive(true);
        await UniTask.Delay(
            TimeSpan.FromSeconds(turnDisplayDuration),
            cancellationToken: this.GetCancellationTokenOnDestroy()
        );
        panel.SetActive(false);
    }
}
