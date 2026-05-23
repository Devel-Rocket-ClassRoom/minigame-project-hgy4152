using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField]
    Animator anim;

    [SerializeField]
    BlockCreator creator;

    [SerializeField]
    Sprite icon;

    [SerializeField]
    Skill skill;

    public string characterName;

    [TextArea]
    public string passiveDescription;

    public string CharacterName => characterName;
    public string PassiveDescription => passiveDescription;
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

    public virtual int ApplyPassive(ChainJudge judge, ChainGroup group, int damage) => damage;
}
