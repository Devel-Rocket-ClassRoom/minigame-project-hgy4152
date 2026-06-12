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

    GameManager _gameManager;

    void Awake()
    {
        // 씬 검색은 1회만 — 구독/해제는 캐시된 참조로 (해제 시 재검색 실패로 구독이 남는 문제 방지)
        _gameManager = FindAnyObjectByType<GameManager>();
    }

    void OnEnable()
    {
        if (boss == null)
            return;
        boss.OnHpChanged += HandleHpChanged;

        if (_gameManager != null)
            _gameManager.OnTurnDamageChanged += HandleTurnDamageChanged;
    }

    void OnDisable()
    {
        if (boss == null)
            return;
        boss.OnHpChanged -= HandleHpChanged;

        if (_gameManager != null)
            _gameManager.OnTurnDamageChanged -= HandleTurnDamageChanged;
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
            turnDamageText.text = $"{total} damage!!";
    }
}
