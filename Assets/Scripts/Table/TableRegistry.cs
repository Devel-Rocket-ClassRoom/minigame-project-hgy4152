using UnityEngine;

[CreateAssetMenu(fileName = "TableRegistry", menuName = "ChainKnights/Table/TableRegistry")]
public class TableRegistry : ScriptableObject
{
    public JokerCardTable JokerCard;
    public EnemyTable Enemy;
    public CharacterTable Character;
    public BossTable Boss;

    private static TableRegistry _instance;
    public static TableRegistry Instance =>
        _instance != null
            ? _instance
            : (_instance = Resources.Load<TableRegistry>("TableRegistry"));
}
