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
    public BlockCreator Creator => creator;
    public Sprite Icon => icon;

    public void PlayAttack() => anim.SetTrigger("Attack");

    public virtual int ApplyPassive(ChainJudge judge, int damage) => damage;
}
