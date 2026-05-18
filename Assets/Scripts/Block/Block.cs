using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    public BlockData data;
    public int chainGroupId = -1;

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

    public void FlyIn(Vector2 targetLocalPos, float duration, System.Action onComplete = null)
    {
        if (_layoutElement != null)
            _layoutElement.ignoreLayout = true;
        StartCoroutine(FlyInCoroutine(targetLocalPos, duration, onComplete));
    }

    System.Collections.IEnumerator FlyInCoroutine(Vector2 targetLocalPos, float duration, System.Action onComplete)
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
        onComplete?.Invoke();
    }

    public void SetChainVisual(int chainCount)
    {
        _background.color = data.blockColor;
    }

    void OnClicked()
    {
        Debug.Log($"[Block] Clicked: {data?.id} ({data?.ownerClass})");
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
        if (_whiteSprite == null)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            _whiteSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);
        }

        var bgGO = new GameObject("Background");
        bgGO.transform.SetParent(transform, false);
        bgGO.transform.localScale = Vector3.one * 1.1f;
        var img = bgGO.AddComponent<Image>();
        img.sprite = _whiteSprite;
        return img;
    }

    Image CreateSpriteImage()
    {
        var spriteGO = new GameObject("SpriteImage");
        spriteGO.transform.SetParent(transform, false);
        return spriteGO.AddComponent<Image>();
    }
}
