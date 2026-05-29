using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("=== 돌진 이동 ===")]
    [SerializeField]
    float chargeDuration = 0.3f;

    [SerializeField]
    Vector3 chargeStopOffset;

    [Header("=== 캐릭터 프리펩 제작 시 필수요소 ===")]
    [SerializeField]
    Animator anim;

    [SerializeField]
    BlockCreator creator;

    [SerializeField]
    Sprite icon;

    [SerializeField]
    protected Skill skill;

    public abstract ClassType Type { get; }
    public abstract Color classColor { get; }
    public BlockCreator Creator => creator;
    public Sprite Icon => icon;

    protected int _chainCount;
    protected float scaleFactor = 1f;
    protected Vector3 _targetPos;
    int _hitEventIndex;
    int[] _perHitDamages;
    EnemyController _target;

    bool _isDashing;
    float _dashElapsed;
    Vector3 _dashStart;
    Vector3 _dashEnd;

    public void PlayAttack(Vector3 targetPos)
    {
        _targetPos = targetPos;
        anim.SetTrigger("Attack");
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

        switch (chainCount)
        {
            case 1:
                scaleFactor = 1f;
                break;
            case 2:
                scaleFactor = 1.5f;
                break;
            case 3:
                scaleFactor = 2f;
                break;
        }

        _hitEventIndex = 0;
    }

    // Animation Event에서 호출 — hit 순번에 맞는 Chain 메소드 실행
    public void OnChainHitEvent()
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
    }

    // Animation Event에서 호출 — _targetPos로 부드럽게 이동
    // LateUpdate에서 transform을 쓰기 때문에 Idle 호흡 애니메이션(m_PositionCurves)을 덮어쓰는 Animator보다 뒤에서 적용된다
    public void OnChargeStartEvent()
    {
        _dashStart = transform.localPosition;
        _dashEnd =
            (
                transform.parent != null
                    ? transform.parent.InverseTransformPoint(_targetPos)
                    : _targetPos
            ) + chargeStopOffset;
        _dashElapsed = 0f;
        _isDashing = true;
    }

    void LateUpdate()
    {
        if (!_isDashing)
            return;
        _dashElapsed += Time.deltaTime;
        float t = chargeDuration > 0f ? Mathf.Clamp01(_dashElapsed / chargeDuration) : 1f;
        transform.localPosition = Vector3.Lerp(_dashStart, _dashEnd, t);
        if (t >= 1f)
            _isDashing = false;
    }

    public virtual int ApplyPassive(ChainJudge judge, ChainGroup group, int damage) => damage;

    public virtual void OnStageStart() { }
}
