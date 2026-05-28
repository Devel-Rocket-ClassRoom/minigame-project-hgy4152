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
        instances = new Character[ids.Length];
        for (int i = 0; i < ids.Length; i++)
        {
            if (string.IsNullOrEmpty(ids[i]))
                continue;
            var def = reg.Character.Get(ids[i]);
            if (def == null)
                continue;
            characterDefs[i] = def;
            Transform parent =
                (heroSlots != null && i < heroSlots.Length && heroSlots[i] != null)
                    ? heroSlots[i]
                    : transform;
            instances[i] = Instantiate(def.prefab, parent);
        }
    }
}
