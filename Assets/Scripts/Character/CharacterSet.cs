using System.Collections.Generic;
using UnityEngine;

public class CharacterSet : MonoBehaviour
{
    [SerializeField]
    CharacterDef[] characterDefs;

    [SerializeField]
    protected Transform blockHand;

    [SerializeField]
    Transform[] heroSlots;

    Character[] instances;

    void Awake()
    {
        SortDefsByClassType();
        instances = new Character[characterDefs.Length];
        for (int i = 0; i < characterDefs.Length; i++)
        {
            Transform parent =
                (heroSlots != null && i < heroSlots.Length && heroSlots[i] != null)
                    ? heroSlots[i]
                    : transform;
            instances[i] = Instantiate(characterDefs[i].prefab, parent);
        }
    }

    void SortDefsByClassType()
    {
        if (characterDefs == null)
            return;
        System.Array.Sort(
            characterDefs,
            (a, b) =>
            {
                if (a == null && b == null)
                    return 0;
                if (a == null)
                    return 1;
                if (b == null)
                    return -1;
                return ((int)a.classType).CompareTo((int)b.classType);
            }
        );
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

    public ClassType[] GetDeployedClassTypes()
    {
        var types = new List<ClassType>();
        foreach (var inst in instances)
            if (inst != null)
                types.Add(inst.Type);
        return types.ToArray();
    }

    public void NotifyStageStart()
    {
        if (instances == null)
            return;
        foreach (var c in instances)
            if (c != null)
                c.OnStageStart();
    }

    public string[] GetCurrentCharacterIds()
    {
        var ids = new string[characterDefs.Length];
        for (int i = 0; i < characterDefs.Length; i++)
            ids[i] = characterDefs[i] != null ? characterDefs[i].id : "";
        return ids;
    }

    public void SetCharactersByIds(string[] ids)
    {
        var reg = TableRegistry.Instance;
        if (reg == null || reg.Character == null)
        {
            Debug.LogWarning("[CharacterSet] CharacterTable을 찾을 수 없어 로드를 건너뜁니다.");
            return;
        }

        foreach (var inst in instances)
            if (inst != null)
                Destroy(inst.gameObject);

        characterDefs = new CharacterDef[ids.Length];
        for (int i = 0; i < ids.Length; i++)
        {
            if (string.IsNullOrEmpty(ids[i]))
                continue;
            characterDefs[i] = reg.Character.Get(ids[i]);
        }

        SortDefsByClassType();

        instances = new Character[characterDefs.Length];
        for (int i = 0; i < characterDefs.Length; i++)
        {
            if (characterDefs[i] == null)
                continue;
            Transform parent =
                (heroSlots != null && i < heroSlots.Length && heroSlots[i] != null)
                    ? heroSlots[i]
                    : transform;
            instances[i] = Instantiate(characterDefs[i].prefab, parent);
        }
    }
}
