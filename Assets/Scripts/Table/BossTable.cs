using UnityEngine;

[CreateAssetMenu(fileName = "BossTable", menuName = "ChainKnights/Table/BossTable")]
public class BossTable : StringTable<BossData>
{
    private void OnEnable()
    {
        entries.Clear();
        entries.AddRange(Resources.LoadAll<BossData>("Bosses"));
    }
}
