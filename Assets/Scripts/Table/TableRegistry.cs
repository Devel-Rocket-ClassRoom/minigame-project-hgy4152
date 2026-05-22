using UnityEngine;

[CreateAssetMenu(fileName = "TableRegistry", menuName = "ChainKnights/Table/TableRegistry")]
public class TableRegistry : ScriptableObject
{
    public BlockTable Block;
    public JokerCardTable JokerCard;
    public EnemyTable Enemy;

    private static TableRegistry _instance;
    public static TableRegistry Instance =>
        _instance != null
            ? _instance
            : (_instance = Resources.Load<TableRegistry>("TableRegistry"));
}
