using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossInfoUIPanel : MonoBehaviour
{
    [SerializeField]
    GameObject panel;

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
        if (panel != null)
            panel.SetActive(false);
        if (backdropButton != null)
            backdropButton.onClick.AddListener(Close);
    }

    public void Show(EnemyData data)
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
    }

    public void Close()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
