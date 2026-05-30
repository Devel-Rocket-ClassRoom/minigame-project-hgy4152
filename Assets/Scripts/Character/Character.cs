using DG.Tweening;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
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
    protected int _hitEventIndex;
    int[] _perHitDamages;
    EnemyController _target;

    protected Vector3 _idlePos;

    void Start()
    {
        _idlePos = transform.localPosition;
        StartBreathing();
    }

    protected void StartBreathing()
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
    }

    public virtual int ApplyPassive(ChainJudge judge, ChainGroup group, int damage) => damage;

    public virtual void OnStageStart() { }
}
