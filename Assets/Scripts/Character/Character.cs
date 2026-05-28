using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("=== 캐릭터 프리펩 제작 시 필수요소 ===")]
    [SerializeField]
    Animator anim;

    [Header("=== 돌진 이동 ===")]
    [SerializeField]
    Vector3 chargeMoveTarget;

    [SerializeField]
    BlockCreator creator;

    [SerializeField]
    Sprite icon;

    [SerializeField]
    Skill skill;

    public abstract ClassType Type { get; }
    public abstract Color classColor { get; }
    public BlockCreator Creator => creator;
    public Sprite Icon => icon;

    protected int _chainCount;
    protected float scaleFactor = 1f;
    protected Vector3 _targetPos;
    int _hitEventIndex;

    public void PlayAttack() => anim.SetTrigger("Attack");

    public virtual void PlaySkillEffect(int chainCount, Vector3 targetPos)
    {
        _chainCount = chainCount;

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

        _targetPos = targetPos;
        _hitEventIndex = 0;
    }

    // Animation Event에서 호출 — hit 순번에 맞는 Chain 메소드 실행
    public void OnChainHitEvent()
    {
        _hitEventIndex++;
        if (_hitEventIndex > _chainCount || skill == null)
            return;

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

    // Animation Event에서 호출 — chargeMoveTarget(로컬 좌표)으로 이동
    public void OnChargeMoveEvent() => transform.localPosition = chargeMoveTarget;

    public virtual int ApplyPassive(ChainJudge judge, ChainGroup group, int damage) => damage;

    public virtual void OnStageStart() { }
}
