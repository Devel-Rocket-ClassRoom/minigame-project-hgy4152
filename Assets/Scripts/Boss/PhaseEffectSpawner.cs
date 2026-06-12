using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PhaseEffectSpawner : MonoBehaviour
{
    [SerializeField]
    Transform effectSpawnPoint;

    [SerializeField]
    FloatingEffectText floatingTextPrefab;

    [SerializeField]
    DebuffIconBarUI debuffIconBarUI;

    public async UniTask PlayEffectAsync(GameObject effectPrefab, string floatText)
    {
        // 플로팅 텍스트 소환 (fire-and-forget)
        if (floatingTextPrefab != null && !string.IsNullOrEmpty(floatText))
        {
            var floatInst = Instantiate(floatingTextPrefab, effectSpawnPoint);
            floatInst.Show(floatText);
        }

        // 이펙트 + 디버프 아이콘 + 플로팅 텍스트 동시
        var fx = Instantiate(effectPrefab, effectSpawnPoint.position, Quaternion.identity);
        debuffIconBarUI?.Refresh();
        await WaitForEffectComplete(fx);
        if (fx != null)
            Destroy(fx);
    }

    async UniTask WaitForEffectComplete(GameObject fx)
    {
        if (fx == null)
            return;

        var ct = this.GetCancellationTokenOnDestroy();

        var ps = fx.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            await UniTask.WaitUntil(() => fx == null || !ps.IsAlive(true), cancellationToken: ct);
            return;
        }

        var anim = fx.GetComponent<Animation>();
        if (anim != null && anim.clip != null)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(anim.clip.length), cancellationToken: ct);
            return;
        }

        await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: ct);
    }
}
