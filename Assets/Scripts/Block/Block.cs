using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    public BlockData data;
    public Character owner;
    public int chainGroupId = -1;

    // 풀링 객체라 GetCancellationTokenOnDestroy 대신 lease별 CTS 사용 (풀 반환 시 Cancel)
    CancellationTokenSource _animCts;
    CancellationToken AnimToken => (_animCts ??= new CancellationTokenSource()).Token;

    public void CancelAnimations()
    {
        if (_animCts == null)
            return;
        _animCts.Cancel();
        _animCts.Dispose();
        _animCts = null;
    }

    void OnDestroy() => CancelAnimations();

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

        MoveAsync(targetLocalPos + new Vector2(1000, 0), targetLocalPos, duration, onComplete, AnimToken)
            .Forget();
    }

    public void Slide(Vector2 targetLocalPos, float duration, Action onComplete = null)
    {
        if (_layoutElement != null)
            _layoutElement.ignoreLayout = true;

        MoveAsync(_rectTransform.anchoredPosition, targetLocalPos, duration, onComplete, AnimToken)
            .Forget();
    }

    async UniTaskVoid MoveAsync(
        Vector2 startPos,
        Vector2 targetLocalPos,
        float duration,
        Action onComplete,
        CancellationToken ct
    )
    {
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * (2 - t); // EaseOutQuad
            _rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetLocalPos, t);
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }

        _rectTransform.anchoredPosition = targetLocalPos;
        if (_layoutElement != null)
            _layoutElement.ignoreLayout = false;

        await UniTask.WaitForEndOfFrame(this, ct);
        onComplete?.Invoke();
    }

    public void HighlightPulse(float scale = 1.2f, float duration = 0.2f)
    {
        HighlightPulseAsync(scale, duration, AnimToken).Forget();
    }

    async UniTaskVoid HighlightPulseAsync(float scale, float duration, CancellationToken ct)
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
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
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

    // 풀 재사용 시 애니메이션 잔존 상태(스케일·위치·레이아웃 무시) 초기화
    public void PrepareForReuse()
    {
        EnsureInitialized();
        _rectTransform.localScale = Vector3.one;
        _rectTransform.anchoredPosition = Vector2.zero;
        if (_layoutElement != null)
            _layoutElement.ignoreLayout = false;
    }

    public void Init(BlockData blockData)
    {
        EnsureInitialized();
        data = blockData;
        if (blockData.icon != null)
        {
            _sprite.sprite = blockData.icon;
            _sprite.preserveAspect = true;
        }
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
        img.preserveAspect = true;
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
