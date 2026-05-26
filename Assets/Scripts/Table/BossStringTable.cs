using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossStringTable", menuName = "ChainKnights/Table/BossStringTable")]
public class BossStringTable : ScriptableObject
{
    [SerializeField]
    private List<BossPattern> patterns = new();

    [SerializeField]
    private List<Modifier> skills = new();

    private Dictionary<string, BossPattern> _patternCache;
    private Dictionary<string, Modifier> _skillCache;

    private void EnsureCache()
    {
        if (_patternCache != null)
            return;

        _patternCache = new Dictionary<string, BossPattern>();
        foreach (var p in patterns)
        {
            if (p == null || string.IsNullOrEmpty(p.Id))
                continue;
            if (_patternCache.ContainsKey(p.Id))
            {
                Debug.LogWarning($"[BossStringTable] 중복 패턴 ID: {p.Id}", this);
                continue;
            }
            _patternCache[p.Id] = p;
        }

        _skillCache = new Dictionary<string, Modifier>();
        foreach (var s in skills)
        {
            if (s == null || string.IsNullOrEmpty(s.Id))
                continue;
            if (_skillCache.ContainsKey(s.Id))
            {
                Debug.LogWarning($"[BossStringTable] 중복 스킬 ID: {s.Id}", this);
                continue;
            }
            _skillCache[s.Id] = s;
        }
    }

    public BossPattern GetPattern(string id)
    {
        EnsureCache();
        _patternCache.TryGetValue(id, out var entry);
        return entry;
    }

    public Modifier GetSkill(string id)
    {
        EnsureCache();
        _skillCache.TryGetValue(id, out var entry);
        return entry;
    }

    public IReadOnlyList<BossPattern> AllPatterns => patterns;
    public IReadOnlyList<Modifier> AllSkills => skills;

#if UNITY_EDITOR
    private void OnValidate()
    {
        _patternCache = null;
        _skillCache = null;
    }
#endif
}
