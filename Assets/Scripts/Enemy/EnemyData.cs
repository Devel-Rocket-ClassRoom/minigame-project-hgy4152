using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string id;
    public string enemyName;
    public string description;
    public Sprite icon;
    public int hp;
    public BossPattern bossPattern;
}
