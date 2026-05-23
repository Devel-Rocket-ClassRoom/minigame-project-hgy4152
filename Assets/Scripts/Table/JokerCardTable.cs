using UnityEngine;

[CreateAssetMenu(fileName = "JokerCardTable", menuName = "ChainKnights/Table/JokerCardTable")]
public class JokerCardTable : StringTable<JokerCard>
{
    private void OnEnable()
    {
        entries.Clear();
        entries.AddRange(Resources.LoadAll<JokerCard>("JokerCards"));
    }
}
