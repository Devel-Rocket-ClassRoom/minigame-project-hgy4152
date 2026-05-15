using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockData", menuName = "ChainKnights/BlockData")]
public class BlockData : ScriptableObject
{
    public string id;
    public Sprite icon;
    public int attackPower = 10;
    public AnimatorController animationClip;
    public ClassType ownerClass;
}
