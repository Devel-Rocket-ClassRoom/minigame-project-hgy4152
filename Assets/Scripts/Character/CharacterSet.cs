using UnityEngine;

public class CharacterSet : MonoBehaviour
{
    [SerializeField]
    Character[] characterPrefabs;

    [SerializeField]
    protected Transform blockHand;

    Character[] instances;

    void Awake()
    {
        instances = new Character[characterPrefabs.Length];
        for (int i = 0; i < characterPrefabs.Length; i++)
        {
            instances[i] = Instantiate(characterPrefabs[i], transform);
        }
    }

    public Character GetCharacter(ClassType classType)
    {
        foreach (var c in instances)
            if (c.Type == classType)
                return c;
        return null;
    }

    public Block CreateBlock(ClassType classType, Transform parent = null) =>
        GetCharacter(classType)?.Creator?.CreateBlock(parent != null ? parent : blockHand);
}
