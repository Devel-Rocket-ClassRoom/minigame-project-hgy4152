using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    [SerializeField]
    EnemyController boss;

    [SerializeField]
    Slider hpSlider;

    [SerializeField]
    TMP_Text hpText;

    [SerializeField]
    TMP_Text turnDamageText;

    void OnEnable()
    {
        if (boss == null)
            return;
        boss.OnHpChanged += HandleHpChanged;

        var gm = FindAnyObjectByType<GameManager>();
        if (gm != null)
            gm.OnTurnDamageChanged += HandleTurnDamageChanged;
    }

    void OnDisable()
    {
        if (boss == null)
            return;
        boss.OnHpChanged -= HandleHpChanged;

        var gm = FindAnyObjectByType<GameManager>();
        if (gm != null)
            gm.OnTurnDamageChanged -= HandleTurnDamageChanged;
    }

    void HandleHpChanged(int current, int max)
    {
        if (hpSlider != null)
            hpSlider.value = (float)current / max;
        if (hpText != null)
            hpText.text = $"{current} / {max}";
    }

    void HandleTurnDamageChanged(int total)
    {
        if (turnDamageText != null)
            turnDamageText.text = $"{Localization.Get("ui_turn_damage_label")}: {total}";
    }
}
