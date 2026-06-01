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
    Button enemyPortraitButton;

    [SerializeField]
    FloatingDamageText damageTextPrefab;

    [SerializeField]
    RectTransform damageSpawnRoot;

    [SerializeField]
    BossInfoUIPanel enemyInfoPanel;

    EnemyData _enemyData;
    BossData _bossData;

    Canvas _canvas;
    Camera _uiCamera;

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
        if (enemyPortraitButton != null)
            enemyPortraitButton.onClick.AddListener(OnPortraitClicked);

        if (damageSpawnRoot != null)
        {
            _canvas = damageSpawnRoot.GetComponentInParent<Canvas>();
            if (_canvas != null)
                _uiCamera =
                    _canvas.renderMode == RenderMode.ScreenSpaceOverlay
                        ? null
                        : _canvas.worldCamera;
        }
    }

    void LateUpdate()
    {
        if (enemySprite == null || damageSpawnRoot == null || _canvas == null)
            return;

        Bounds bounds = enemySprite.bounds;
        Vector3 worldTop = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(_uiCamera, worldTop);

        if (
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)damageSpawnRoot.parent,
                screenPoint,
                _uiCamera,
                out Vector2 localPoint
            )
        )
        {
            damageSpawnRoot.localPosition = localPoint;
        }
    }

    protected virtual void Start()
    {
        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    public void Init(EnemyData data)
    {
        _enemyData = data;
        _bossData = null;
        maxHp = data.hp;
        currentHp = data.hp;
        enemySprite.sprite = data.icon;
        enemyPortrait.sprite = data.icon;
        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    public void Init(BossData data)
    {
        _bossData = data;
        _enemyData = null;
        maxHp = data.hp;
        currentHp = data.hp;
        enemySprite.sprite = data.icon;
        enemyPortrait.sprite = data.icon;
        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    void OnPortraitClicked()
    {
        if (_bossData != null)
            enemyInfoPanel?.Show(_bossData);
        else if (_enemyData != null)
            enemyInfoPanel?.Show(_enemyData);
    }

    public void TakeDamage(int amount, Color color)
    {
        if (amount <= 0)
            return;

        SpawnDamageText(amount, color);
        PlayHitEffect(amount);

        if (!IsAlive)
            return;

        currentHp = Mathf.Max(0, currentHp - amount);
        OnDamageTaken?.Invoke(amount);
        OnHpChanged?.Invoke(currentHp, maxHp);

        if (currentHp == 0)
            OnDefeated?.Invoke();
    }

    protected virtual void PlayHitEffect(int damage) { }

    void SpawnDamageText(int amount, Color color)
    {
        if (damageTextPrefab == null || damageSpawnRoot == null)
            return;
        var instance = Instantiate(damageTextPrefab, damageSpawnRoot);
        instance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        instance.Show(amount, color);
    }
}
