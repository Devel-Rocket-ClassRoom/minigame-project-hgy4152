using UnityEngine;

public class BossController : EnemyController
{
    protected override void PlayHitEffect(int damage)
    {
        // TODO: 보스 피격 파티클/흔들림 이펙트 재생
        Debug.Log($"[Boss] HitEffect damage={damage}");
    }
}
