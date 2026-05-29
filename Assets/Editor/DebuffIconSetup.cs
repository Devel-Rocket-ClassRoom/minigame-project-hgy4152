#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ChainKnights/Setup/Create Debuff Icon UI 메뉴로 실행하세요.
/// GamePlay 씬에 DebuffIconBar, DebuffInfoPopup 오브젝트를 생성하고
/// 프리팹 3종(DebuffIcon, DebuffModifierEntry, DebuffInfoPopup)을 만듭니다.
/// </summary>
public static class DebuffIconSetup
{
    const string PrefabRoot = "Assets/Prefabs/UI";

    [MenuItem("ChainKnights/Setup/Create Debuff Icon UI")]
    static void Run()
    {
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Font/JejuGothic SDF.asset");
        if (font == null) { Debug.LogError("[DebuffSetup] Font not found"); return; }

        CreateModifierEntryPrefab(font);
        CreateDebuffIconPrefab();
        var popupPrefab = CreateDebuffInfoPopupPrefab(font);

        SetupGamePlayScene(popupPrefab);

        AssetDatabase.SaveAssets();
        Debug.Log("[DebuffSetup] Done. GamePlay 씬을 저장하세요.");
    }

    // ── 프리팹 ──────────────────────────────────────────────────────────────

    static void CreateModifierEntryPrefab(TMP_FontAsset font)
    {
        string path = $"{PrefabRoot}/DebuffModifierEntry.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
        { Debug.Log("[DebuffSetup] DebuffModifierEntry already exists, skipping."); return; }

        var root = new GameObject("DebuffModifierEntry");

        var vlg = root.AddComponent<VerticalLayoutGroup>();
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 4f;
        vlg.padding = new RectOffset(8, 8, 6, 6);

        var csf = root.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // 구분선 배경
        var bg = root.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.2f, 0.8f);

        MakeTMP(root.transform, "NameText", font, 16, FontStyles.Bold, Color.white);
        MakeTMP(root.transform, "DescText", font, 13, FontStyles.Normal, new Color(0.85f, 0.85f, 0.85f));

        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        Debug.Log("[DebuffSetup] DebuffModifierEntry.prefab created");
    }

    static void CreateDebuffIconPrefab()
    {
        string path = $"{PrefabRoot}/DebuffIcon.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
        { Debug.Log("[DebuffSetup] DebuffIcon already exists, skipping."); return; }

        var root = new GameObject("DebuffIcon");
        root.AddComponent<Image>().color = new Color(0.75f, 0.2f, 0.2f, 0.9f);
        root.AddComponent<Button>();
        var le = root.AddComponent<LayoutElement>();
        le.preferredWidth = 40;
        le.preferredHeight = 40;
        le.minWidth = 40;
        le.minHeight = 40;

        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        Debug.Log("[DebuffSetup] DebuffIcon.prefab created");
    }

    static GameObject CreateDebuffInfoPopupPrefab(TMP_FontAsset font)
    {
        string path = $"{PrefabRoot}/DebuffInfoPopup.prefab";
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null)
        { Debug.Log("[DebuffSetup] DebuffInfoPopup already exists, skipping."); return existing; }

        // Root (DebuffInfoPopupUI 컴포넌트 보유)
        var root = new GameObject("DebuffInfoPopup");
        root.AddComponent<RectTransform>();

        // Backdrop (전체화면 투명 버튼)
        var backdrop = new GameObject("Backdrop");
        backdrop.transform.SetParent(root.transform, false);
        var bdRT = backdrop.AddComponent<RectTransform>();
        bdRT.anchorMin = Vector2.zero;
        bdRT.anchorMax = Vector2.one;
        bdRT.offsetMin = bdRT.offsetMax = Vector2.zero;
        var bdImg = backdrop.AddComponent<Image>();
        bdImg.color = new Color(0, 0, 0, 0.4f);
        backdrop.AddComponent<Button>();

        // Panel (슬라이드 인)
        var panel = new GameObject("Panel");
        panel.transform.SetParent(root.transform, false);
        var panelRT = panel.AddComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(1, 0.5f);
        panelRT.anchorMax = new Vector2(1, 0.5f);
        panelRT.pivot = new Vector2(1, 0.5f);
        panelRT.sizeDelta = new Vector2(420, 600);
        panelRT.anchoredPosition = new Vector2(0, 0);
        var panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

        var vlg = panel.AddComponent<VerticalLayoutGroup>();
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 0;
        vlg.padding = new RectOffset(0, 0, 0, 0);

        // 헤더
        var header = new GameObject("Header");
        header.transform.SetParent(panel.transform, false);
        var headerRT = header.AddComponent<RectTransform>();
        headerRT.sizeDelta = new Vector2(420, 50);
        var headerImg = header.AddComponent<Image>();
        headerImg.color = new Color(0.2f, 0.2f, 0.3f, 1f);
        var le = header.AddComponent<LayoutElement>();
        le.preferredHeight = 50;
        le.flexibleHeight = 0;
        var headerTxt = MakeTMP(header.transform, "HeaderText", font, 17, FontStyles.Bold, Color.white);
        var hrt = headerTxt.GetComponent<RectTransform>();
        hrt.anchorMin = Vector2.zero; hrt.anchorMax = Vector2.one;
        hrt.offsetMin = new Vector2(12, 0); hrt.offsetMax = Vector2.zero;
        headerTxt.GetComponent<TMP_Text>().text = "현재 디버프";
        headerTxt.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.MidlineLeft;

        // ScrollRect
        var scrollGO = new GameObject("Scroll");
        scrollGO.transform.SetParent(panel.transform, false);
        var scrollRT = scrollGO.AddComponent<RectTransform>();
        scrollRT.sizeDelta = new Vector2(420, 540);
        var scrollLE = scrollGO.AddComponent<LayoutElement>();
        scrollLE.preferredHeight = 540;
        scrollLE.flexibleHeight = 1;
        var scroll = scrollGO.AddComponent<ScrollRect>();
        scroll.horizontal = false;

        // Viewport
        var viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollGO.transform, false);
        var vpRT = viewport.AddComponent<RectTransform>();
        vpRT.anchorMin = Vector2.zero; vpRT.anchorMax = Vector2.one;
        vpRT.offsetMin = vpRT.offsetMax = Vector2.zero;
        viewport.AddComponent<Image>().color = Color.clear;
        viewport.AddComponent<Mask>().showMaskGraphic = false;

        // Content
        var content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        var contentRT = content.AddComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0, 1);
        contentRT.anchorMax = new Vector2(1, 1);
        contentRT.pivot = new Vector2(0.5f, 1);
        contentRT.sizeDelta = new Vector2(0, 0);
        var contentVLG = content.AddComponent<VerticalLayoutGroup>();
        contentVLG.childControlWidth = true;
        contentVLG.childControlHeight = true;
        contentVLG.childForceExpandWidth = true;
        contentVLG.childForceExpandHeight = false;
        contentVLG.spacing = 2f;
        contentVLG.padding = new RectOffset(8, 8, 8, 8);
        var contentCSF = content.AddComponent<ContentSizeFitter>();
        contentCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scroll.viewport = vpRT;
        scroll.content = contentRT;

        // DebuffInfoPopupUI 연결
        var comp = root.AddComponent<DebuffInfoPopupUI>();
        var so = new SerializedObject(comp);
        so.FindProperty("panel").objectReferenceValue = panelRT;
        so.FindProperty("backdropButton").objectReferenceValue = backdrop.GetComponent<Button>();
        so.FindProperty("entryContainer").objectReferenceValue = content.transform;
        so.FindProperty("modifierEntryPrefab").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabRoot}/DebuffModifierEntry.prefab");
        so.ApplyModifiedProperties();

        var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        Debug.Log("[DebuffSetup] DebuffInfoPopup.prefab created");
        return prefab;
    }

    // ── 씬 배치 ────────────────────────────────────────────────────────────

    static void SetupGamePlayScene(GameObject popupPrefab)
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/GamePlay.unity", OpenSceneMode.Single);

        var modal = FindByName("Modal");
        var slotsGO = FindByName("Slots");
        var enemyMgr = FindByName("EnemyManager");

        if (modal == null || slotsGO == null || enemyMgr == null)
        {
            Debug.LogError("[DebuffSetup] Could not find Modal/Slots/EnemyManager in scene.");
            return;
        }

        var bossPatternSystem = enemyMgr.GetComponent<BossPatternSystem>();

        // DebuffInfoPopup (Modal의 sibling Canvas 레벨에 배치)
        var popupInst = PrefabUtility.InstantiatePrefab(popupPrefab, modal.transform.parent) as GameObject;
        popupInst.name = "DebuffInfoPopup";
        var popupRT = popupInst.GetComponent<RectTransform>();
        popupRT.anchorMin = Vector2.zero;
        popupRT.anchorMax = Vector2.one;
        popupRT.offsetMin = popupRT.offsetMax = Vector2.zero;
        var popupComp = popupInst.GetComponent<DebuffInfoPopupUI>();

        // DebuffIconBar (Slots의 sibling, Modal 내부)
        var iconBar = new GameObject("DebuffIconBar");
        iconBar.transform.SetParent(modal.transform, false);
        var barRT = iconBar.AddComponent<RectTransform>();
        // Slots 위쪽에 배치 (Slots: y=-97, h=450 → top은 y=128, 바는 그 위)
        barRT.anchorMin = new Vector2(0.5f, 0.5f);
        barRT.anchorMax = new Vector2(0.5f, 0.5f);
        barRT.pivot = new Vector2(0.5f, 0f);
        barRT.sizeDelta = new Vector2(500, 40);
        barRT.anchoredPosition = new Vector2(0, 140);  // Slots top 근처 위

        var hlg = iconBar.AddComponent<HorizontalLayoutGroup>();
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childControlWidth = false;
        hlg.childControlHeight = false;
        hlg.spacing = 6f;
        hlg.padding = new RectOffset(4, 4, 4, 4);

        var barComp = iconBar.AddComponent<DebuffIconBarUI>();
        var so = new SerializedObject(barComp);
        so.FindProperty("bossPatternSystem").objectReferenceValue = bossPatternSystem;
        so.FindProperty("iconContainer").objectReferenceValue = iconBar.transform;
        so.FindProperty("debuffIconPrefab").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabRoot}/DebuffIcon.prefab");
        so.FindProperty("debuffInfoPopup").objectReferenceValue = popupComp;
        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[DebuffSetup] Scene updated. DebuffIconBar + DebuffInfoPopup added.");
    }

    // ── 유틸 ───────────────────────────────────────────────────────────────

    static GameObject FindByName(string name) =>
        Resources.FindObjectsOfTypeAll<GameObject>()
            .FirstOrDefault(g => g.scene.isLoaded && g.name == name);

    static GameObject MakeTMP(Transform parent, string goName, TMP_FontAsset font,
        float size, FontStyles style, Color color)
    {
        var go = new GameObject(goName);
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.font = font;
        tmp.fontSize = size;
        tmp.fontStyle = style;
        tmp.color = color;
        tmp.enableWordWrapping = true;
        return go;
    }
}
#endif
