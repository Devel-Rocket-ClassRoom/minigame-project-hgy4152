using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingDamageText : MonoBehaviour
{
    [SerializeField]
    TMP_Text label;

    [SerializeField]
    float duration = 0.8f;

    [SerializeField]
    float rise = 80f;

    public void Show(int amount)
    {
        label.text = amount.ToString();
        StartCoroutine(Float());
    }

    IEnumerator Float()
    {
        var rect = GetComponent<RectTransform>();
        Vector2 startPos = rect.anchoredPosition;
        Color startColor = label.color;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float ease = 1f - t * t;
            rect.anchoredPosition = startPos + new Vector2(0, rise * t);
            label.color = new Color(startColor.r, startColor.g, startColor.b, ease);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
