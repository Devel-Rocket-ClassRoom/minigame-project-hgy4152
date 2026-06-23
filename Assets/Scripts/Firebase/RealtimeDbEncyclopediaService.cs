using System;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

public static class RealtimeDbEncyclopediaService
{
    public static async UniTask PushAsync(EncyclopediaCloudData data, string userId)
    {
        try
        {
            var reference = FirebaseDatabase.DefaultInstance.GetReference($"codex/{userId}");
            await reference.SetRawJsonValueAsync(JsonUtility.ToJson(data)).AsUniTask();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[DB] Push failed: {e.Message}");
        }
    }

    public static async UniTask<EncyclopediaCloudData> PullAsync(string userId)
    {
        try
        {
            var reference = FirebaseDatabase.DefaultInstance.GetReference($"codex/{userId}");
            var snapshot = await reference.GetValueAsync().AsUniTask();
            if (!snapshot.Exists) return null;
            return JsonUtility.FromJson<EncyclopediaCloudData>(snapshot.GetRawJsonValue());
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[DB] Pull failed: {e.Message}");
            return null;
        }
    }
}
