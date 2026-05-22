using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDef", menuName = "ChainKnights/CharacterDef")]
public class CharacterDef : ScriptableObject, IDisplayable
{
    public string id;
    public ClassType classType;
    public Character prefab;
    public string displayName;
    public string description;
    public Sprite icon;

    public string Id => id;
    public string DisplayName => displayName;
    string IDisplayable.Description => description;
}
