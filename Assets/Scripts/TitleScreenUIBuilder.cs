using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

// Title screen — "CHAIN KNIGHTS" / "체인 나이츠".
// Reference resolution 1920x1080, ScaleWithScreenSize (Match 0.5).
// Anywhere-tap starts: the whole screen is the start button.
//
// Setup: empty scene -> empty GameObject -> add this component -> Play.
public class TitleScreenUIBuilder : MonoBehaviour
{
    [Header("Build options")]
    public bool buildOnAwake = true;
    public bool clearExistingOnBuild = true;

    [Header("Text")]
    public string titleText = "CHAIN  KNIGHTS";
    public string subtitleText = "체인 나이츠";
    public string version = "v 0.1.0";
    public string copyright = "Copyright © Your Studio. All Rights Reserved";

    [Header("Palette (matches in-game / character-select UI)")]
    public Color bgColor = new Color(0.05f, 0.03f, 0.02f, 1f);
    public Color haloColor = new Color(0.94f, 0.69f, 0.27f, 1f);
    public Color goldBright = new Color(1.00f, 0.86f, 0.51f, 1f);
    public Color cream = new Color(1.00f, 0.96f, 0.82f, 1f);
    public Color gold = new Color(0.89f, 0.70f, 0.31f, 1f);
    public Color dimText = new Color(0.78f, 0.71f, 0.55f, 1f);

    // runtime refs
    Text _tapText;
    Coroutine _blink;

    void Awake()
    {
        if (buildOnAwake)
            Build();
    }

    [ContextMenu("Build UI Now")]
    public void Build()
    {
        if (clearExistingOnBuild)
        {
            var existing = GameObject.Find("TitleScreenUI");
            if (existing != null)
                DestroyImmediate(existing);
            var es = GameObject.Find("EventSystem");
            if (es != null)
                DestroyImmediate(es);
        }

        var canvasGO = new GameObject(
            "TitleScreenUI",
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));

        var safeAreaGO = new GameObject("SafeArea", typeof(RectTransform));
        safeAreaGO.transform.SetParent(canvasGO.transform, false);
        safeAreaGO.AddComponent<SafeAreaFitter>();
        var root = safeAreaGO.transform;

        // ---- Background ----
        var bg = CreateImage("Background", root, bgColor);
        FillParent(bg.rectTransform);
        bg.raycastTarget = true; // whole screen is the tap target

        // ---- Radial amber halo ----
        var halo = CreateImage("Halo", root, haloColor);
        halo.sprite = MakeRadialSprite(256, 1.0f);
        halo.color = new Color(haloColor.r, haloColor.g, haloColor.b, 0.85f);
        halo.raycastTarget = false;
        var hrt = halo.rectTransform;
        hrt.anchorMin = hrt.anchorMax = hrt.pivot = new Vector2(0.5f, 0.5f);
        hrt.anchoredPosition = new Vector2(0, 60); // slightly above center, matches preview
        hrt.sizeDelta = new Vector2(2200, 1300);

        // ---- Title ----
        var title = CreateText(
            "Title",
            root,
            titleText,
            150,
            goldBright,
            TextAnchor.MiddleCenter,
            rt =>
            {
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(0, 60);
                rt.sizeDelta = new Vector2(1600, 220);
            }
        );
        title.fontStyle = FontStyle.Bold;
        // drop shadow
        var sh = title.gameObject.AddComponent<Shadow>();
        sh.effectColor = new Color(0.12f, 0.07f, 0.03f, 0.85f);
        sh.effectDistance = new Vector2(4, -6);
        // warm outer glow via Outline
        var ol = title.gameObject.AddComponent<Outline>();
        ol.effectColor = new Color(1f, 0.78f, 0.35f, 0.6f);
        ol.effectDistance = new Vector2(3, -3);
        ol.useGraphicAlpha = false;

        // ---- Underline group ----
        var ulGroup = CreateRT("Underline", root);
        ulGroup.anchorMin = ulGroup.anchorMax = ulGroup.pivot = new Vector2(0.5f, 0.5f);
        ulGroup.anchoredPosition = new Vector2(0, -50);
        ulGroup.sizeDelta = new Vector2(960, 20);

        var line1 = CreateImage("Line1", ulGroup, cream);
        var l1rt = line1.rectTransform;
        l1rt.anchorMin = new Vector2(0, 0.5f);
        l1rt.anchorMax = new Vector2(1, 0.5f);
        l1rt.pivot = new Vector2(0.5f, 0.5f);
        l1rt.anchoredPosition = new Vector2(0, 4);
        l1rt.sizeDelta = new Vector2(0, 4);

        var line2 = CreateImage("Line2", ulGroup, gold);
        var l2rt = line2.rectTransform;
        l2rt.anchorMin = new Vector2(0, 0.5f);
        l2rt.anchorMax = new Vector2(1, 0.5f);
        l2rt.pivot = new Vector2(0.5f, 0.5f);
        l2rt.anchoredPosition = new Vector2(0, -4);
        l2rt.sizeDelta = new Vector2(0, 2);

        AddDiamond(ulGroup, new Vector2(-475, 0), 20, goldBright);
        AddDiamond(ulGroup, new Vector2(475, 0), 20, goldBright);

        // ---- Subtitle (right under the underline, right side) ----
        CreateText(
            "Subtitle",
            root,
            subtitleText,
            38,
            cream,
            TextAnchor.MiddleRight,
            rt =>
            {
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(360, -100);
                rt.sizeDelta = new Vector2(420, 50);
            }
        );

        // ---- TAP TO START ----
        _tapText = CreateText(
            "TapToStart",
            root,
            "TAP TO START",
            56,
            goldBright,
            TextAnchor.MiddleCenter,
            rt =>
            {
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(0, -300);
                rt.sizeDelta = new Vector2(800, 80);
            }
        );
        _tapText.fontStyle = FontStyle.Bold;
        var tapShadow = _tapText.gameObject.AddComponent<Shadow>();
        tapShadow.effectColor = new Color(0.94f, 0.67f, 0.24f, 0.55f);
        tapShadow.effectDistance = new Vector2(0, -4);

        // ---- Version (top-right) ----
        CreateText(
            "Version",
            root,
            version,
            18,
            dimText,
            TextAnchor.UpperRight,
            rt =>
            {
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1, 1);
                rt.anchoredPosition = new Vector2(-30, -30);
                rt.sizeDelta = new Vector2(200, 24);
            }
        );

        // ---- Copyright (bottom-center) ----
        CreateText(
            "Copyright",
            root,
            copyright,
            20,
            dimText,
            TextAnchor.LowerCenter,
            rt =>
            {
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0);
                rt.anchoredPosition = new Vector2(0, 30);
                rt.sizeDelta = new Vector2(1400, 28);
            }
        );

        // ---- Tap-anywhere button ----
        var btn = bg.gameObject.AddComponent<Button>();
        btn.transition = Selectable.Transition.None;
        btn.onClick.AddListener(OnTapToStart);

        // start blink
        if (_blink != null)
            StopCoroutine(_blink);
        _blink = StartCoroutine(BlinkTap());
    }

    IEnumerator BlinkTap()
    {
        float t = 0f;
        while (_tapText != null)
        {
            t += Time.unscaledDeltaTime * 2.4f;
            float a = 0.45f + 0.55f * (Mathf.Sin(t) * 0.5f + 0.5f);
            var c = _tapText.color;
            c.a = a;
            _tapText.color = c;
            yield return null;
        }
    }

    void OnTapToStart()
    {
        Debug.Log("[TitleScreen] Tap to start");
        // Hook your scene transition here, e.g.:
        // UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterSelect");
    }

    // ============ Primitives ============

    static Sprite _white;
    static Sprite WhiteSprite
    {
        get
        {
            if (_white == null)
            {
                var t = Texture2D.whiteTexture;
                _white = Sprite.Create(
                    t,
                    new Rect(0, 0, t.width, t.height),
                    new Vector2(0.5f, 0.5f)
                );
            }
            return _white;
        }
    }

    // Procedural soft radial gradient (white center -> transparent edge).
    Sprite MakeRadialSprite(int size, float falloff)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;
        var pix = new Color[size * size];
        float cx = (size - 1) * 0.5f,
            cy = (size - 1) * 0.5f;
        float maxR = cx;
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float dx = (x - cx) / maxR,
                dy = (y - cy) / maxR;
            float r = Mathf.Sqrt(dx * dx + dy * dy);
            float a = Mathf.Clamp01(1f - r);
            a = Mathf.Pow(a, 2.4f * falloff);
            pix[y * size + x] = new Color(1f, 1f, 1f, a);
        }
        tex.SetPixels(pix);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    RectTransform CreateRT(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go.GetComponent<RectTransform>();
    }

    Image CreateImage(string name, Transform parent, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        img.sprite = WhiteSprite;
        img.color = color;
        img.raycastTarget = false;
        return img;
    }

    Text CreateText(
        string name,
        Transform parent,
        string content,
        int size,
        Color color,
        TextAnchor align,
        System.Action<RectTransform> layout
    )
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        go.transform.SetParent(parent, false);
        var t = go.GetComponent<Text>();
        t.text = content;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = size;
        t.color = color;
        t.alignment = align;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
        layout?.Invoke(t.rectTransform);
        return t;
    }

    void AddDiamond(Transform parent, Vector2 pos, float size, Color color)
    {
        var img = CreateImage("Diamond", parent, color);
        var rt = img.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(size, size);
        rt.localRotation = Quaternion.Euler(0, 0, 45f);
    }

    void FillParent(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}
