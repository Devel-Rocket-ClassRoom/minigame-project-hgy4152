using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

// Character-select modal — placeholder UI with basic shapes only.
// Reference resolution 1920x1080. Modal is centered, so resolution / aspect
// ratio changes keep the panel in the middle.
//
// Setup:
//   1) Empty scene -> empty GameObject -> attach this script -> Play.
//   2) Or right-click the component header -> "Build UI Now" in Edit Mode.
//
// Wired interactions:
//   - Grid slot click -> select that character (gold highlight + info panel updates)
//   - "출전중" overlay slot click -> remove that character from party
//   - Party slot X button -> remove from party
//   - Cancel / Start Battle -> Debug.Log placeholders
public class CharacterSelectUIBuilder : MonoBehaviour
{
    [Header("Build options")]
    public bool buildOnAwake = true;
    public bool clearExistingOnBuild = true;

    // -------- palette --------
    static readonly Color BG_DIM = new Color(0f, 0f, 0f, 0.55f);
    static readonly Color BG_DARK = new Color(0.15f, 0.11f, 0.08f, 1f);
    static readonly Color PANEL_BRN = new Color(0.35f, 0.24f, 0.15f, 1f);
    static readonly Color TITLE_BAR = new Color(0.24f, 0.16f, 0.11f, 1f);
    static readonly Color GOLD = new Color(0.89f, 0.70f, 0.31f, 1f);
    static readonly Color GOLD_DARK = new Color(0.63f, 0.47f, 0.20f, 1f);
    static readonly Color SLOT_BG = new Color(0.24f, 0.16f, 0.11f, 1f);
    static readonly Color SLOT_FILL = new Color(0.37f, 0.25f, 0.16f, 1f);
    static readonly Color SLOT_EMPTY = new Color(0.18f, 0.14f, 0.11f, 1f);
    static readonly Color HP_BACK = new Color(0.31f, 0.12f, 0.12f, 1f);
    static readonly Color HP_FILL = new Color(0.35f, 0.78f, 0.35f, 1f);
    static readonly Color SP_BACK = new Color(0.14f, 0.14f, 0.29f, 1f);
    static readonly Color SP_FILL = new Color(0.35f, 0.67f, 0.94f, 1f);
    static readonly Color TXT_W = new Color(1.00f, 0.97f, 0.90f, 1f);
    static readonly Color TXT_GOLD = new Color(1.00f, 0.86f, 0.51f, 1f);
    static readonly Color BTN_GREEN = new Color(0.59f, 0.82f, 0.35f, 1f);
    static readonly Color BTN_GRAY = new Color(0.51f, 0.41f, 0.33f, 1f);
    static readonly Color RED = new Color(0.63f, 0.20f, 0.20f, 1f);

    // -------- data --------
    [Serializable]
    public class CharData
    {
        public string name;
        public string className;
        public int level;
        public int rarity = 1; // 1..3 stars
        public int atk,
            def,
            spd,
            crit;
        public int hp,
            hpMax,
            sp,
            spMax;
    }

    public List<CharData> roster = new List<CharData>();
    public int gridCols = 8;
    public int gridRows = 2;

    // selection state
    int _selectedIndex = 2;
    readonly List<int> _party = new List<int> { 0, 1, 5 };

    // runtime references for updates
    Image[] _slotBg;
    Image[] _slotOutline;
    GameObject[] _slotPartyOverlay;
    RectTransform _partyRow;
    Image _infoPortrait;
    Text _infoName,
        _infoClass,
        _infoAtk,
        _infoDef,
        _infoSpd,
        _infoCrit,
        _infoHp,
        _infoSp;
    Image _infoHpFill,
        _infoSpFill;
    Text _ownedCountLbl;

    // -------- entry --------
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
            var existing = GameObject.Find("CharacterSelectUI");
            if (existing != null)
                DestroyImmediate(existing);
            var es = GameObject.Find("EventSystem");
            if (es != null)
                DestroyImmediate(es);
        }
        if (roster.Count == 0)
            FillDefaultRoster();

        var canvasGO = new GameObject(
            "CharacterSelectUI",
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
        var dim = CreateImage("Dim", root, BG_DIM);
        FillParent(dim.rectTransform);
        dim.raycastTarget = true; // block click-through

        var modal = CreateImage("Modal", root, BG_DARK);
        AddOutline(modal, GOLD, 4f);
        var mrt = modal.rectTransform;
        mrt.anchorMin = mrt.anchorMax = mrt.pivot = new Vector2(0.5f, 0.5f);
        mrt.sizeDelta = new Vector2(1640, 940);
        mrt.anchoredPosition = Vector2.zero;

        var inner = CreateImage("Inner", modal.transform, PANEL_BRN);
        AddOutline(inner, GOLD_DARK, 2f);
        SetRect(
            inner.rectTransform,
            new Vector2(0, 0),
            new Vector2(1, 1),
            new Vector2(0.5f, 0.5f),
            Vector2.zero,
            Vector2.zero,
            new Vector2(14, 14),
            new Vector2(-14, -14)
        );

        BuildTitleBar(modal.transform);
        BuildPartyRow(modal.transform);
        BuildDivider(modal.transform);
        BuildOwnedGrid(modal.transform);
        BuildInfoPanel(modal.transform);
        BuildBottomButtons(modal.transform);

        RefreshSelectionVisuals();
        RefreshInfoPanel();
        RefreshPartyRow();
        if (_ownedCountLbl != null)
            _ownedCountLbl.text = $"{roster.Count} / {gridCols * gridRows}";
    }

    void FillDefaultRoster()
    {
        roster.Clear();
        string[] names =
        {
            "학살의 크림힐트",
            "간호학생",
            "한조",
            "유주검사",
            "마법학도",
            "성기사",
            "도적",
            "사냥꾼",
            "광전사",
            "주술사",
            "성녀",
            "기계병",
        };
        for (int i = 0; i < names.Length; i++)
        {
            roster.Add(
                new CharData
                {
                    name = names[i],
                    className =
                        (i % 3 == 0) ? "근접 · 전사"
                        : (i % 3 == 1) ? "원거리 · 사냥꾼"
                        : "지원 · 치유",
                    level = 10 + i * 3,
                    rarity = (i % 3) + 1,
                    atk = 100 + i * 18,
                    def = 50 + i * 9,
                    spd = 60 + i * 4,
                    crit = 5 + i * 2,
                    hp = 1500 + i * 250,
                    hpMax = 1600 + i * 250,
                    sp = 80 + i * 12,
                    spMax = 250 + i * 8,
                }
            );
        }
    }

    // ============ Sections ============

    void BuildTitleBar(Transform modal)
    {
        var bar = CreateImage("TitleBar", modal, TITLE_BAR);
        AddOutline(bar, GOLD, 4f);
        var rt = bar.rectTransform;
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0, 10);
        rt.sizeDelta = new Vector2(900, 90);

        CreateText(
            "Title",
            bar.transform,
            "캐릭터 선택",
            40,
            TXT_W,
            TextAnchor.MiddleCenter,
            FillParent
        );
        AddDiamond(bar.transform, new Vector2(-280, 0));
        AddDiamond(bar.transform, new Vector2(+280, 0));
    }

    void AddDiamond(Transform parent, Vector2 offsetFromCenter)
    {
        var img = CreateImage("Diamond", parent, GOLD);
        var rt = img.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = offsetFromCenter;
        rt.sizeDelta = new Vector2(18, 18);
        rt.localRotation = Quaternion.Euler(0, 0, 45f);
    }

    void BuildPartyRow(Transform modal)
    {
        CreateText(
            "PartyLbl",
            modal,
            "선택된 파티  (3 / 3)",
            22,
            TXT_GOLD,
            TextAnchor.UpperLeft,
            rt => AnchorTopLeftInside(rt, new Vector2(50, -95), new Vector2(360, 28))
        );

        _partyRow = CreateRT("PartyRow", modal);
        AnchorTopCenterInside(_partyRow, new Vector2(0, -130), new Vector2(3 * 230 + 2 * 24, 250));
    }

    void RefreshPartyRow()
    {
        for (int i = _partyRow.childCount - 1; i >= 0; i--)
            DestroyImmediate(_partyRow.GetChild(i).gameObject);

        const float slotW = 230,
            slotH = 250,
            gap = 24;
        float total = 3 * slotW + 2 * gap;
        for (int i = 0; i < 3; i++)
        {
            int partyIdx = i;
            bool filled = i < _party.Count;
            var card = CreateImage($"PartySlot{i + 1}", _partyRow, filled ? SLOT_BG : SLOT_EMPTY);
            AddOutline(card, GOLD, 3f);
            var rt = card.rectTransform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0, 1);
            rt.sizeDelta = new Vector2(slotW, slotH);
            rt.anchoredPosition = new Vector2(i * (slotW + gap), 0);

            if (!filled)
            {
                CreateText(
                    "Empty",
                    card.transform,
                    "비어있음",
                    18,
                    TXT_W,
                    TextAnchor.MiddleCenter,
                    FillParent
                );
                continue;
            }

            var data = roster[_party[i]];

            var port = CreateImage("Portrait", card.transform, SLOT_FILL);
            AddOutline(port, GOLD_DARK, 2f);
            SetRect(
                port.rectTransform,
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(0.5f, 1f),
                new Vector2(0, -12),
                new Vector2(0, 138),
                new Vector2(12, 0),
                new Vector2(-12, 0)
            );
            CreateText(
                "PortLbl",
                port.transform,
                "PORT",
                18,
                new Color(0.82f, 0.78f, 0.70f),
                TextAnchor.MiddleCenter,
                FillParent
            );

            CreateText(
                "Name",
                card.transform,
                data.name,
                18,
                TXT_W,
                TextAnchor.MiddleCenter,
                rt2 =>
                    SetRect(
                        rt2,
                        new Vector2(0, 1),
                        new Vector2(1, 1),
                        new Vector2(0.5f, 1f),
                        new Vector2(0, -162),
                        new Vector2(0, 24),
                        new Vector2(8, 0),
                        new Vector2(-8, 0)
                    )
            );

            var lvChip = CreateImage("LvChip", card.transform, new Color(0.16f, 0.12f, 0.08f, 1f));
            AddOutline(lvChip, GOLD, 1f);
            SetRect(
                lvChip.rectTransform,
                new Vector2(0, 1),
                new Vector2(0, 1),
                new Vector2(0, 1),
                new Vector2(12, -190),
                new Vector2(78, 26)
            );
            CreateText(
                "Lv",
                lvChip.transform,
                $"Lv {data.level}",
                14,
                TXT_GOLD,
                TextAnchor.MiddleCenter,
                FillParent
            );

            var hpBack = CreateImage("HPBack", card.transform, HP_BACK);
            AddOutline(hpBack, GOLD_DARK, 1f);
            SetRect(
                hpBack.rectTransform,
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(0.5f, 1f),
                new Vector2(0, -202),
                new Vector2(0, 16),
                new Vector2(100, 0),
                new Vector2(-12, 0)
            );
            var hpFill = CreateImage("HPFill", hpBack.transform, HP_FILL);
            hpFill.type = Image.Type.Filled;
            hpFill.fillMethod = Image.FillMethod.Horizontal;
            hpFill.fillAmount = data.hpMax > 0 ? (float)data.hp / data.hpMax : 0f;
            FillParent(hpFill.rectTransform, 1);

            // X remove button
            var xBtn = CreateImage("X", card.transform, RED);
            AddOutline(xBtn, GOLD, 1f);
            SetRect(
                xBtn.rectTransform,
                new Vector2(1, 1),
                new Vector2(1, 1),
                new Vector2(1, 1),
                new Vector2(-8, -8),
                new Vector2(24, 24)
            );
            CreateText("XLbl", xBtn.transform, "X", 16, TXT_W, TextAnchor.MiddleCenter, FillParent);
            var btn = xBtn.gameObject.AddComponent<Button>();
            int captured = partyIdx;
            btn.onClick.AddListener(() => RemoveFromParty(captured));
        }
    }

    void BuildDivider(Transform modal)
    {
        var line = CreateImage("Divider", modal, GOLD_DARK);
        AnchorTopCenterInside(line.rectTransform, new Vector2(0, -410), new Vector2(1560, 2));
    }

    void BuildOwnedGrid(Transform modal)
    {
        CreateText(
            "OwnedLbl",
            modal,
            "보유 캐릭터",
            22,
            TXT_GOLD,
            TextAnchor.UpperLeft,
            rt => AnchorTopLeftInside(rt, new Vector2(50, -440), new Vector2(200, 28))
        );

        _ownedCountLbl = CreateText(
            "OwnedCount",
            modal,
            "",
            18,
            TXT_W,
            TextAnchor.UpperRight,
            rt => AnchorTopRightInside(rt, new Vector2(-50, -440), new Vector2(200, 28))
        );

        const float cell = 130,
            gap = 16;
        int total = gridCols * gridRows;
        float gw = gridCols * cell + (gridCols - 1) * gap;
        float gh = gridRows * cell + (gridRows - 1) * gap;

        var tray = CreateImage("GridTray", modal, new Color(0.20f, 0.14f, 0.09f, 1f));
        AddOutline(tray, GOLD_DARK, 2f);
        AnchorTopCenterInside(
            tray.rectTransform,
            new Vector2(0, -490),
            new Vector2(gw + 40, gh + 40)
        );

        var grid = CreateRT("Grid", tray.transform);
        SetRect(
            grid,
            new Vector2(0.5f, 1f),
            new Vector2(0.5f, 1f),
            new Vector2(0.5f, 1f),
            new Vector2(0, -20),
            new Vector2(gw, gh)
        );

        _slotBg = new Image[total];
        _slotOutline = new Image[total];
        _slotPartyOverlay = new GameObject[total];

        for (int r = 0; r < gridRows; r++)
        for (int c = 0; c < gridCols; c++)
        {
            int idx = r * gridCols + c;
            float x = c * (cell + gap);
            float y = -r * (cell + gap);
            bool empty = idx >= roster.Count;

            var slot = CreateImage($"Slot_{idx}", grid, empty ? SLOT_EMPTY : SLOT_FILL);
            var orl = slot.gameObject.AddComponent<Outline>();
            orl.effectColor = GOLD;
            orl.effectDistance = new Vector2(2, -2);
            orl.useGraphicAlpha = false;
            _slotOutline[idx] = slot;
            _slotBg[idx] = slot;

            var rt = slot.rectTransform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0, 1);
            rt.sizeDelta = new Vector2(cell, cell);
            rt.anchoredPosition = new Vector2(x, y);

            if (empty)
            {
                CreateText(
                    "Plus",
                    slot.transform,
                    "+",
                    36,
                    new Color(0.47f, 0.39f, 0.31f),
                    TextAnchor.MiddleCenter,
                    FillParent
                );
                continue;
            }

            var port = CreateImage("Portrait", slot.transform, new Color(0.29f, 0.22f, 0.16f, 1f));
            AddOutline(port, GOLD_DARK, 1f);
            SetRect(
                port.rectTransform,
                new Vector2(0, 0),
                new Vector2(1, 1),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                Vector2.zero,
                new Vector2(10, 32),
                new Vector2(-10, -10)
            );
            CreateText(
                "PortLbl",
                port.transform,
                "PORT",
                14,
                new Color(0.82f, 0.78f, 0.70f),
                TextAnchor.MiddleCenter,
                FillParent
            );

            var lvChip = CreateImage("LvChip", slot.transform, new Color(0.12f, 0.09f, 0.06f, 1f));
            AddOutline(lvChip, GOLD, 1f);
            SetRect(
                lvChip.rectTransform,
                new Vector2(0, 0),
                new Vector2(0, 0),
                new Vector2(0, 0),
                new Vector2(6, 6),
                new Vector2(42, 20)
            );
            CreateText(
                "Lv",
                lvChip.transform,
                $"L{roster[idx].level}",
                12,
                TXT_GOLD,
                TextAnchor.MiddleCenter,
                FillParent
            );

            // rarity stars
            for (int s = 0; s < Mathf.Clamp(roster[idx].rarity, 0, 3); s++)
            {
                var star = CreateImage($"Star{s}", slot.transform, new Color(1f, 0.86f, 0.31f));
                var srt = star.rectTransform;
                srt.anchorMin = srt.anchorMax = srt.pivot = new Vector2(0, 0);
                srt.sizeDelta = new Vector2(10, 10);
                srt.anchoredPosition = new Vector2(54 + s * 14, 10);
                srt.localRotation = Quaternion.Euler(0, 0, 45);
            }

            // in-party overlay
            var overlay = CreateImage("PartyOverlay", slot.transform, new Color(0, 0, 0, 0.45f));
            FillParent(overlay.rectTransform);
            CreateText(
                "InPartyLbl",
                overlay.transform,
                "출전중",
                16,
                TXT_GOLD,
                TextAnchor.MiddleCenter,
                FillParent
            );
            _slotPartyOverlay[idx] = overlay.gameObject;

            // click handler
            var btn = slot.gameObject.AddComponent<Button>();
            int captured = idx;
            btn.onClick.AddListener(() => OnGridSlotClicked(captured));
        }
    }

    void BuildInfoPanel(Transform modal)
    {
        var panel = CreateImage("InfoPanel", modal, new Color(0.20f, 0.14f, 0.09f, 1f));
        AddOutline(panel, GOLD_DARK, 2f);
        AnchorTopCenterInside(panel.rectTransform, new Vector2(0, -830), new Vector2(1560, 110));

        _infoPortrait = CreateImage(
            "Portrait",
            panel.transform,
            new Color(0.29f, 0.22f, 0.16f, 1f)
        );
        AddOutline(_infoPortrait, GOLD, 2f);
        SetRect(
            _infoPortrait.rectTransform,
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(0, 0.5f),
            new Vector2(12, 0),
            new Vector2(86, 0),
            new Vector2(0, 12),
            new Vector2(0, -12)
        );
        CreateText(
            "PortLbl",
            _infoPortrait.transform,
            "PORT",
            14,
            new Color(0.86f, 0.82f, 0.74f),
            TextAnchor.MiddleCenter,
            FillParent
        );

        _infoName = CreateText(
            "Name",
            panel.transform,
            "",
            22,
            TXT_W,
            TextAnchor.UpperLeft,
            rt => AnchorTopLeftInside(rt, new Vector2(114, -14), new Vector2(420, 26))
        );
        _infoClass = CreateText(
            "Class",
            panel.transform,
            "",
            16,
            TXT_GOLD,
            TextAnchor.UpperLeft,
            rt => AnchorTopLeftInside(rt, new Vector2(114, -44), new Vector2(420, 22))
        );

        // stats — 4 columns
        string[] keys = { "공격력", "방어력", "속도", "치명타" };
        Text[] valTexts = new Text[4];
        for (int i = 0; i < 4; i++)
        {
            float xOff = 540 + i * 200;
            CreateText(
                $"K{i}",
                panel.transform,
                keys[i],
                16,
                TXT_GOLD,
                TextAnchor.UpperLeft,
                rt => AnchorTopLeftInside(rt, new Vector2(xOff, -18), new Vector2(120, 22))
            );
            valTexts[i] = CreateText(
                $"V{i}",
                panel.transform,
                "",
                22,
                TXT_W,
                TextAnchor.UpperLeft,
                rt => AnchorTopLeftInside(rt, new Vector2(xOff, -44), new Vector2(120, 28))
            );
        }
        _infoAtk = valTexts[0];
        _infoDef = valTexts[1];
        _infoSpd = valTexts[2];
        _infoCrit = valTexts[3];

        // HP / SP bars on the right
        var hpBack = CreateImage("HPBack", panel.transform, HP_BACK);
        AddOutline(hpBack, GOLD_DARK, 1f);
        SetRect(
            hpBack.rectTransform,
            new Vector2(1, 1),
            new Vector2(1, 1),
            new Vector2(1, 1),
            new Vector2(-20, -16),
            new Vector2(320, 22)
        );
        _infoHpFill = CreateImage("HPFill", hpBack.transform, HP_FILL);
        _infoHpFill.type = Image.Type.Filled;
        _infoHpFill.fillMethod = Image.FillMethod.Horizontal;
        FillParent(_infoHpFill.rectTransform, 2);
        _infoHp = CreateText(
            "HPTxt",
            hpBack.transform,
            "",
            14,
            TXT_W,
            TextAnchor.MiddleLeft,
            rt =>
            {
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(1, 1);
                rt.offsetMin = new Vector2(8, 0);
                rt.offsetMax = new Vector2(-8, 0);
            }
        );

        var spBack = CreateImage("SPBack", panel.transform, SP_BACK);
        AddOutline(spBack, GOLD_DARK, 1f);
        SetRect(
            spBack.rectTransform,
            new Vector2(1, 1),
            new Vector2(1, 1),
            new Vector2(1, 1),
            new Vector2(-20, -54),
            new Vector2(320, 22)
        );
        _infoSpFill = CreateImage("SPFill", spBack.transform, SP_FILL);
        _infoSpFill.type = Image.Type.Filled;
        _infoSpFill.fillMethod = Image.FillMethod.Horizontal;
        FillParent(_infoSpFill.rectTransform, 2);
        _infoSp = CreateText(
            "SPTxt",
            spBack.transform,
            "",
            14,
            TXT_W,
            TextAnchor.MiddleLeft,
            rt =>
            {
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(1, 1);
                rt.offsetMin = new Vector2(8, 0);
                rt.offsetMax = new Vector2(-8, 0);
            }
        );
    }

    void BuildBottomButtons(Transform modal)
    {
        BuildButton(modal, "CancelBtn", "취소", BTN_GRAY, new Vector2(-180, 70), OnCancel);
        BuildButton(modal, "StartBtn", "전투 시작", BTN_GREEN, new Vector2(180, 70), OnStart);
    }

    void BuildButton(
        Transform parent,
        string name,
        string label,
        Color color,
        Vector2 bottomCenterOffset,
        Action onClick
    )
    {
        var img = CreateImage(name, parent, color);
        AddOutline(img, GOLD, 3f);
        var rt = img.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0);
        rt.sizeDelta = new Vector2(280, 70);
        rt.anchoredPosition = bottomCenterOffset;
        CreateText(
            "Lbl",
            img.transform,
            label,
            26,
            new Color(0.12f, 0.10f, 0.08f),
            TextAnchor.MiddleCenter,
            FillParent
        );
        var btn = img.gameObject.AddComponent<Button>();
        btn.onClick.AddListener(() => onClick?.Invoke());
    }

    // ============ Interactions ============

    void OnGridSlotClicked(int idx)
    {
        if (_party.Contains(idx))
        {
            // already in party -> remove
            int partySlot = _party.IndexOf(idx);
            RemoveFromParty(partySlot);
            return;
        }

        _selectedIndex = idx;
        if (_party.Count < 3)
        {
            _party.Add(idx);
            RefreshPartyRow();
        }
        RefreshSelectionVisuals();
        RefreshInfoPanel();
    }

    void RemoveFromParty(int partySlot)
    {
        if (partySlot < 0 || partySlot >= _party.Count)
            return;
        _party.RemoveAt(partySlot);
        RefreshPartyRow();
        RefreshSelectionVisuals();
    }

    void OnCancel()
    {
        Debug.Log("[CharacterSelect] Cancel");
    }

    void OnStart()
    {
        Debug.Log(
            "[CharacterSelect] Start battle with: "
                + string.Join(", ", _party.ConvertAll(i => roster[i].name))
        );
    }

    // ============ Refresh ============

    void RefreshSelectionVisuals()
    {
        if (_slotOutline == null)
            return;
        for (int i = 0; i < _slotOutline.Length; i++)
        {
            if (_slotOutline[i] == null)
                continue;
            var ol = _slotOutline[i].GetComponent<Outline>();
            if (ol != null)
            {
                bool sel = i == _selectedIndex;
                ol.effectColor = sel ? new Color(1f, 0.93f, 0.43f) : GOLD;
                ol.effectDistance = sel ? new Vector2(4, -4) : new Vector2(2, -2);
            }
            if (_slotPartyOverlay[i] != null)
                _slotPartyOverlay[i].SetActive(_party.Contains(i));
        }
    }

    void RefreshInfoPanel()
    {
        if (_infoName == null)
            return;
        if (_selectedIndex < 0 || _selectedIndex >= roster.Count)
            return;
        var d = roster[_selectedIndex];
        _infoName.text = d.name;
        _infoClass.text = d.className;
        _infoAtk.text = d.atk.ToString();
        _infoDef.text = d.def.ToString();
        _infoSpd.text = d.spd.ToString();
        _infoCrit.text = d.crit + "%";
        _infoHp.text = $"HP {d.hp:N0} / {d.hpMax:N0}";
        _infoSp.text = $"SP {d.sp:N0} / {d.spMax:N0}";
        _infoHpFill.fillAmount = d.hpMax > 0 ? (float)d.hp / d.hpMax : 0f;
        _infoSpFill.fillAmount = d.spMax > 0 ? (float)d.sp / d.spMax : 0f;
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
        Action<RectTransform> layout
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

    void AddOutline(Graphic g, Color color, float thickness = 2f)
    {
        var ol = g.gameObject.AddComponent<Outline>();
        ol.effectColor = color;
        ol.effectDistance = new Vector2(thickness, -thickness);
        ol.useGraphicAlpha = false;
    }

    // ---- layout helpers ----

    void FillParent(RectTransform rt)
    {
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    void FillParent(RectTransform rt, float pad)
    {
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = new Vector2(pad, pad);
        rt.offsetMax = new Vector2(-pad, -pad);
    }

    void SetRect(
        RectTransform rt,
        Vector2 aMin,
        Vector2 aMax,
        Vector2 pivot,
        Vector2 anchoredPos,
        Vector2 size
    )
    {
        rt.anchorMin = aMin;
        rt.anchorMax = aMax;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
    }

    // stretch variant: aMin/aMax differ -> use offsetMin/offsetMax
    void SetRect(
        RectTransform rt,
        Vector2 aMin,
        Vector2 aMax,
        Vector2 pivot,
        Vector2 anchoredPos,
        Vector2 size,
        Vector2 offMin,
        Vector2 offMax
    )
    {
        rt.anchorMin = aMin;
        rt.anchorMax = aMax;
        rt.pivot = pivot;
        if (aMin.x == aMax.x && aMin.y == aMax.y)
        {
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;
        }
        else
        {
            rt.offsetMin = offMin;
            rt.offsetMax = offMax;
        }
    }

    void AnchorTopLeftInside(RectTransform rt, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    void AnchorTopRightInside(RectTransform rt, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1, 1);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    void AnchorTopCenterInside(RectTransform rt, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 1);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }
}
