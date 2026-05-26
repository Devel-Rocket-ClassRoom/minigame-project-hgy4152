using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossInfoUIPanel : MonoBehaviour
{
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

    void Awake()
    {
        if (backdropButton != null)
            backdropButton.onClick.AddListener(() => Destroy(gameObject));
    }

    public void Init(EnemyData data, RectTransform anchor)
    {
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

        if (contentPanel != null)
            contentPanel.localPosition = contentPanel.parent.InverseTransformPoint(anchor.position);
    }

    public static BossInfoUIPanel Show(BossInfoUIPanel prefab, EnemyData data, RectTransform anchor)
    {
        if (prefab == null || data.bossPattern == null) return null;
        var canvas = anchor.GetComponentInParent<Canvas>();
        if (canvas == null) return null;
        var instance = Instantiate(prefab, canvas.transform);
        instance.Init(data, anchor);
        return instance;
    }
}
