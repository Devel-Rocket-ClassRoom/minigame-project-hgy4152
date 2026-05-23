using System.Collections.Generic;
using UnityEngine;

public static class Localization
{
    static Dictionary<string, string> _map;

    public static string Get(string key)
    {
        if (string.IsNullOrEmpty(key))
            return key;
        EnsureLoaded();
        return _map.TryGetValue(key, out var v) && !string.IsNullOrEmpty(v) ? v : key;
    }

    static void EnsureLoaded()
    {
        if (_map != null)
            return;
        _map = new Dictionary<string, string>();
        var asset = Resources.Load<TextAsset>("StringTable");
        if (asset == null)
        {
            Debug.LogWarning("[Localization] Resources/StringTable.csv 를 찾을 수 없습니다.");
            return;
        }
        ParseCsv(asset.text, _map);
    }

    static void ParseCsv(string text, Dictionary<string, string> dict)
    {
        var lines = text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].TrimEnd('\r');
            if (string.IsNullOrWhiteSpace(line))
                continue;
            int comma = line.IndexOf(',');
            if (comma < 0)
                continue;
            var key = line.Substring(0, comma);
            var value = line.Substring(comma + 1);
            if (value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"')
                value = value.Substring(1, value.Length - 2).Replace("\"\"", "\"");
            dict[key] = value;
        }
    }
}
