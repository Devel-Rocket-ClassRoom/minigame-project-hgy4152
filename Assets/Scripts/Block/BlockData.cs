using UnityEngine;

[CreateAssetMenu(fileName = "BlockData", menuName = "ChainKnights/BlockData")]
public class BlockData : ScriptableObject
{
    public string id;
    public Sprite icon;
    public int attackPower = 10;
    public ClassType ownerClass;
    public Color blockColor = Color.white;
}
