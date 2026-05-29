using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockNotificationSpawner : MonoBehaviour
{
    [SerializeField]
    UnlockNotificationUI prefab;

    [SerializeField]
    RectTransform spawnParent;

    Queue<(string text, Sprite icon)> _queue = new();
    bool _running;

    void OnEnable() => UnlockManager.OnUnlocked += HandleUnlock;

    void OnDisable() => UnlockManager.OnUnlocked -= HandleUnlock;

    void HandleUnlock(UnlockKind kind, string id)
    {
        string text = ResolveText(kind, id);
        if (string.IsNullOrEmpty(text))
            return;
        _queue.Enqueue((text, ResolveIcon(kind, id)));
        if (!_running)
            StartCoroutine(RunQueue());
    }

    IEnumerator RunQueue()
    {
        _running = true;
        while (_queue.Count > 0)
        {
            var (text, icon) = _queue.Dequeue();
            var popup = Instantiate(prefab, spawnParent);
            yield return StartCoroutine(popup.Play(text, icon));
        }
        _running = false;
    }

    string ResolveText(UnlockKind kind, string id)
    {
        var reg = TableRegistry.Instance;
        if (reg == null)
            return null;

        string nameKey = kind switch
        {
            UnlockKind.Character => reg.Character?.Get(id)?.displayName,
            UnlockKind.Joker => reg.JokerCard?.Get(id)?.cardName,
            UnlockKind.Enemy => reg.Enemy?.Get(id)?.enemyName,
            UnlockKind.Boss => reg.Boss?.Get(id)?.bossName,
            _ => null,
        };

        if (string.IsNullOrEmpty(nameKey))
            return null;

        return $"{Localization.Get(nameKey)} {Localization.Get("ui_unlock_suffix")}";
    }

    Sprite ResolveIcon(UnlockKind kind, string id)
    {
        var reg = TableRegistry.Instance;
        if (reg == null)
            return null;

        return kind switch
        {
            UnlockKind.Character => reg.Character?.Get(id)?.prefab?.Icon,
            UnlockKind.Joker => reg.JokerCard?.Get(id)?.icon,
            UnlockKind.Enemy => reg.Enemy?.Get(id)?.icon,
            UnlockKind.Boss => reg.Boss?.Get(id)?.icon,
            _ => null,
        };
    }
}
