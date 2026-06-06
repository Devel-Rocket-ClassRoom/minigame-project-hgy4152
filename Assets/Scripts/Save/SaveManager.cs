using System;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public const int SlotCount = 3;

    public bool HasSlot(int slot) => File.Exists(GetPath(slot));

    public bool TryLoad(int slot, out SaveSlotData data)
    {
        var path = GetPath(slot);
        if (!File.Exists(path))
        {
            data = null;
            return false;
        }
        data = JsonUtility.FromJson<SaveSlotData>(File.ReadAllText(path));
        return data != null;
    }

    public void Save(int slot, SaveSlotData data)
    {
        var path = GetPath(slot);
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, JsonUtility.ToJson(data, true));
        Debug.Log($"[Save] 슬롯 {slot} 저장 완료: {path}");
    }

    public void Delete(int slot)
    {
        var path = GetPath(slot);
        if (File.Exists(path))
            File.Delete(path);
    }

    public int FindFirstEmptySlot()
    {
        for (int i = 0; i < SlotCount; i++)
            if (!HasSlot(i))
                return i;
        return -1;
    }

    public static void DeleteAll()
    {
        for (int i = 0; i < SlotCount; i++)
        {
            var path = Path.Combine(Application.persistentDataPath, $"save_{i}.json");
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    public SaveSlotData BuildFromCurrentState(GameManager gm)
    {
        var data = new SaveSlotData { clearedAtIso = DateTime.UtcNow.ToString("o") };

        var ids = gm.CharacterSet.GetCurrentCharacterIds();
        for (int i = 0; i < data.characterIds.Length; i++)
            data.characterIds[i] = i < ids.Length ? ids[i] : "";

        var hand = gm.JokerManager.ActiveHand;
        for (int i = 0; i < data.jokerIds.Length; i++)
            data.jokerIds[i] =
                (hand != null && i < hand.Length && hand[i] != null) ? hand[i].id : "";

        return data;
    }

    private string GetPath(int slot) =>
        Path.Combine(Application.persistentDataPath, $"save_{slot}.json");
}
