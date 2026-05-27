using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockNotificationSpawner : MonoBehaviour
{
    [SerializeField]
    UnlockNotificationUI prefab;

    [SerializeField]
    RectTransform spawnParent;

    Queue<string> _queue = new();
    bool _running;

    void OnEnable() => UnlockManager.OnUnlocked += HandleUnlock;

    void OnDisable() => UnlockManager.OnUnlocked -= HandleUnlock;

    void HandleUnlock(UnlockKind kind, string id)
    {
        string text = ResolveText(kind, id);
        if (string.IsNullOrEmpty(text))
            return;
        _queue.Enqueue(text);
        if (!_running)
            StartCoroutine(RunQueue());
    }

    IEnumerator RunQueue()
    {
        _running = true;
        while (_queue.Count > 0)
        {
            string text = _queue.Dequeue();
            var popup = Instantiate(prefab, spawnParent);
            yield return StartCoroutine(popup.Play(text));
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

        return $"{Localization.Get(nameKey)} 해금!";
    }
}
