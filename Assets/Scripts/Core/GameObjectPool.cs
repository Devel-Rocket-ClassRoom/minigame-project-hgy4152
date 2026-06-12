using System.Collections.Generic;
using UnityEngine;

// 프리팹 키 기반 공용 오브젝트 풀 (데미지 텍스트, 스킬 이펙트 등 Instantiate/Destroy 반복 대체)
public static class GameObjectPool
{
    class PooledMarker : MonoBehaviour
    {
        public GameObject prefabKey;
    }

    static readonly Dictionary<GameObject, Stack<GameObject>> _pools = new();

    public static T Get<T>(T prefab, Transform parent)
        where T : Component => Get(prefab.gameObject, parent).GetComponent<T>();

    public static GameObject Get(GameObject prefab, Transform parent)
    {
        if (!_pools.TryGetValue(prefab, out var stack))
            _pools[prefab] = stack = new Stack<GameObject>();

        GameObject instance = null;
        while (stack.Count > 0 && instance == null)
            instance = stack.Pop(); // 씬 전환으로 파괴된 인스턴스는 건너뜀

        if (instance == null)
        {
            instance = Object.Instantiate(prefab, parent);
            instance.AddComponent<PooledMarker>().prefabKey = prefab;
        }
        else
        {
            instance.transform.SetParent(parent, false);
            instance.SetActive(true);
        }
        return instance;
    }

    public static void Release(GameObject instance)
    {
        var marker = instance.GetComponent<PooledMarker>();
        if (marker == null || marker.prefabKey == null)
        {
            Object.Destroy(instance); // 풀 출신이 아니면 기존 동작 유지
            return;
        }
        instance.SetActive(false);
        _pools[marker.prefabKey].Push(instance);
    }
}
