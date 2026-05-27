#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class UnlockManagerMenu
{
    [MenuItem("ChainKnights/Codex/Reset Codex (codex.json)")]
    static void Reset()
    {
        UnlockManager.ResetAll();
        Debug.Log("[Codex] codex.json 초기화 완료");
    }

    [MenuItem("ChainKnights/Codex/Open persistentDataPath")]
    static void Open() => EditorUtility.RevealInFinder(Application.persistentDataPath);
}
#endif
