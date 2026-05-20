using UnityEngine;

public abstract class JokerCard : ScriptableObject
{
    public string id;
    public string cardName;
    public string description;
    public Sprite icon;
    public int rarity;

    public abstract int GetBonus(ChainJudge judge); // 그룹 보너스
    public abstract float DeckBonus(ChainJudge judge); // 핸드 보너스

}
