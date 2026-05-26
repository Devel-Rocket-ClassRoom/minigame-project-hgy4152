using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossInfoUIPanel : MonoBehaviour
{
    [SerializeField]
    RectTransform panel;

    [SerializeField]
    Button backdropButton;

    [SerializeField]
    TMP_Text nameText;

    [SerializeField]
    TMP_Text descText;

    [SerializeField]
    TMP_Text patternText;

    [SerializeField]
    float slideDuration = 0.25f;

    Vector2 _shownPos;
    Vector2 _hiddenPos;
    Coroutine _slide;

    void Awake()
    {
        _shownPos = panel.anchoredPosition;
        _hiddenPos = _shownPos - new Vector2(panel.sizeDelta.x, 0);
        panel.anchoredPosition = _hiddenPos;

        if (backdropButton != null)
        {
            backdropButton.gameObject.SetActive(false);
            backdropButton.onClick.AddListener(Hide);
        }
    }

    public void Show(EnemyData data)
    {
        if (nameText != null)
            nameText.text = Localization.Get(data.enemyName);
        if (descText != null)
            descText.text = Localization.Get(data.description);

        if (patternText != null)
        {
            var bp = data.bossPattern;
            if (bp != null)
            {
                int passiveCount = bp.passive != null ? bp.passive.Count : 0;
                int turnCount = 0;
                if (bp.turnModifiers != null)
                    foreach (var m in bp.turnModifiers)
                        if (m != null) turnCount++;
                patternText.text = $"{bp.patternName}\n패시브: {passiveCount}개 / 턴 모디파이어: {turnCount}개";
            }
            patternText.gameObject.SetActive(bp != null);
        }

        if (backdropButton != null)
            backdropButton.gameObject.SetActive(true);
        Slide(_hiddenPos, _shownPos);
    }

    public void Hide()
    {
        if (backdropButton != null)
            backdropButton.gameObject.SetActive(false);
        Slide(_shownPos, _hiddenPos);
    }

    void Slide(Vector2 from, Vector2 to)
    {
        if (_slide != null)
            StopCoroutine(_slide);
        _slide = StartCoroutine(SlideRoutine(from, to));
    }

    IEnumerator SlideRoutine(Vector2 from, Vector2 to)
    {
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / slideDuration);
            panel.anchoredPosition = Vector2.LerpUnclamped(from, to, t);
            yield return null;
        }
        panel.anchoredPosition = to;
    }
}
