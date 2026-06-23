using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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

    static readonly string[] DefaultCharacterIds = { "wa1", "ar1", "pr1", "pa1", "wi1", "hu1" };
    static readonly (string bossId, string prerequisiteId)[] BossUnlockChain =
    {
        ("bss2", "bss1"),
        ("bss3", "bss2"),
    };
    static readonly string[] DefaultJokerIds =
    {
        "cwar1",
        "cpal1",
        "cwiz1",
        "carc1",
        "chun1",
        "cpri1",
        "ch1",
        "ch2",
        "ch3",
        "spd1",
        "solo1",
        "prev1",
        "shift1",
        "trich",
    };

    static HashSet<string> _chars;
    static HashSet<string> _jokers;
    static HashSet<string> _enemies;
    static HashSet<string> _bosses;

    static int _adventureClearCount;
    static int _bossModeClearCount;
    static int _chain1Used;
    static int _chain2Used;
    static int _chain3Used;
    static int _blocksDiscarded;
    static Dictionary<ClassType, int> _classClearCounts;

    static bool _initialized;
    static string _savedUserId = "";

    public static bool IsCharacterUnlocked(string id)
    {
        EnsureInitialized();
        return _chars.Contains(id);
    }

    public static bool IsJokerUnlocked(string id)
    {
        EnsureInitialized();
        return _jokers.Contains(id);
    }

    public static bool IsEnemyUnlocked(string id)
    {
        EnsureInitialized();
        return _enemies.Contains(id);
    }

    public static bool IsBossPlayable(string id)
    {
        EnsureInitialized();
        foreach (var (bossId, prereq) in BossUnlockChain)
            if (bossId == id)
                return _bosses.Contains(prereq);
        return true;
    }

    public static bool IsBossUnlocked(string id)
    {
        EnsureInitialized();
        return _bosses.Contains(id);
    }

    public static void OnAdventureClear(string[] partyCharacterIds)
    {
        EnsureInitialized();
        _adventureClearCount++;
        RecordClassClears(partyCharacterIds);
        CheckAndUnlockAll();
        PushToCloudAsync().Forget();
    }

    public static void OnBossModeClear(string[] partyCharacterIds)
    {
        EnsureInitialized();
        _bossModeClearCount++;
        RecordClassClears(partyCharacterIds);
        CheckAndUnlockAll();
        PushToCloudAsync().Forget();
    }

    public static void RecordChainUsed(int chainLength)
    {
        EnsureInitialized();
        if (chainLength == 1)
            _chain1Used++;
        else if (chainLength == 2)
            _chain2Used++;
        else if (chainLength >= 3)
            _chain3Used++;
        CheckAndUnlockAll();
        PushToCloudAsync().Forget();
    }

    public static void RecordBlocksDiscarded(int count)
    {
        EnsureInitialized();
        _blocksDiscarded += count;
        CheckAndUnlockAll();
        PushToCloudAsync().Forget();
    }

    public static void OnEnemyDefeated(string enemyId)
    {
        if (string.IsNullOrEmpty(enemyId))
            return;
        EnsureInitialized();
        if (_enemies.Add(enemyId))
        {
            PushToCloudAsync().Forget();
            OnUnlocked?.Invoke(UnlockKind.Enemy, enemyId);
        }
    }

    public static void OnBossDefeated(string bossId)
    {
        if (string.IsNullOrEmpty(bossId))
            return;
        EnsureInitialized();
        if (_bosses.Add(bossId))
        {
            PushToCloudAsync().Forget();
            OnUnlocked?.Invoke(UnlockKind.Boss, bossId);
        }
    }

    public static void PrepareForUser(string userId)
    {
        if (_savedUserId == userId) return;
        _chars = new HashSet<string>(DefaultCharacterIds);
        _jokers = new HashSet<string>(DefaultJokerIds);
        _enemies = new HashSet<string>();
        _bosses = new HashSet<string>();
        _adventureClearCount = 0;
        _bossModeClearCount = 0;
        _chain1Used = 0;
        _chain2Used = 0;
        _chain3Used = 0;
        _blocksDiscarded = 0;
        _classClearCounts = new Dictionary<ClassType, int>();
        _savedUserId = userId;
        _initialized = true;
    }

    public static void ResetAll()
    {
        _chars = new HashSet<string>(DefaultCharacterIds);
        _jokers = new HashSet<string>(DefaultJokerIds);
        _enemies = new HashSet<string>();
        _bosses = new HashSet<string>();
        _adventureClearCount = 0;
        _bossModeClearCount = 0;
        _chain1Used = 0;
        _chain2Used = 0;
        _chain3Used = 0;
        _blocksDiscarded = 0;
        _classClearCounts = new Dictionary<ClassType, int>();
        _initialized = false;
        _savedUserId = "";
    }

    public static EncyclopediaCloudData ToCloudData()
    {
        EnsureInitialized();
        return new EncyclopediaCloudData
        {
            unlockedCharacterIds = new List<string>(_chars),
            unlockedJokerIds = new List<string>(_jokers),
            defeatedEnemyIds = new List<string>(_enemies),
            defeatedBossIds = new List<string>(_bosses),
            adventureClearCount = _adventureClearCount,
            bossModeClearCount = _bossModeClearCount,
            chain1Used = _chain1Used,
            chain2Used = _chain2Used,
            chain3Used = _chain3Used,
            blocksDiscarded = _blocksDiscarded,
            classClearCounts = _classClearCounts
                .Select(kv => new ClassClearEntryCloud { classType = kv.Key.ToString(), count = kv.Value })
                .ToList(),
        };
    }

    public static void MergeFromCloud(EncyclopediaCloudData cloud)
    {
        if (cloud == null) return;
        EnsureInitialized();

        if (cloud.unlockedCharacterIds != null) foreach (var id in cloud.unlockedCharacterIds) _chars.Add(id);
        if (cloud.unlockedJokerIds != null) foreach (var id in cloud.unlockedJokerIds) _jokers.Add(id);
        if (cloud.defeatedEnemyIds != null) foreach (var id in cloud.defeatedEnemyIds) _enemies.Add(id);
        if (cloud.defeatedBossIds != null)
        {
            foreach (var id in MigrateBossIds(cloud.defeatedBossIds.ToArray()))
                _bosses.Add(id);
        }

        _adventureClearCount = Mathf.Max(_adventureClearCount, cloud.adventureClearCount);
        _bossModeClearCount = Mathf.Max(_bossModeClearCount, cloud.bossModeClearCount);
        _chain1Used = Mathf.Max(_chain1Used, cloud.chain1Used);
        _chain2Used = Mathf.Max(_chain2Used, cloud.chain2Used);
        _chain3Used = Mathf.Max(_chain3Used, cloud.chain3Used);
        _blocksDiscarded = Mathf.Max(_blocksDiscarded, cloud.blocksDiscarded);

        if (cloud.classClearCounts != null)
            foreach (var e in cloud.classClearCounts)
                if (Enum.TryParse<ClassType>(e.classType, out var ct))
                {
                    _classClearCounts.TryGetValue(ct, out int cur);
                    _classClearCounts[ct] = Mathf.Max(cur, e.count);
                }

        MigrateCharIds(_chars);
        CheckAndUnlockAll();
        PushToCloudAsync().Forget();
    }

    static void RecordClassClears(string[] partyCharacterIds)
    {
        var reg = TableRegistry.Instance;
        if (reg?.Character == null)
            return;
        foreach (var id in partyCharacterIds)
        {
            var def = reg.Character.Get(id);
            if (def == null)
                continue;
            _classClearCounts.TryGetValue(def.classType, out int cur);
            _classClearCounts[def.classType] = cur + 1;
        }
    }

    static void CheckAndUnlockAll()
    {
        var reg = TableRegistry.Instance;
        if (reg == null)
            return;

        if (reg.Character != null)
            foreach (var def in reg.Character.All)
            {
                if (def == null || def.unlockConditions.Count == 0 || _chars.Contains(def.id))
                    continue;
                if (AllConditionsMet(def.unlockConditions))
                    if (_chars.Add(def.id))
                        OnUnlocked?.Invoke(UnlockKind.Character, def.id);
            }

        if (reg.JokerCard != null)
            foreach (var card in reg.JokerCard.All)
            {
                if (card == null || card.unlockConditions.Count == 0 || _jokers.Contains(card.id))
                    continue;
                if (AllConditionsMet(card.unlockConditions))
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
                UnlockConditionType.AdventureClear => _adventureClearCount >= c.count,
                UnlockConditionType.BossModeClear => _bossModeClearCount >= c.count,
                UnlockConditionType.Chain1Used => _chain1Used >= c.count,
                UnlockConditionType.Chain2Used => _chain2Used >= c.count,
                UnlockConditionType.Chain3Used => _chain3Used >= c.count,
                UnlockConditionType.BlocksDiscarded => _blocksDiscarded >= c.count,
                UnlockConditionType.UnlockedJokerCount => _jokers.Count >= c.count,
                UnlockConditionType.UnlockedCharacterCount => _chars.Count >= c.count,
                UnlockConditionType.ClearWithClass => GetClassClearCount(c.classType) >= c.count,
                _ => false,
            };
            if (!met)
                return false;
        }
        return true;
    }

    static int GetClassClearCount(ClassType ct)
    {
        _classClearCounts.TryGetValue(ct, out int v);
        return v;
    }

    static void EnsureInitialized()
    {
        if (!_initialized)
            Debug.LogWarning("[UnlockManager] PrepareForUser() has not been called yet.");
    }

    static readonly Dictionary<string, string> CharIdMigration = new()
    {
        { "wa_reon", "wa1" },
        { "wa3", "wa2" },
        { "ar_hikari", "ar1" },
        { "ar3", "ar2" },
        { "hu_raven", "hu1" },
        { "hu_ahnmansik", "hu2" },
        { "pa_victor", "pa1" },
        { "pa3", "pa2" },
        { "wi_acan", "wi1" },
        { "wi_zyum", "wi2" },
        { "pr_beatrice", "pr1" },
        { "pr_selmu", "pr2" },
    };

    static readonly Dictionary<string, string> BossIdMigration = new()
    {
        { "boss_geumsuabi", "bss1" },
        { "boss_blackmage", "bss2" },
        { "boss_chaoslord", "bss3" },
    };

    static void MigrateCharIds(HashSet<string> ids)
    {
        var old = new List<string>(ids.Where(id => CharIdMigration.ContainsKey(id)));
        foreach (var o in old)
        {
            ids.Remove(o);
            ids.Add(CharIdMigration[o]);
        }
    }

    static IEnumerable<string> MigrateBossIds(string[] ids)
    {
        foreach (var id in ids)
            yield return BossIdMigration.TryGetValue(id, out var newId) ? newId : id;
    }

    static async UniTaskVoid PushToCloudAsync()
    {
        var user = AuthManager.Instance?.CurrentUser;
        if (user == null) return;
        await RealtimeDbEncyclopediaService.PushAsync(ToCloudData(), user.UserId);
    }
}
