using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHPBarUI : MonoBehaviour
{
    [SerializeField]
    EnemyController boss;

    [SerializeField]
    BossPatternSystem bossPatternSystem;

    [SerializeField]
    Slider hpSlider;

    [SerializeField]
    TMP_Text hpText;

    [SerializeField]
    RectTransform thresholdContainer;

    [SerializeField]
    GameObject thresholdMarkerPrefab;

    void OnEnable()
    {
        if (boss != null)
            boss.OnHpChanged += HandleHpChanged;
        if (bossPatternSystem != null)
            bossPatternSystem.OnInjected += RefreshThresholdMarkers;
    }

    void OnDisable()
    {
        if (boss != null)
            boss.OnHpChanged -= HandleHpChanged;
        if (bossPatternSystem != null)
            bossPatternSystem.OnInjected -= RefreshThresholdMarkers;
    }

    void HandleHpChanged(int current, int max)
    {
        if (hpSlider != null)
            hpSlider.value = max > 0 ? (float)current / max : 0f;
        if (hpText != null)
        {
            int pct = max > 0 ? Mathf.RoundToInt((float)current / max * 100f) : 0;
            hpText.text = $"{current}/{max}({pct}%)";
        }
    }

    void RefreshThresholdMarkers()
    {
        if (thresholdContainer == null || thresholdMarkerPrefab == null)
            return;

        foreach (Transform child in thresholdContainer)
            Destroy(child.gameObject);

        var pattern = bossPatternSystem?.Current;
        if (pattern?.hpThresholds == null)
            return;

        foreach (float threshold in pattern.hpThresholds)
        {
            if (threshold <= 0f)
                continue; // 0%는 죽음, 마커 불필요

            var marker = Instantiate(thresholdMarkerPrefab, thresholdContainer);
            var rt = marker.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = new Vector2(threshold, 0f);
                rt.anchorMax = new Vector2(threshold, 1f);
                rt.anchoredPosition = Vector2.zero;
            }
        }
    }
}
