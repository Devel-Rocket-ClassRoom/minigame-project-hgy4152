using UnityEngine;

public class CharacterSet : MonoBehaviour
{
    [SerializeField]
    WarriorCreator warriorCreator;

    [SerializeField]
    ArcherCreator archerCreator;

    [SerializeField]
    PriestCreator priestCreator;

    public BlockCreator GetCreator(ClassType classType) =>
        classType switch
        {
            ClassType.Warrior => warriorCreator,
            ClassType.Archer => archerCreator,
            ClassType.Priest => priestCreator,
            _ => null,
        };

    public Block CreateBlock(ClassType classType) => GetCreator(classType)?.CreateBlock();
}
