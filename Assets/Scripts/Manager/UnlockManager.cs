using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public enum UnlockKind
{
    Character,
    Joker,
    Enemy,
    Boss,
}

public static class UnlockManager
{
    public static event Action<UnlockKind, string> OnUnlocked;
    static readonly string[] DefaultCharacterIds =
    {
        "wa_reon",
        "ar_hikari",
        "pr_beatrice",
        "pa_victor",
        "wi_acan",
        "hu_raven",
    };
    static readonly string[] DefaultJokerIds = { "bs1", "ch1" };
    const string WarriorId = "wa_reon";
    const string WarriorJokerId = "cwar1";
    const string TestLockedCharacterId = "test_locked";

    static HashSet<string> _chars;
    static HashSet<string> _jokers;
    static HashSet<string> _enemies;
    static HashSet<string> _bosses;
    static bool _loaded;

    static string SavePath => Path.Combine(Application.persistentDataPath, "codex.json");

    public static bool IsCharacterUnlocked(string id)
    {
        EnsureLoaded();
        return _chars.Contains(id);
    }

    public static bool IsJokerUnlocked(string id)
    {
        EnsureLoaded();
        return _jokers.Contains(id);
    }

    public static bool IsEnemyUnlocked(string id)
    {
        EnsureLoaded();
        return _enemies.Contains(id);
    }

    public static bool IsBossUnlocked(string id)
    {
        EnsureLoaded();
        return _bosses.Contains(id);
    }

    public static void OnAdventureClear(string[] partyCharacterIds)
    {
        EnsureLoaded();
        if (_chars.Add(TestLockedCharacterId))
            OnUnlocked?.Invoke(UnlockKind.Character, TestLockedCharacterId);
        foreach (var id in partyCharacterIds)
            if (id == WarriorId)
            {
                if (_jokers.Add(WarriorJokerId))
                    OnUnlocked?.Invoke(UnlockKind.Joker, WarriorJokerId);
                break;
            }
        Save();
    }

    public static void OnEnemyDefeated(string enemyId)
    {
        if (string.IsNullOrEmpty(enemyId))
            return;
        EnsureLoaded();
        if (_enemies.Add(enemyId))
        {
            Save();
            OnUnlocked?.Invoke(UnlockKind.Enemy, enemyId);
        }
    }

    public static void OnBossDefeated(string bossId)
    {
        if (string.IsNullOrEmpty(bossId))
            return;
        EnsureLoaded();
        if (_bosses.Add(bossId))
        {
            Save();
            OnUnlocked?.Invoke(UnlockKind.Boss, bossId);
        }
    }

    public static void ResetAll()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
        _loaded = false;
    }

    static void EnsureLoaded()
    {
        if (_loaded)
            return;
        if (File.Exists(SavePath))
        {
            var dto = JsonUtility.FromJson<CodexData>(File.ReadAllText(SavePath));
            _chars = new HashSet<string>(dto.unlockedCharacterIds ?? Array.Empty<string>());
            _jokers = new HashSet<string>(dto.unlockedJokerIds ?? Array.Empty<string>());
            _enemies = new HashSet<string>(dto.defeatedEnemyIds ?? Array.Empty<string>());
            _bosses = new HashSet<string>(dto.defeatedBossIds ?? Array.Empty<string>());
        }
        else
        {
            _chars = new HashSet<string>(DefaultCharacterIds);
            _jokers = new HashSet<string>(DefaultJokerIds);
            _enemies = new HashSet<string>();
            _bosses = new HashSet<string>();
        }
        _loaded = true;
    }

    static void Save()
    {
        var dto = new CodexData
        {
            unlockedCharacterIds = _chars.ToArray(),
            unlockedJokerIds = _jokers.ToArray(),
            defeatedEnemyIds = _enemies.ToArray(),
            defeatedBossIds = _bosses.ToArray(),
        };
        File.WriteAllText(SavePath, JsonUtility.ToJson(dto, true));
    }

    [Serializable]
    class CodexData
    {
        public string[] unlockedCharacterIds;
        public string[] unlockedJokerIds;
        public string[] defeatedEnemyIds;
        public string[] defeatedBossIds;
    }
}
