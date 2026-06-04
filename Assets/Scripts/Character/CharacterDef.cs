using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDef", menuName = "ChainKnights/CharacterDef")]
public class CharacterDef : ScriptableObject, IDisplayable
{
    public string id;
    public ClassType classType;
    public Character prefab;
    public BlockData blockData;
    public Rarity rarity;
    public string displayName;
    public string passiveName;
    public string description;
    public Sprite passiveIcon;
    public Sprite buffIcon;
    public List<UnlockCondition> unlockConditions = new();
    public string Id => id;
    public string DisplayName => displayName;
    string IDisplayable.Description => description;
}
