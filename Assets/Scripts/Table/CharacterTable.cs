using UnityEngine;

[CreateAssetMenu(fileName = "CharacterTable", menuName = "ChainKnights/Table/CharacterTable")]
public class CharacterTable : StringTable<CharacterDef>
{
    private void OnEnable()
    {
        entries.Clear();
        entries.AddRange(Resources.LoadAll<CharacterDef>("Characters"));
    }
}
