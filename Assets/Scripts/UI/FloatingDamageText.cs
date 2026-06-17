using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingDamageText : MonoBehaviour
{
    [SerializeField]
    TMP_Text label;

    [SerializeField]
    float popDuration = 0.15f;

    [SerializeField]
    float fadeDuration = 0.5f;
    [SerializeField]
    float startfadeDuration = 0.5f;

    [SerializeField]
    float peakScale = 1.4f;

    [SerializeField]
    float spawnRadius = 50f;

    public void Show(int amount, Color color)
    {
        label.text = amount.ToString();
        label.color = new Color(color.r, color.g, color.b, 0f);
        var rect = GetComponent<RectTransform>();
        rect.anchoredPosition += (Vector2)Random.insideUnitCircle * spawnRadius;
        StartCoroutine(Stamp());
    }

    IEnumerator Stamp()
    {
        var rect = GetComponent<RectTransform>();
        Color baseColor = label.color;

        // 팝인: 작게 시작해서 peakScale까지 커지며 나타남
        float elapsed = 0f;
        while (elapsed < popDuration)
        {
            float t = elapsed / popDuration;
            rect.localScale = Vector3.one * Mathf.SmoothStep(0.3f, peakScale, t);
            label.color = new Color(baseColor.r, baseColor.g, baseColor.b, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(startfadeDuration);

        // 페이드아웃: 수축하면서 사라짐
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;
            rect.localScale = Vector3.one * Mathf.Lerp(peakScale, 0.8f, t);
            label.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f - t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        GameObjectPool.Release(gameObject);
    }
}
