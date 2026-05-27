using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject, IDisplayable
{
    public string id;
    public EnemyType enemyType;
    public Rarity rarity;
    public string enemyName;
    public string description;
    public Sprite icon;
    public int hp;
    public BossPattern bossPattern;

    public string Id => id;
    public string DisplayName => enemyName;
    string IDisplayable.Description => description;
}
