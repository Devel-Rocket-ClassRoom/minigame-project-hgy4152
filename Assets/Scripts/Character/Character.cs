using System.Collections;
using DG.Tweening;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    protected Skill skill;

    [Header("=== 타격 타이밍 (공격 시작 후 초) ===")]
    [SerializeField]
    float[] hitTimings = { 0.15f, 0.35f, 0.55f };

    public abstract ClassType Type { get; }
    public abstract Color classColor { get; }
    public BlockCreator Creator { get; private set; }
    public Sprite Icon => GetComponent<SpriteRenderer>().sprite;
    public Vector3 IdlePos => _idlePos;

    protected int _chainCount;
    protected float scaleFactor = 1f;
    protected Vector3 _targetPos;
    protected int _hitEventIndex;
    int[] _perHitDamages;
    EnemyController _target;
    Coroutine _hitCoroutine;

    protected Vector3 _idlePos;

    void Awake()
    {
        skill = GetComponent<Skill>();
        Creator = GetComponent<BlockCreator>();
    }

    void Start()
    {
        _idlePos = transform.localPosition;
        StartBreathing();
    }

    void OnDisable()
    {
        if (_hitCoroutine != null)
            StopCoroutine(_hitCoroutine);
    }

    public void StartBreathing()
    {
        transform
            .DOLocalMoveY(_idlePos.y + 0.03f, 0.5f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void PlayAttack(Vector3 targetPos)
    {
        _targetPos = targetPos;
        DOTween.Kill(transform);
    }

    public virtual void PlaySkillEffect(
        int chainCount,
        int[] perHitDamages = null,
        EnemyController target = null
    )
    {
        _chainCount = chainCount;
        _perHitDamages = perHitDamages;
        _target = target;
        _hitEventIndex = 0;

        scaleFactor = chainCount switch
        {
            1 => 1f,
            2 => 1.5f,
            _ => 2f,
        };

        if (_hitCoroutine != null)
            StopCoroutine(_hitCoroutine);
        _hitCoroutine = StartCoroutine(RunHitTimings());
    }

    protected virtual bool IsSingleCast => false;

    IEnumerator RunHitTimings()
    {
        if (IsSingleCast && _chainCount > 1 && _perHitDamages != null)
        {
            int total = 0;
            foreach (int d in _perHitDamages)
                total += d;
            _perHitDamages = new int[] { total };
            _chainCount = 1;
        }

        float prev = 0f;
        int count = Mathf.Min(_chainCount, hitTimings.Length);
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(hitTimings[i] - prev);
            prev = hitTimings[i];
            OnChainHitEvent();
        }
    }

    public virtual void OnChainHitEvent()
    {
        _hitEventIndex++;
        if (_hitEventIndex > _chainCount)
            return;

        if (skill != null)
        {
            switch (_hitEventIndex)
            {
                case 1:
                    skill.Chain1(_targetPos, scaleFactor);
                    break;
                case 2:
                    skill.Chain2(_targetPos, scaleFactor);
                    break;
                case 3:
                    skill.Chain3(_targetPos, scaleFactor);
                    break;
            }
        }

        int idx = _hitEventIndex - 1;
        if (_target != null && _perHitDamages != null && idx >= 0 && idx < _perHitDamages.Length)
            _target.TakeDamage(_perHitDamages[idx], classColor);

        if (_hitEventIndex == _chainCount)
            OnLastChainHitComplete();
    }

    protected virtual void OnLastChainHitComplete() { }

    public virtual int ApplyPassive(ChainJudge judge, ChainGroup group, int damage) => damage;

    public virtual void OnStageStart() { }

    public virtual void OnAnyGroupDamageApplied(int rawDamage, int finalDamage) { }

    public virtual void OnTurnProcessed(bool wasThisCharacterUsed) { }

    public virtual void OnAfterGroupPlayed(CharacterSet characterSet, ChainGroup group) { }

    public virtual int GetBonusAttackCount(ChainJudge judge, ChainGroup group) => 0;

    public virtual void OnTurnSequenceEnd() { }

    public virtual void OnAnyGroupAttackStart(ChainGroup group, EnemyController target) { }
}
