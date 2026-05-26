using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossInfoUIPanel : MonoBehaviour
{
    [SerializeField]
    GameObject panel;

    [SerializeField]
    RectTransform contentPanel;

    [SerializeField]
    Button backdropButton;

    [SerializeField]
    TMP_Text nameText;

    [SerializeField]
    TMP_Text descText;

    [SerializeField]
    TMP_Text patternText;

    Canvas _canvas;

    void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        if (panel != null)
            panel.SetActive(false);
        if (backdropButton != null)
            backdropButton.onClick.AddListener(Close);
    }

    public void Show(EnemyData data, RectTransform anchor)
    {
        if (data.bossPattern == null)
        {
            Debug.LogWarning($"[BossInfoUIPanel] {data.id} 에 bossPattern이 없습니다.");
            return;
        }

        if (nameText != null)
            nameText.text = Localization.Get(data.enemyName);
        if (descText != null)
            descText.text = Localization.Get(data.description);
        if (patternText != null)
        {
            var bp = data.bossPattern;
            int passiveCount = bp.passive != null ? bp.passive.Count : 0;
            int turnCount = 0;
            if (bp.turnModifiers != null)
                foreach (var m in bp.turnModifiers)
                    if (m != null) turnCount++;
            patternText.text = $"{bp.patternName}\n패시브: {passiveCount}개 / 턴 모디파이어: {turnCount}개";
        }

        if (panel != null)
            panel.SetActive(true);
        PositionNear(anchor);
    }

    public void Close()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    void PositionNear(RectTransform anchor)
    {
        if (contentPanel == null || _canvas == null)
            return;

        Camera cam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, anchor.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            screenPos,
            cam,
            out Vector2 localPos
        );

        float offsetY = anchor.rect.height * 0.5f + contentPanel.rect.height * 0.5f + 10f;
        Vector2 targetPos = localPos + Vector2.up * offsetY;

        var canvasRect = _canvas.transform as RectTransform;
        float halfW = contentPanel.rect.width * 0.5f;
        float halfH = contentPanel.rect.height * 0.5f;
        targetPos.x = Mathf.Clamp(targetPos.x, -canvasRect.rect.width * 0.5f + halfW, canvasRect.rect.width * 0.5f - halfW);
        targetPos.y = Mathf.Clamp(targetPos.y, -canvasRect.rect.height * 0.5f + halfH, canvasRect.rect.height * 0.5f - halfH);

        contentPanel.anchoredPosition = targetPos;
    }
}
