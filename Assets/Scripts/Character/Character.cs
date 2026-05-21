using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField]
    Animator anim;

    [SerializeField]
    BlockCreator creator;

    [SerializeField]
    Sprite icon;

    public abstract ClassType Type { get; }
    public abstract Color classColor { get; }
    public BlockCreator Creator => creator;
    public Sprite Icon => icon;

    public void PlayAttack() => anim.SetTrigger("Attack");

    public virtual void PlaySkillEffect(int chainCount) { }

    public virtual int ApplyPassive(ChainJudge judge, ChainGroup group, int damage) => damage;
}
