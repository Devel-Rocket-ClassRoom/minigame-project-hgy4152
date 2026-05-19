using System;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField]
    int maxHp = 100000;

    int currentHp;

    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;
    public bool IsAlive => currentHp > 0;

    public event Action<int, int> OnHpChanged;
    public event Action<int> OnDamageTaken;
    public event Action OnDefeated;

    void Awake()
    {
        currentHp = maxHp;
    }

    void Start()
    {
        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    public void TakeDamage(int amount)
    {
        if (!IsAlive || amount <= 0)
            return;

        currentHp = Mathf.Max(0, currentHp - amount);
        OnDamageTaken?.Invoke(amount);
        OnHpChanged?.Invoke(currentHp, maxHp);

        if (currentHp == 0)
            OnDefeated?.Invoke();
    }
}
