using UnityEngine;

[CreateAssetMenu(fileName = "BlockData", menuName = "ChainKnights/BlockData")]
public class BlockData : ScriptableObject
{
    public int id;
    public Sprite icon;
    public int attackPower = 10;
    public AnimationClip animationClip;
    public ClassType ownerClass;
}
