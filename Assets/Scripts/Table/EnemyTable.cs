using UnityEngine;

[CreateAssetMenu(fileName = "EnemyTable", menuName = "ChainKnights/Table/EnemyTable")]
public class EnemyTable : StringTable<EnemyData>
{
    private void OnEnable()
    {
        entries.Clear();
        entries.AddRange(Resources.LoadAll<EnemyData>("Enemies"));
    }
}
