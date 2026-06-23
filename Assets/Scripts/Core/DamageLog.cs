using System.Diagnostics;

// 정산 시 그룹별 데미지 로그 — 리팩토링 전후 데미지 동일성 검증용 (에디터 전용)
public static class DamageLog
{
    [Conditional("UNITY_EDITOR")]
    public static void Group(int index, ChainGroup group, int damage)
    {
        UnityEngine.Debug.Log(
            $"[DamageLog] group{index} class={group.DominantClass} len={group.Length} dmg={damage}"
        );
    }
}
