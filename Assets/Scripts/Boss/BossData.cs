using UnityEngine;

[CreateAssetMenu(fileName = "BossData", menuName = "ChainKnights/Boss/BossData")]
public class BossData : ScriptableObject, IDisplayable
{
    public string id;
    public EnemyType enemyType;
    public Rarity rarity;
    public string bossName;
    public string description;
    public Sprite icon;
    public int hp;
    public BossPattern bossPattern;

    public string Id => id;
    public string DisplayName => bossName;
    string IDisplayable.Description => description;
}
