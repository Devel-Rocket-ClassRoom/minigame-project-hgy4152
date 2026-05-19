using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField]
    Animator anim;

    [SerializeField]
    BlockCreator creator;

    public abstract ClassType Type { get; }
    public BlockCreator Creator => creator;

    void Awake() => anim ??= GetComponentInChildren<Animator>();

    public void PlayAttack() => anim.Play("Attack");

    public virtual int ApplyPassive(ChainJudge judge, int damage) => damage;
}
