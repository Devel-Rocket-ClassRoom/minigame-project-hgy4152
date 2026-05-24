using System.Collections.Generic;
using UnityEngine;

public abstract class StringTable<T> : ScriptableObject
    where T : ScriptableObject, IIdentifiable
{
    [SerializeField]
    protected List<T> entries = new();

    private Dictionary<string, T> _cache;

    private void EnsureCache()
    {
        if (_cache != null)
            return;
        _cache = new Dictionary<string, T>();
        foreach (var e in entries)
        {
            if (e == null || string.IsNullOrEmpty(e.Id))
                continue;
            if (_cache.ContainsKey(e.Id))
            {
                Debug.LogWarning($"[{GetType().Name}] 중복 ID: {e.Id}", this);
                continue;
            }
            _cache[e.Id] = e;
        }
    }

    public T Get(string id)
    {
        EnsureCache();
        _cache.TryGetValue(id, out var entry);
        return entry;
    }

    public bool TryGet(string id, out T entry)
    {
        EnsureCache();
        return _cache.TryGetValue(id, out entry);
    }

    public IReadOnlyList<T> All => entries;

    public string GetName(string id) =>
        Get(id) is IDisplayable d ? Localization.Get(d.DisplayName) : id;

    public string GetDescription(string id) =>
        Get(id) is IDisplayable d ? Localization.Get(d.Description) : string.Empty;

#if UNITY_EDITOR
    private void OnValidate() => _cache = null;
#endif
}
