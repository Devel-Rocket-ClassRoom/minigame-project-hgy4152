using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    public BlockData data;
    public int chainGroupId = -1;

    [SerializeField]
    Sprite backgroundSprite;

    Image _sprite;
    Image _background;

    static Sprite _whiteSprite;

    LayoutElement _layoutElement;
    RectTransform _rectTransform;

    void Awake()
    {
        EnsureInitialized();
    }

    void EnsureInitialized()
    {
        if (_rectTransform != null)
            return;

        _rectTransform = GetComponent<RectTransform>();
        _layoutElement = GetComponent<LayoutElement>();
        _background = CreateBackground();
        _sprite = CreateSpriteImage();

        var button = GetComponent<Button>();

        if (button != null)
        {
            button.targetGraphic = _background;
            button.onClick.AddListener(OnClicked);
        }
    }

    public void FlyIn(Vector2 targetLocalPos, float duration, Action onComplete = null)
    {
        if (_layoutElement != null)
            _layoutElement.ignoreLayout = true;

        StartCoroutine(FlyInCoroutine(targetLocalPos, duration, onComplete));
    }

    public void Slide(Vector2 targetLocalPos, float duration, Action onComplete = null)
    {
        if (_layoutElement != null)
            _layoutElement.ignoreLayout = true;

        StartCoroutine(
            SlideCoroutine(_rectTransform.anchoredPosition, targetLocalPos, duration, onComplete)
        );
    }

    IEnumerator SlideCoroutine(
        Vector2 startPos,
        Vector2 targetLocalPos,
        float duration,
        Action onComplete
    )
    {
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * (2 - t);
            _rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetLocalPos, t);
            yield return null;
        }

        _rectTransform.anchoredPosition = targetLocalPos;
        if (_layoutElement != null)
            _layoutElement.ignoreLayout = false;

        yield return new WaitForEndOfFrame();
        onComplete?.Invoke();
    }

    IEnumerator FlyInCoroutine(Vector2 targetLocalPos, float duration, System.Action onComplete)
    {
        Vector2 startPos = targetLocalPos + new Vector2(1000, 0);
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * (2 - t); // EaseOutQuad
            _rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetLocalPos, t);
            yield return null;
        }

        _rectTransform.anchoredPosition = targetLocalPos;
        if (_layoutElement != null)
            _layoutElement.ignoreLayout = false;

        yield return new WaitForEndOfFrame();
        onComplete?.Invoke();
    }

    public IEnumerator HighlightPulseRoutine(float scale = 1.2f, float duration = 0.2f)
    {
        Vector3 originalScale = _rectTransform.localScale;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float s =
                t < 0.5f
                    ? Mathf.SmoothStep(1f, scale, t * 2f)
                    : Mathf.SmoothStep(scale, 1f, (t - 0.5f) * 2f);
            _rectTransform.localScale = originalScale * s;
            yield return null;
        }
        _rectTransform.localScale = originalScale;
    }

    public void SetChainVisual(int chainCount)
    {
        _background.color = data.blockColor;
    }

    public Action<Block> OnDiscardRequested;

    void OnClicked()
    {
        OnDiscardRequested?.Invoke(this);
    }

    public void Init(BlockData blockData)
    {
        EnsureInitialized();
        data = blockData;
        if (blockData.icon != null)
            _sprite.sprite = blockData.icon;
        _background.color = blockData.blockColor;
    }

    Image CreateBackground()
    {
        if (backgroundSprite == null)
        {
            if (_whiteSprite == null)
            {
                var tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, Color.white);
                tex.Apply();
                _whiteSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);
            }
        }

        var bgGO = new GameObject("Background");
        bgGO.transform.SetParent(transform, false);
        bgGO.transform.localScale = Vector3.one * 1.1f;
        var img = bgGO.AddComponent<Image>();
        img.sprite = backgroundSprite != null ? backgroundSprite : _whiteSprite;
        Stretch(img.rectTransform);
        return img;
    }

    Image CreateSpriteImage()
    {
        var spriteGO = new GameObject("SpriteImage");
        spriteGO.transform.SetParent(transform, false);
        var img = spriteGO.AddComponent<Image>();
        Stretch(img.rectTransform);
        img.rectTransform.offsetMin = new Vector2(10, 10);
        img.rectTransform.offsetMax = new Vector2(-10, -10);
        return img;
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
