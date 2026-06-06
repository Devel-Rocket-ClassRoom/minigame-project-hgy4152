using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingEffectText : MonoBehaviour
{
    [SerializeField]
    TMP_Text label;

    [SerializeField]
    float riseDuration = 0.4f;

    [SerializeField]
    float holdDuration = 0.6f;

    [SerializeField]
    float fadeDuration = 0.5f;

    [SerializeField]
    float riseDistance = 80f;

    public float TotalDuration => riseDuration + holdDuration + fadeDuration;

    public void Show(string content)
    {
        label.text = content;
        StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayRoutine()
    {
        var startPos = transform.localPosition;
        var endPos = startPos + Vector3.up * riseDistance;
        var color = label.color;

        // 위로 올라오며 등장
        for (float t = 0; t < riseDuration; t += Time.deltaTime)
        {
            transform.localPosition = Vector3.Lerp(startPos, endPos, t / riseDuration);
            yield return null;
        }
        transform.localPosition = endPos;

        yield return new WaitForSeconds(holdDuration);

        // 페이드 아웃
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            color.a = 1f - t / fadeDuration;
            label.color = color;
            yield return null;
        }

        Destroy(gameObject);
    }
}
