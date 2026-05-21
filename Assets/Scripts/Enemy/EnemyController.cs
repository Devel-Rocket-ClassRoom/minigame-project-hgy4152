using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    int maxHp = 100000;

    [SerializeField]
    SpriteRenderer enemySprite;

    [SerializeField]
    Image enemyPortrait;

    [SerializeField]
    FloatingDamageText damageTextPrefab;

    [SerializeField]
    RectTransform damageSpawnRoot;

    int currentHp;

    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;
    public bool IsAlive => currentHp > 0;

    public event Action<int, int> OnHpChanged;
    public event Action<int> OnDamageTaken;
    public event Action OnDefeated;

    protected virtual void Awake()
    {
        currentHp = maxHp;
    }

    protected virtual void Start()
    {
        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    public void Init(EnemyData data)
    {
        maxHp = data.hp;
        currentHp = data.hp;
        enemySprite.sprite = data.icon;
        enemyPortrait.sprite = data.icon;
        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    public void TakeDamage(int amount, Color color)
    {
        if (amount <= 0)
            return;

        SpawnDamageText(amount, color);

        if (!IsAlive)
            return;

        currentHp = Mathf.Max(0, currentHp - amount);
        OnDamageTaken?.Invoke(amount);
        OnHpChanged?.Invoke(currentHp, maxHp);

        if (currentHp == 0)
            OnDefeated?.Invoke();
    }

    void SpawnDamageText(int amount, Color color)
    {
        if (damageTextPrefab == null || damageSpawnRoot == null)
            return;
        var instance = Instantiate(damageTextPrefab, damageSpawnRoot);
        instance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        instance.Show(amount, color);
    }
}
