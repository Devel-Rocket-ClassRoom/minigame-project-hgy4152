using UnityEngine;

[CreateAssetMenu(fileName = "SkillCard", menuName = "ChainKnights/SkillCard")]
public class SkillCard : ScriptableObject
{
    public string id;
    public new string name;
    public string description;
    public Sprite icon;
    public TriggerEvent trigger;
    public string condition;
    public string[] effects;
    public int rarity;
}
