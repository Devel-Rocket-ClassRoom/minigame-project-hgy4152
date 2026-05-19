using UnityEngine;

public class CharacterSet : MonoBehaviour
{
    [SerializeField]
    WarriorCharacter warriorCharacter;

    [SerializeField]
    ArcherCharacter archerCharacter;

    [SerializeField]
    PriestCharacter priestCharacter;

    [SerializeField]
    protected Transform blockHand;

    public Character GetCharacter(ClassType classType) =>
        classType switch
        {
            ClassType.Warrior => warriorCharacter,
            ClassType.Archer => archerCharacter,
            ClassType.Priest => priestCharacter,
            _ => null,
        };

    public Block CreateBlock(ClassType classType, Transform parent = null) =>
        GetCharacter(classType)?.Creator?.CreateBlock(parent != null ? parent : blockHand);
}
