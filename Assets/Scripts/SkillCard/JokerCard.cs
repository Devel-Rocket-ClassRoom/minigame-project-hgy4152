using UnityEngine;

[CreateAssetMenu(fileName = "JokerCard", menuName = "ChainKnights/JokerCard")]
public class JokerCard : ScriptableObject
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
