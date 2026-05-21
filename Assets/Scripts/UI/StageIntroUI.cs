using System.Collections;
using TMPro;
using UnityEngine;

public class StageIntroUI : MonoBehaviour
{
    [SerializeField]
    GameObject panel;

    [SerializeField]
    TMP_Text introText;

    [SerializeField]
    TMP_Text stageText;

    [SerializeField]
    TMP_Text chapterText;

    [SerializeField]
    float introDuration = 2f;

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
        if (panel != null) panel.SetActive(false);
    }

    void HandleStageStart(StageManager.StageEntry entry)
    {
        gameManager.SetPaused(true);
        introText.text = $"{entry.chapter}챕터 {entry.stage}스테이지";

        chapterText.text = $"{entry.chapter} 챕터";
        stageText.text = $"{entry.stage} 스테이지";

        panel.SetActive(true);
        StartCoroutine(IntroRoutine());
    }

    IEnumerator IntroRoutine()
    {
        yield return new WaitForSeconds(introDuration);
        panel.SetActive(false);
        gameManager.BeginBattle();
    }
}
