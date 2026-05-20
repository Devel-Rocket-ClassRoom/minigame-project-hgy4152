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

    void OnEnable()
    {
        if (boss == null)
            return;
        boss.OnHpChanged += HandleHpChanged;
    }

    void OnDisable()
    {
        if (boss == null)
            return;
        boss.OnHpChanged -= HandleHpChanged;
    }

    void HandleHpChanged(int current, int max)
    {
        if (hpSlider != null)
            hpSlider.value = (float)current / max;
        if (hpText != null)
            hpText.text = $"{current} / {max}";
    }
}
