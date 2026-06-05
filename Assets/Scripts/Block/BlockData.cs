using UnityEngine;

[CreateAssetMenu(fileName = "BlockData", menuName = "ChainKnights/BlockData")]
public class BlockData : ScriptableObject, IDisplayable
{
    public string displayName;
    public string description;
    public Sprite icon;
    public int attackPower = 10;
    public ClassType ownerClass;
    public Color blockColor = Color.white;

    public string Id => id;
    public string DisplayName => displayName;
    string IDisplayable.Description => description;
}
