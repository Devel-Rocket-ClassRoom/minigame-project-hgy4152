using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    [SerializeField]
    BossController boss;

    [SerializeField]
    Slider hpSlider;

    [SerializeField]
    TMP_Text hpText;

    [SerializeField]
    GameObject clearText;

    [SerializeField]
    FloatingDamageText damageTextPrefab;

    [SerializeField]
    RectTransform damageSpawnRoot;

    void OnEnable()
    {
        if (boss == null)
            return;
        boss.OnHpChanged += HandleHpChanged;
        boss.OnDamageTaken += HandleDamageTaken;
        boss.OnDefeated += HandleDefeated;
    }

    void OnDisable()
    {
        if (boss == null)
            return;
        boss.OnHpChanged -= HandleHpChanged;
        boss.OnDamageTaken -= HandleDamageTaken;
        boss.OnDefeated -= HandleDefeated;
    }

    void HandleHpChanged(int current, int max)
    {
        if (hpSlider != null)
            hpSlider.value = (float)current / max;
        if (hpText != null)
            hpText.text = $"{current} / {max}";
    }

    void HandleDamageTaken(int amount)
    {
        if (damageTextPrefab == null || damageSpawnRoot == null)
            return;
        var instance = Instantiate(damageTextPrefab, damageSpawnRoot);
        instance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        instance.Show(amount);
    }

    void HandleDefeated()
    {
        if (clearText != null)
            clearText.SetActive(true);
    }
}
