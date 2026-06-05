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
    TMP_Text debuffText;

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
        panel.gameObject.SetActive(false);

        if (backdropButton != null)
        {
            backdropButton.gameObject.SetActive(false);
            backdropButton.onClick.AddListener(Hide);
        }
    }

    public void Show(EnemyData data)
    {
        ShowInternal(data.enemyName, data.description, data.bossPattern);
    }

    public void Show(BossData data)
    {
        ShowInternal(data.bossName, data.description, data.bossPattern);
    }

    private void ShowInternal(string name, string desc, BossPattern bp)
    {
        panel.gameObject.SetActive(true);
        if (nameText != null)
            nameText.text = Localization.Get(name);
        if (descText != null)
            descText.text = Localization.Get(desc);

        if (patternText != null)
        {
            if (bp != null)
            {
                int passiveCount = bp.passive != null ? bp.passive.Count : 0;
                int turnCount = 0;
                if (bp.phaseModifiers != null)
                    foreach (var m in bp.phaseModifiers)
                        if (m != null)
                            turnCount++;
                patternText.text =
                    $"{Localization.Get(bp.patternName)}\n{string.Format(Localization.Get("ui_boss_pattern_info"), passiveCount, turnCount)}";
            }
            patternText.gameObject.SetActive(bp != null);
        }

        if (debuffText != null)
        {
            if (bp != null)
                debuffText.text = BuildDebuffText(bp);
            debuffText.gameObject.SetActive(bp != null);
        }

        if (backdropButton != null)
            backdropButton.gameObject.SetActive(true);
        Slide(_hiddenPos, _shownPos);
    }

    static string BuildDebuffText(BossPattern bp)
    {
        var sb = new System.Text.StringBuilder();

        string passiveLabel = Localization.Get("ui_label_passive");
        string turnLabel = Localization.Get("ui_label_turn");

        if (bp.passive != null)
        {
            foreach (var m in bp.passive)
            {
                if (m == null)
                    continue;
                if (sb.Length > 0)
                    sb.AppendLine();
                sb.AppendLine($"[{passiveLabel}] {Localization.Get(m.modName)}");
                sb.Append(Localization.Get(m.description));
            }
        }

        if (bp.phaseModifiers != null)
        {
            for (int i = 0; i < bp.phaseModifiers.Length; i++)
            {
                var m = bp.phaseModifiers[i];
                if (m == null)
                    continue;
                if (sb.Length > 0)
                    sb.AppendLine();
                sb.AppendLine($"[{turnLabel} {i + 1}] {Localization.Get(m.modName)}");
                sb.Append(Localization.Get(m.description));
            }
        }

        return sb.ToString();
    }

    public void Hide()
    {
        if (backdropButton != null)
            backdropButton.gameObject.SetActive(false);
        Slide(_shownPos, _hiddenPos, () => panel.gameObject.SetActive(false));
    }

    void Slide(Vector2 from, Vector2 to, System.Action onComplete = null)
    {
        if (_slide != null)
            StopCoroutine(_slide);
        _slide = StartCoroutine(SlideRoutine(from, to, onComplete));
    }

    IEnumerator SlideRoutine(Vector2 from, Vector2 to, System.Action onComplete = null)
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
        onComplete?.Invoke();
    }
}
