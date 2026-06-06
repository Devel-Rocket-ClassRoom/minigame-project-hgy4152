using System.Collections;
using UnityEngine;

public class PhaseEffectSpawner : MonoBehaviour
{
    [SerializeField]
    Transform effectSpawnPoint;

    [SerializeField]
    FloatingEffectText floatingTextPrefab;

    [SerializeField]
    DebuffIconBarUI debuffIconBarUI;

    public IEnumerator PlayEffect(GameObject effectPrefab, string floatText)
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
        yield return StartCoroutine(WaitForEffectComplete(fx));
        if (fx != null)
            Destroy(fx);
    }

    IEnumerator WaitForEffectComplete(GameObject fx)
    {
        if (fx == null)
            yield break;

        var ps = fx.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            yield return new WaitUntil(() => fx == null || !ps.IsAlive(true));
            yield break;
        }

        var anim = fx.GetComponent<Animation>();
        if (anim != null && anim.clip != null)
        {
            yield return new WaitForSeconds(anim.clip.length);
            yield break;
        }

        yield return new WaitForSeconds(1.5f);
    }
}
