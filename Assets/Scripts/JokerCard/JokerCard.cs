using UnityEngine;

public abstract class JokerCard : ScriptableObject
{
    public string id;
    public string cardName;
    public string description;
    public Sprite icon;
    public int rarity;

    public abstract int GetBonus(ChainJudge judge);
}
