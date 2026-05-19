using UnityEngine;

public class CharacterSet : MonoBehaviour
{
    [SerializeField]
    WarriorCreator warriorCreator;

    [SerializeField]
    ArcherCreator archerCreator;

    [SerializeField]
    PriestCreator priestCreator;

    [SerializeField]
    protected Transform blockHand;

    void Awake()
    {
        // ?? 연산자 : 없으면 탐색해서 할당
        warriorCreator ??= GetComponent<WarriorCreator>();
        archerCreator ??= GetComponent<ArcherCreator>();
        priestCreator ??= GetComponent<PriestCreator>();
    }

    public BlockCreator GetCreator(ClassType classType) =>
        classType switch
        {
            ClassType.Warrior => warriorCreator,
            ClassType.Archer => archerCreator,
            ClassType.Priest => priestCreator,
            _ => null,
        };

    // 해당 직업의 크리에이터의 CreateBlock 실행
    public Block CreateBlock(ClassType classType, Transform parent = null) =>
        GetCreator(classType)?.CreateBlock(parent != null ? parent : blockHand);
}
