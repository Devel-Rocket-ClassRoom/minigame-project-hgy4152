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

    static HashSet<string> _chars;
    static HashSet<string> _jokers;
    static HashSet<string> _enemies;
    static HashSet<string> _bosses;
    static bool _adventureCleared;
    static HashSet<string> _clearedWithChars;
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
        _adventureCleared = true;
        foreach (var id in partyCharacterIds)
            _clearedWithChars.Add(id);
        CheckAndUnlockAll();
        Save();
    }

    public static void OnEnemyDefeated(string enemyId)
    {
        if (string.IsNullOrEmpty(enemyId))
            return;
        EnsureLoaded();
        if (!_enemies.Add(enemyId))
            return;
        OnUnlocked?.Invoke(UnlockKind.Enemy, enemyId);
        CheckAndUnlockAll();
        Save();
    }

    public static void OnBossDefeated(string bossId)
    {
        if (string.IsNullOrEmpty(bossId))
            return;
        EnsureLoaded();
        if (!_bosses.Add(bossId))
            return;
        OnUnlocked?.Invoke(UnlockKind.Boss, bossId);
        CheckAndUnlockAll();
        Save();
    }

    public static void ResetAll()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
        _loaded = false;
    }

    static void CheckAndUnlockAll()
    {
        var reg = TableRegistry.Instance;
        if (reg == null)
            return;

        if (reg.Character != null)
            foreach (var def in reg.Character.All)
            {
                if (def == null || def.unlockConditions.Count == 0)
                    continue;
                if (_chars.Contains(def.id))
                    continue;
                if (!AllConditionsMet(def.unlockConditions))
                    continue;
                if (_chars.Add(def.id))
                    OnUnlocked?.Invoke(UnlockKind.Character, def.id);
            }

        if (reg.JokerCard != null)
            foreach (var card in reg.JokerCard.All)
            {
                if (card == null || card.unlockConditions.Count == 0)
                    continue;
                if (_jokers.Contains(card.id))
                    continue;
                if (!AllConditionsMet(card.unlockConditions))
                    continue;
                if (_jokers.Add(card.id))
                    OnUnlocked?.Invoke(UnlockKind.Joker, card.id);
            }
    }

    static bool AllConditionsMet(List<UnlockCondition> conditions)
    {
        foreach (var c in conditions)
        {
            bool met = c.type switch
            {
                UnlockConditionType.DefeatEnemy => _enemies.Contains(c.targetId),
                UnlockConditionType.DefeatBoss => _bosses.Contains(c.targetId),
                UnlockConditionType.ClearAdventure => _adventureCleared,
                UnlockConditionType.ClearWithCharacter => _clearedWithChars.Contains(c.targetId),
                _ => false,
            };
            if (!met)
                return false;
        }
        return true;
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
            _adventureCleared = dto.adventureCleared;
            _clearedWithChars = new HashSet<string>(
                dto.clearedWithCharacterIds ?? Array.Empty<string>()
            );
        }
        else
        {
            _chars = new HashSet<string>(DefaultCharacterIds);
            _jokers = new HashSet<string>(DefaultJokerIds);
            _enemies = new HashSet<string>();
            _bosses = new HashSet<string>();
            _adventureCleared = false;
            _clearedWithChars = new HashSet<string>();
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
            adventureCleared = _adventureCleared,
            clearedWithCharacterIds = _clearedWithChars.ToArray(),
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
        public bool adventureCleared;
        public string[] clearedWithCharacterIds;
    }
}
