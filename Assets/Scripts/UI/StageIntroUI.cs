using System.Collections;
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
        gameManager.SetPaused(true);
        introText.text =
            $"{entry.chapter}{Localization.Get("ui_chapter")} {entry.stage}{Localization.Get("ui_stage")}";

        chapterText.text = $"{entry.chapter} {Localization.Get("ui_chapter")}";
        stageText.text = $"{entry.stage} {Localization.Get("ui_stage")}";

        panel.SetActive(true);
        StartCoroutine(IntroRoutine());
    }

    IEnumerator IntroRoutine()
    {
        yield return new WaitForSeconds(introDuration);
        panel.SetActive(false);
        gameManager.OnStageIntroComplete();
    }

    public IEnumerator ShowBossRoutineRoutine(Modifier mod)
    {
        if (mod == null)
            yield break;
        if (bossRoutineNameText != null)
            bossRoutineNameText.text = Localization.Get(mod.modName);
        if (bossRoutineDescText != null)
            bossRoutineDescText.text = Localization.Get(mod.description);
        if (bossRoutinePanel != null)
            bossRoutinePanel.SetActive(true);
        yield return new WaitForSeconds(bossRoutineDuration);
        if (bossRoutinePanel != null)
            bossRoutinePanel.SetActive(false);
    }

    public IEnumerator ShowMultipleRoutinesRoutine(
        System.Collections.Generic.IEnumerable<Modifier> mods
    )
    {
        foreach (var mod in mods)
            yield return StartCoroutine(ShowBossRoutineRoutine(mod));
    }

    public IEnumerator ShowTurnRoutine(int turn, int maxTurns)
    {
        introText.text = $"TURN {turn} / {maxTurns}";
        TurnText.text = $"TURN {turn} / {maxTurns}";
        panel.SetActive(true);
        yield return new WaitForSeconds(turnDisplayDuration);
        panel.SetActive(false);
    }
}
