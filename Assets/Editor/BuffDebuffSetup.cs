#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ChainKnights/Setup/Create BuffDebuff UI 메뉴로 실행하세요.
/// 1) BuffIcon.prefab, BuffDebuffSlot.prefab 생성
/// 2) GamePlay 씬에 PartyBuffBarUI + BuffDebuffInfoPanelUI 와이어링
/// </summary>
public static class BuffDebuffSetup
{
    const string PrefabRoot = "Assets/Prefabs/UI";

    [MenuItem("ChainKnights/Setup/Create BuffDebuff UI")]
    static void Run()
    {
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Font/JejuGothic SDF.asset");
        if (font == null)
        {
            Debug.LogError("[BuffDebuffSetup] Font not found");
            return;
        }

        CreateBuffIconPrefab();
        CreateBuffDebuffSlotPrefab(font);

        WireGamePlayScene();

        AssetDatabase.SaveAssets();
        Debug.Log("[BuffDebuffSetup] Done. GamePlay 씬을 저장하세요.");
    }

    // ── 프리팹 ──────────────────────────────────────────────────────────────

    static void CreateBuffIconPrefab()
    {
        string path = $"{PrefabRoot}/BuffIcon.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
        {
            Debug.Log("[BuffDebuffSetup] BuffIcon already exists, skipping.");
            return;
        }

        var root = new GameObject("BuffIcon");
        root.AddComponent<Image>().color = new Color(0.2f, 0.55f, 0.2f, 0.9f);
        root.AddComponent<Button>();
        var le = root.AddComponent<LayoutElement>();
        le.preferredWidth = 40;
        le.preferredHeight = 40;
        le.minWidth = 40;
        le.minHeight = 40;

        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        Debug.Log("[BuffDebuffSetup] BuffIcon.prefab created");
    }

    // Icon(Image) + NameText + DescText 1열 슬롯.
    // BuffDebuffInfoPanelUI의 slotContainer에 동적으로 채워짐.
    static void CreateBuffDebuffSlotPrefab(TMP_FontAsset font)
    {
        string path = $"{PrefabRoot}/BuffDebuffSlot.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
        {
            Debug.Log("[BuffDebuffSetup] BuffDebuffSlot already exists, skipping.");
            return;
        }

        // Root: HorizontalLayoutGroup
        var root = new GameObject("BuffDebuffSlot");
        var hlg = root.AddComponent<HorizontalLayoutGroup>();
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childControlWidth = false;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;
        hlg.spacing = 8f;
        hlg.padding = new RectOffset(6, 6, 4, 4);
        var csf = root.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        root.AddComponent<Image>().color = new Color(0.12f, 0.12f, 0.18f, 0.85f);

        // Icon (Image only — sprite가 없으면 투명)
        var iconGO = new GameObject("Icon");
        iconGO.transform.SetParent(root.transform, false);
        iconGO.AddComponent<Image>().color = Color.white;
        var iconLE = iconGO.AddComponent<LayoutElement>();
        iconLE.minWidth = 40;
        iconLE.minHeight = 40;
        iconLE.preferredWidth = 40;
        iconLE.preferredHeight = 40;

        // TextGroup: VerticalLayoutGroup
        var textGroup = new GameObject("TextGroup");
        textGroup.transform.SetParent(root.transform, false);
        var textLE = textGroup.AddComponent<LayoutElement>();
        textLE.flexibleWidth = 1;
        var vlg = textGroup.AddComponent<VerticalLayoutGroup>();
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 2f;

        MakeTMP(textGroup.transform, "NameText", font, 14, FontStyles.Bold, Color.white);
        MakeTMP(
            textGroup.transform,
            "DescText",
            font,
            12,
            FontStyles.Normal,
            new Color(0.82f, 0.82f, 0.82f)
        );

        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        Debug.Log("[BuffDebuffSetup] BuffDebuffSlot.prefab created");
    }

    // ── 씬 와이어링 ─────────────────────────────────────────────────────────

    static void WireGamePlayScene()
    {
        var scene = EditorSceneManager.OpenScene(
            "Assets/Scenes/GamePlay.unity",
            OpenSceneMode.Single
        );

        var partyFrameGO = FindByName("PartyFrame");
        var enemyMgrGO = FindByName("EnemyManager");
        var modalGO = FindByName("Modal");

        if (partyFrameGO == null || enemyMgrGO == null || modalGO == null)
        {
            Debug.LogError(
                "[BuffDebuffSetup] PartyFrame / EnemyManager / Modal 중 하나를 씬에서 찾지 못했습니다."
            );
            return;
        }

        var bossPatternSystem = enemyMgrGO.GetComponent<BossPatternSystem>();
        var characterSet = FindComponentInScene<CharacterSet>();
        var jokerManager = FindComponentInScene<JokerManager>();

        // ── BuffDebuffInfoPanel 생성 (씬 내 Modal 하위) ──
        var panelGO = FindByName("BuffDebuffInfoPanel");
        if (panelGO == null)
            panelGO = CreateBuffDebuffInfoPanelInScene(modalGO.transform);

        var infoPanelComp = panelGO.GetComponent<BuffDebuffInfoPanelUI>();

        // ── PartyBuffBar 생성 (PartyFrame 하위) ──
        var barGO = FindByName("PartyBuffBar");
        if (barGO == null)
        {
            barGO = new GameObject("PartyBuffBar");
            barGO.transform.SetParent(partyFrameGO.transform, false);
            var rt = barGO.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(0, 48);
            rt.anchoredPosition = new Vector2(0, 0);
            var hlg = barGO.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childControlWidth = false;
            hlg.childControlHeight = false;
            hlg.spacing = 4f;
            hlg.padding = new RectOffset(4, 4, 4, 4);
            barGO.AddComponent<PartyBuffBarUI>();
        }

        var barComp = barGO.GetComponent<PartyBuffBarUI>();
        var soBar = new SerializedObject(barComp);
        soBar.FindProperty("jokerManager").objectReferenceValue = jokerManager;
        soBar.FindProperty("bossPatternSystem").objectReferenceValue = bossPatternSystem;
        soBar.FindProperty("characterSet").objectReferenceValue = characterSet;
        soBar.FindProperty("iconContainer").objectReferenceValue = barGO.transform;
        soBar.FindProperty("buffIconPrefab").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabRoot}/BuffIcon.prefab");
        soBar.FindProperty("debuffIconPrefab").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabRoot}/DebuffIcon.prefab");
        soBar.FindProperty("infoPanel").objectReferenceValue = infoPanelComp;
        soBar.ApplyModifiedProperties();
        Debug.Log("[BuffDebuffSetup] PartyBuffBarUI wired");

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[BuffDebuffSetup] Scene saved.");
    }

    static GameObject CreateBuffDebuffInfoPanelInScene(Transform parent)
    {
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Font/JejuGothic SDF.asset");
        var slotPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            $"{PrefabRoot}/BuffDebuffSlot.prefab"
        );

        // Root
        var root = new GameObject("BuffDebuffInfoPanel");
        root.transform.SetParent(parent, false);
        var rootRT = root.AddComponent<RectTransform>();
        rootRT.anchorMin = Vector2.zero;
        rootRT.anchorMax = Vector2.one;
        rootRT.offsetMin = rootRT.offsetMax = Vector2.zero;

        // Backdrop Button (transparent fullscreen)
        var backdropGO = new GameObject("Backdrop");
        backdropGO.transform.SetParent(root.transform, false);
        var backdropRT = backdropGO.AddComponent<RectTransform>();
        backdropRT.anchorMin = Vector2.zero;
        backdropRT.anchorMax = Vector2.one;
        backdropRT.offsetMin = backdropRT.offsetMax = Vector2.zero;
        var backdropImg = backdropGO.AddComponent<Image>();
        backdropImg.color = new Color(0, 0, 0, 0.01f);
        backdropGO.AddComponent<Button>();

        // Panel (the sliding element) — anchored to right side
        var panelGO = new GameObject("Panel");
        panelGO.transform.SetParent(root.transform, false);
        var panelRT = panelGO.AddComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(1, 0);
        panelRT.anchorMax = new Vector2(1, 1);
        panelRT.pivot = new Vector2(1, 0.5f);
        panelRT.sizeDelta = new Vector2(320, 0);
        panelRT.anchoredPosition = Vector2.zero;
        var panelBg = panelGO.AddComponent<Image>();
        panelBg.color = new Color(0.08f, 0.08f, 0.12f, 0.95f);

        // SlotContainer inside Panel
        var containerGO = new GameObject("SlotContainer");
        containerGO.transform.SetParent(panelGO.transform, false);
        var containerRT = containerGO.AddComponent<RectTransform>();
        containerRT.anchorMin = new Vector2(0, 1);
        containerRT.anchorMax = new Vector2(1, 1);
        containerRT.pivot = new Vector2(0.5f, 1);
        containerRT.sizeDelta = Vector2.zero;
        var vlg = containerGO.AddComponent<VerticalLayoutGroup>();
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 4f;
        vlg.padding = new RectOffset(8, 8, 8, 8);
        var csf = containerGO.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // BuffDebuffInfoPanelUI 컴포넌트 연결
        var comp = root.AddComponent<BuffDebuffInfoPanelUI>();
        var so = new SerializedObject(comp);
        so.FindProperty("panel").objectReferenceValue = panelRT;
        so.FindProperty("backdropButton").objectReferenceValue = backdropGO.GetComponent<Button>();
        so.FindProperty("slotContainer").objectReferenceValue = containerGO.transform;
        so.FindProperty("slotPrefab").objectReferenceValue = slotPrefab;
        so.FindProperty("slideFromRight").boolValue = true;
        so.ApplyModifiedProperties();

        Debug.Log("[BuffDebuffSetup] BuffDebuffInfoPanel created in scene");
        return root;
    }

    // ── 유틸 ───────────────────────────────────────────────────────────────

    static T FindComponentInScene<T>()
        where T : Component => Object.FindObjectsOfType<T>(true).FirstOrDefault();

    static GameObject FindByName(string name) =>
        Resources
            .FindObjectsOfTypeAll<GameObject>()
            .FirstOrDefault(g => g.scene.isLoaded && g.name == name);

    static void MakeTMP(
        Transform parent,
        string goName,
        TMP_FontAsset font,
        float size,
        FontStyles style,
        Color color
    )
    {
        var go = new GameObject(goName);
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.font = font;
        tmp.fontSize = size;
        tmp.fontStyle = style;
        tmp.color = color;
        tmp.enableWordWrapping = true;
    }
}
#endif
