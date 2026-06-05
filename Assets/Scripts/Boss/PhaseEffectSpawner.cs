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
        var fx = Instantiate(effectPrefab, effectSpawnPoint.position, Quaternion.identity);

        // 플로팅 텍스트 소환
        if (floatingTextPrefab != null && !string.IsNullOrEmpty(floatText))
        {
            var floatInst = Instantiate(floatingTextPrefab, effectSpawnPoint);
            floatInst.Show(floatText);
            yield return new WaitForSeconds(floatInst.TotalDuration);
        }

        // 플로팅 텍스트 종료 시점에 디버프 아이콘 갱신
        debuffIconBarUI?.Refresh();

        // 이펙트 완료 대기
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
