using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

// Builds a placeholder game HUD at runtime using only basic shapes (colored
// Images + legacy Text). Sprites can be swapped in later by replacing
// Image.sprite / Image.color on the named children.
//
// Setup:
//   1) Create an empty GameObject in an empty scene.
//   2) Attach this script.
//   3) Press Play. (Or use the context menu "Build UI Now" while in Edit Mode.)
//   4) Optional: right-click the generated "GameHUD" Canvas -> Prefab it.
//
// Reference resolution: 1920x1080. Canvas Scaler matches by 0.5 so the HUD
// keeps its layout on wider/taller aspect ratios.
public class GameUIBuilder : MonoBehaviour
{
    [Header("Build options")]
    public bool buildOnAwake = true;
    public bool clearExistingOnBuild = true;

    // Palette — easy to tweak in inspector.
    public Color bgPanel = new Color(0.10f, 0.11f, 0.14f, 0.86f);
    public Color bgPanelLight = new Color(0.16f, 0.18f, 0.22f, 0.86f);
    public Color outlineLight = new Color(1f, 1f, 1f, 0.70f);
    public Color outlineGold = new Color(1.00f, 0.86f, 0.47f, 1f);
    public Color hpBack = new Color(0.31f, 0.16f, 0.16f, 1f);
    public Color hpFill = new Color(0.31f, 0.78f, 0.31f, 1f);
    public Color spBack = new Color(0.16f, 0.16f, 0.31f, 1f);
    public Color spFill = new Color(0.31f, 0.63f, 0.94f, 1f);
    public Color castBack = new Color(0.08f, 0.08f, 0.12f, 0.86f);
    public Color castFill = new Color(0.31f, 0.78f, 0.94f, 1f);
    public Color textGold = new Color(1.00f, 0.90f, 0.47f, 1f);

    // ---------- entry points ----------

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
            var existing = GameObject.Find("GameHUD");
            if (existing != null)
                DestroyImmediate(existing);
            var es = GameObject.Find("EventSystem");
            if (es != null)
                DestroyImmediate(es);
        }

        var canvasGO = new GameObject(
            "GameHUD",
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));

        var safeAreaGO = new GameObject("SafeArea", typeof(RectTransform));
        safeAreaGO.transform.SetParent(canvasGO.transform, false);
        safeAreaGO.AddComponent<SafeAreaFitter>();
        var root = safeAreaGO.transform;

        BuildPartyFrame(root);
        BuildStatsPanel(root);
        BuildTopRight(root);
        BuildCastBar(root);
        BuildSkillBar(root);
    }

    // ---------- sections ----------

    void BuildPartyFrame(Transform parent)
    {
        var group = CreateRT("PartyFrame", parent);
        AnchorTopLeft(group, new Vector2(30, -20), new Vector2(680, 100));

        int[] levels = { 10, 27, 35 };
        int[] hp = { 1334, 988, 2298 };
        int[] sp = { 80, 0, 0 };
        float[] hpP = { 0.85f, 0.55f, 0.92f };
        float[] spP = { 0.80f, 0f, 0f };

        for (int i = 0; i < 3; i++)
        {
            var card = CreateImage($"Card{i + 1}", group, bgPanelLight);
            AddOutline(card, outlineLight);
            var cardRT = card.rectTransform;
            cardRT.anchorMin = cardRT.anchorMax = new Vector2(0, 1);
            cardRT.pivot = new Vector2(0, 1);
            cardRT.anchoredPosition = new Vector2(i * 220f, 0);
            cardRT.sizeDelta = new Vector2(200, 90);

            // Portrait placeholder
            var port = CreateImage("Portrait", card.transform, new Color(0.35f, 0.35f, 0.43f, 1f));
            AddOutline(port, outlineLight);
            SetRect(
                port.rectTransform,
                new Vector2(0, 1),
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(6, -6),
                new Vector2(72, 72)
            );

            CreateText(
                "PortLabel",
                port.transform,
                "PORT",
                14,
                Color.white,
                TextAnchor.MiddleCenter,
                FillParent
            );

            // Level
            CreateText(
                "Level",
                card.transform,
                $"LV{levels[i]}",
                22,
                textGold,
                TextAnchor.UpperLeft,
                rt =>
                    SetRect(
                        rt,
                        new Vector2(0, 1),
                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(86, -4),
                        new Vector2(110, 26)
                    )
            );

            // HP bar
            BuildBar(
                card.transform,
                "HPBar",
                new Vector2(86, -34),
                new Vector2(106, 16),
                hpBack,
                hpFill,
                hpP[i],
                $"HP {hp[i]}",
                12
            );

            // SP bar
            BuildBar(
                card.transform,
                "SPBar",
                new Vector2(86, -56),
                new Vector2(106, 16),
                spBack,
                spFill,
                spP[i],
                $"SP {sp[i]}",
                12
            );
        }
    }

    void BuildStatsPanel(Transform parent)
    {
        var bg = CreateImage("StatsPanel", parent, new Color(0, 0, 0, 0.63f));
        AddOutline(bg, outlineLight);
        AnchorTopLeft(bg.rectTransform, new Vector2(30, -130), new Vector2(360, 110));

        CreateText(
            "H1",
            bg.transform,
            "이름",
            18,
            textGold,
            TextAnchor.UpperLeft,
            rt =>
                SetRect(
                    rt,
                    new Vector2(0, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(20, -8),
                    new Vector2(150, 24)
                )
        );
        CreateText(
            "H2",
            bg.transform,
            "피해량",
            18,
            textGold,
            TextAnchor.UpperLeft,
            rt =>
                SetRect(
                    rt,
                    new Vector2(0, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(170, -8),
                    new Vector2(110, 24)
                )
        );
        CreateText(
            "H3",
            bg.transform,
            "치유량",
            18,
            textGold,
            TextAnchor.UpperLeft,
            rt =>
                SetRect(
                    rt,
                    new Vector2(0, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(280, -8),
                    new Vector2(110, 24)
                )
        );

        string[,] rows =
        {
            { "학살의 크림힐트", "10,062", "0" },
            { "간호학생", "417", "1,331" },
            { "한조", "2,148", "0" },
        };
        for (int i = 0; i < 3; i++)
        {
            float y = -32 - i * 24;
            CreateText(
                $"R{i}N",
                bg.transform,
                rows[i, 0],
                16,
                Color.white,
                TextAnchor.UpperLeft,
                rt =>
                    SetRect(
                        rt,
                        new Vector2(0, 1),
                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(12, y),
                        new Vector2(160, 22)
                    )
            );
            CreateText(
                $"R{i}D",
                bg.transform,
                rows[i, 1],
                16,
                Color.white,
                TextAnchor.UpperLeft,
                rt =>
                    SetRect(
                        rt,
                        new Vector2(0, 1),
                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(170, y),
                        new Vector2(110, 22)
                    )
            );
            CreateText(
                $"R{i}H",
                bg.transform,
                rows[i, 2],
                16,
                Color.white,
                TextAnchor.UpperLeft,
                rt =>
                    SetRect(
                        rt,
                        new Vector2(0, 1),
                        new Vector2(0, 1),
                        new Vector2(0, 0),
                        new Vector2(280, y),
                        new Vector2(110, 22)
                    )
            );
        }
    }

    void BuildTopRight(Transform parent)
    {
        var group = CreateRT("TopRight", parent);
        AnchorTopRight(group, new Vector2(-30, -20), new Vector2(460, 110));

        // Chest
        var chest = CreateImage("Chest", group, new Color(0.55f, 0.36f, 0.20f, 1f));
        AddOutline(chest, outlineLight);
        SetRect(
            chest.rectTransform,
            new Vector2(0, 1),
            new Vector2(0, 1),
            new Vector2(0, 0),
            new Vector2(0, 0),
            new Vector2(50, 50)
        );
        CreateText(
            "ChestLbl",
            chest.transform,
            "박스",
            14,
            Color.white,
            TextAnchor.MiddleCenter,
            FillParent
        );
        CreateText(
            "ChestCount",
            group,
            "2",
            28,
            Color.white,
            TextAnchor.MiddleLeft,
            rt =>
                SetRect(
                    rt,
                    new Vector2(0, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(58, -16),
                    new Vector2(60, 40)
                )
        );

        // Gold
        var gold = CreateImage("Gold", group, new Color(0.86f, 0.71f, 0.24f, 1f));
        AddOutline(gold, outlineLight);
        SetRect(
            gold.rectTransform,
            new Vector2(0, 1),
            new Vector2(0, 1),
            new Vector2(0, 0),
            new Vector2(160, 0),
            new Vector2(50, 50)
        );
        CreateText(
            "G",
            gold.transform,
            "G",
            22,
            new Color(0.35f, 0.24f, 0, 1),
            TextAnchor.MiddleCenter,
            FillParent
        );
        CreateText(
            "GoldCount",
            group,
            "54",
            28,
            Color.white,
            TextAnchor.MiddleLeft,
            rt =>
                SetRect(
                    rt,
                    new Vector2(0, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(218, -16),
                    new Vector2(80, 40)
                )
        );

        // Pause button
        var pauseBtn = CreateImage("PauseBtn", group, new Color(0.31f, 0.55f, 0.86f, 1f));
        AddOutline(pauseBtn, outlineLight);
        SetRect(
            pauseBtn.rectTransform,
            new Vector2(1, 1),
            new Vector2(1, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(60, 60)
        );
        pauseBtn.gameObject.AddComponent<Button>();
        // pause icon (two white bars)
        var barL = CreateImage("BarL", pauseBtn.transform, Color.white);
        SetRect(
            barL.rectTransform,
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(-9, 0),
            new Vector2(8, 30)
        );
        var barR = CreateImage("BarR", pauseBtn.transform, Color.white);
        SetRect(
            barR.rectTransform,
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(9, 0),
            new Vector2(8, 30)
        );

        // Stage label (left of WAVE)
        CreateText(
            "Stage",
            group,
            "사막-16",
            22,
            Color.white,
            TextAnchor.MiddleLeft,
            rt =>
                SetRect(
                    rt,
                    new Vector2(0, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(0, -76),
                    new Vector2(180, 30)
                )
        );
        // Wave label (right side)
        CreateText(
            "Wave",
            group,
            "WAVE 4/6",
            22,
            Color.white,
            TextAnchor.MiddleRight,
            rt =>
                SetRect(
                    rt,
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 1),
                    new Vector2(-70, -76),
                    new Vector2(180, 30)
                )
        );
    }

    void BuildCastBar(Transform parent)
    {
        var group = CreateRT("CastBar", parent);
        // anchored bottom-center, sits above the skill bar
        AnchorBottomCenter(group, new Vector2(0, 240), new Vector2(760, 60));

        // Bar background
        var back = CreateImage("Back", group, castBack);
        AddOutline(back, outlineGold);
        SetRect(
            back.rectTransform,
            new Vector2(0, 0.5f),
            new Vector2(1, 0.5f),
            new Vector2(0, 0.5f),
            new Vector2(30, -18),
            new Vector2(0, 36)
        );
        // stretch horizontally except left 30px reserved for portrait
        var brt = back.rectTransform;
        brt.anchorMin = new Vector2(0, 0.5f);
        brt.anchorMax = new Vector2(1, 0.5f);
        brt.offsetMin = new Vector2(30, -18);
        brt.offsetMax = new Vector2(0, 18);

        // Fill
        var fill = CreateImage("Fill", back.transform, castFill);
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillAmount = 0.55f;
        var frt = fill.rectTransform;
        frt.anchorMin = new Vector2(0, 0);
        frt.anchorMax = new Vector2(1, 1);
        frt.offsetMin = new Vector2(2, 2);
        frt.offsetMax = new Vector2(-2, -2);

        // Portrait circle placeholder (square here, but ringed)
        var port = CreateImage("Portrait", group, new Color(0.47f, 0.63f, 0.35f, 1f));
        AddOutline(port, outlineGold);
        SetRect(
            port.rectTransform,
            new Vector2(0, 0.5f),
            new Vector2(0, 0.5f),
            new Vector2(0, 0.5f),
            new Vector2(0, 0),
            new Vector2(60, 60)
        );
        CreateText(
            "PortLbl",
            port.transform,
            "FACE",
            12,
            Color.white,
            TextAnchor.MiddleCenter,
            FillParent
        );
    }

    void BuildSkillBar(Transform parent)
    {
        const int slotCount = 8;
        const float slotSize = 90f;
        const float gap = 12f;
        const float pad = 18f;

        float totalW = slotCount * slotSize + (slotCount - 1) * gap;
        float frameW = totalW + pad * 2f;
        float frameH = slotSize + pad * 2f;

        var frame = CreateImage("SkillBarFrame", parent, new Color(0.10f, 0.08f, 0.06f, 0.86f));
        AddOutline(frame, outlineGold, 4f);
        AnchorBottomCenter(
            frame.rectTransform,
            new Vector2(0, 30 + frameH * 0.5f),
            new Vector2(frameW, frameH)
        );

        Color[] slotColors =
        {
            new Color(0.70f, 0.20f, 0.20f),
            new Color(0.70f, 0.20f, 0.20f),
            new Color(0.24f, 0.63f, 0.31f),
            new Color(0.24f, 0.63f, 0.31f),
            new Color(0.70f, 0.20f, 0.20f),
            new Color(0.35f, 0.55f, 0.86f),
            new Color(0.55f, 0.35f, 0.71f),
            new Color(0.35f, 0.55f, 0.86f),
        };

        float startX = -totalW * 0.5f + slotSize * 0.5f;
        for (int i = 0; i < slotCount; i++)
        {
            var slot = CreateImage($"Skill{i + 1}", frame.transform, slotColors[i]);
            AddOutline(slot, outlineGold, 3f);
            var rt = slot.rectTransform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(slotSize, slotSize);
            rt.anchoredPosition = new Vector2(startX + i * (slotSize + gap), 0);
            slot.gameObject.AddComponent<Button>();
        }
    }

    // ---------- helpers ----------

    static Sprite _whiteSprite;
    static Sprite WhiteSprite
    {
        get
        {
            if (_whiteSprite == null)
            {
                var tex = Texture2D.whiteTexture;
                _whiteSprite = Sprite.Create(
                    tex,
                    new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f)
                );
            }
            return _whiteSprite;
        }
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
        img.sprite = WhiteSprite; // so future sprite swap is trivial
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

    void BuildBar(
        Transform parent,
        string name,
        Vector2 anchoredTopLeft,
        Vector2 size,
        Color back,
        Color fill,
        float pct,
        string label,
        int labelSize
    )
    {
        var bar = CreateImage(name, parent, back);
        AddOutline(bar, outlineLight, 1f);
        var rt = bar.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = anchoredTopLeft;
        rt.sizeDelta = size;

        var f = CreateImage("Fill", bar.transform, fill);
        f.type = Image.Type.Filled;
        f.fillMethod = Image.FillMethod.Horizontal;
        f.fillAmount = Mathf.Clamp01(pct);
        var frt = f.rectTransform;
        frt.anchorMin = new Vector2(0, 0);
        frt.anchorMax = new Vector2(1, 1);
        frt.offsetMin = new Vector2(1, 1);
        frt.offsetMax = new Vector2(-1, -1);

        CreateText(
            "Label",
            bar.transform,
            label,
            labelSize,
            Color.white,
            TextAnchor.MiddleLeft,
            r =>
            {
                r.anchorMin = new Vector2(0, 0);
                r.anchorMax = new Vector2(1, 1);
                r.offsetMin = new Vector2(4, 0);
                r.offsetMax = new Vector2(-4, 0);
            }
        );
    }

    void AddOutline(Graphic g, Color color, float thickness = 2f)
    {
        var ol = g.gameObject.AddComponent<Outline>();
        ol.effectColor = color;
        ol.effectDistance = new Vector2(thickness, -thickness);
        ol.useGraphicAlpha = false;
    }

    // ---------- RectTransform layout helpers ----------

    void FillParent(RectTransform rt)
    {
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    void SetRect(
        RectTransform rt,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Vector2 anchoredPos,
        Vector2 size
    )
    {
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
    }

    void AnchorTopLeft(RectTransform rt, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    void AnchorTopRight(RectTransform rt, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1, 1);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    void AnchorBottomCenter(RectTransform rt, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }
}
