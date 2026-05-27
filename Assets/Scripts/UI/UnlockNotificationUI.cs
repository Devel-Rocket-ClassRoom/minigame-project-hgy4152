using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockNotificationUI : MonoBehaviour
{
    [SerializeField]
    Image iconImage;

    [SerializeField]
    TMP_Text nameText;

    [SerializeField]
    float slideInDuration = 0.3f;

    [SerializeField]
    float displayDuration = 1.5f;

    [SerializeField]
    float slideOutDuration = 0.3f;

    [SerializeField]
    float offscreenX = 400f;

    [SerializeField]
    float onscreenX = 0f;

    RectTransform _rect;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _rect.anchoredPosition = new Vector2(offscreenX, _rect.anchoredPosition.y);
    }

    public IEnumerator Play(string text, Sprite icon)
    {
        if (nameText != null)
            nameText.text = text;
        if (iconImage != null)
            iconImage.sprite = icon;

        float y = _rect.anchoredPosition.y;

        for (float t = 0; t < slideInDuration; t += Time.unscaledDeltaTime)
        {
            _rect.anchoredPosition = new Vector2(
                Mathf.Lerp(offscreenX, onscreenX, t / slideInDuration),
                y
            );
            yield return null;
        }
        _rect.anchoredPosition = new Vector2(onscreenX, y);

        yield return new WaitForSecondsRealtime(displayDuration);

        for (float t = 0; t < slideOutDuration; t += Time.unscaledDeltaTime)
        {
            _rect.anchoredPosition = new Vector2(
                Mathf.Lerp(onscreenX, offscreenX, t / slideOutDuration),
                y
            );
            yield return null;
        }

        Destroy(gameObject);
    }
}
