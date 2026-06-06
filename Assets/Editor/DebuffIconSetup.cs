#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ChainKnights/Setup/Create Debuff Icon UI 메뉴로 실행하세요.
/// 프리팹 생성 + GamePlay 씬 와이어링을 수행합니다.
/// 이미 오브젝트가 있으면 생성은 건너뛰고 필드 연결만 업데이트합니다.
/// </summary>
public static class DebuffIconSetup
{
    const string PrefabRoot = "Assets/Prefabs/UI";

    [MenuItem("ChainKnights/Setup/Create Debuff Icon UI")]
    static void Run()
    {
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Font/JejuGothic SDF.asset");
        if (font == null)
        {
            Debug.LogError("[DebuffSetup] Font not found");
            return;
        }

        CreateModifierEntryPrefab(font);
        CreateDebuffIconPrefab();
        CreateOrUpdateDebuffInfoPopupPrefab();

        WireGamePlayScene();

        AssetDatabase.SaveAssets();
        Debug.Log("[DebuffSetup] Done. GamePlay 씬을 저장하세요.");
    }

    // ── 프리팹 ──────────────────────────────────────────────────────────────

    static void CreateModifierEntryPrefab(TMP_FontAsset font)
    {
        string path = $"{PrefabRoot}/DebuffModifierEntry.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
        {
            Debug.Log("[DebuffSetup] DebuffModifierEntry already exists, skipping.");
            return;
        }

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
        root.AddComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f, 0.8f);

        MakeTMP(root.transform, "NameText", font, 16, FontStyles.Bold, Color.white);
        MakeTMP(
            root.transform,
            "DescText",
            font,
            13,
            FontStyles.Normal,
            new Color(0.85f, 0.85f, 0.85f)
        );

        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        Debug.Log("[DebuffSetup] DebuffModifierEntry.prefab created");
    }

    static void CreateDebuffIconPrefab()
    {
        string path = $"{PrefabRoot}/DebuffIcon.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
        {
            Debug.Log("[DebuffSetup] DebuffIcon already exists, skipping.");
            return;
        }

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

    // DebuffInfoPopup 프리팹: InfoPopupUI 안에 들어가는 심플한 콘텐츠 컨테이너.
    // 슬라이드/백드롭 없음 — InfoPopupUI가 담당.
    static void CreateOrUpdateDebuffInfoPopupPrefab()
    {
        string path = $"{PrefabRoot}/DebuffInfoPopup.prefab";

        // 기존 프리팹 삭제 후 재생성 (구조 변경)
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
            AssetDatabase.DeleteAsset(path);

        var root = new GameObject("DebuffInfoPopup");
        root.AddComponent<RectTransform>();

        // Content: 모디파이어 엔트리를 쌓는 VLG
        var content = new GameObject("Content");
        content.transform.SetParent(root.transform, false);
        var contentRT = content.AddComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0, 1);
        contentRT.anchorMax = new Vector2(1, 1);
        contentRT.pivot = new Vector2(0.5f, 1);
        contentRT.sizeDelta = Vector2.zero;
        var contentVLG = content.AddComponent<VerticalLayoutGroup>();
        contentVLG.childControlWidth = true;
        contentVLG.childControlHeight = true;
        contentVLG.childForceExpandWidth = true;
        contentVLG.childForceExpandHeight = false;
        contentVLG.spacing = 4f;
        contentVLG.padding = new RectOffset(0, 0, 4, 4);
        var contentCSF = content.AddComponent<ContentSizeFitter>();
        contentCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var comp = root.AddComponent<DebuffInfoPopupUI>();
        var so = new SerializedObject(comp);
        so.FindProperty("entryContainer").objectReferenceValue = content.transform;
        so.FindProperty("modifierEntryPrefab").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabRoot}/DebuffModifierEntry.prefab");
        so.ApplyModifiedProperties();

        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        Debug.Log("[DebuffSetup] DebuffInfoPopup.prefab created (simplified content-only)");
    }

    // ── 씬 와이어링 ─────────────────────────────────────────────────────────

    static void WireGamePlayScene()
    {
        var scene = EditorSceneManager.OpenScene(
            "Assets/Scenes/GamePlay.unity",
            OpenSceneMode.Single
        );

        var enemyMgr = FindByName("EnemyManager");
        var infoUIGO = FindByName("InfoUI");
        var iconBarGO = FindByName("DebuffIconBar");

        if (enemyMgr == null || infoUIGO == null)
        {
            Debug.LogError("[DebuffSetup] EnemyManager 또는 InfoUI를 씬에서 찾을 수 없습니다.");
            return;
        }

        var bossPatternSystem = enemyMgr.GetComponent<BossPatternSystem>();
        var infoPopupUI = infoUIGO.GetComponent<InfoPopupUI>();

        // ── DebuffInfoPopup 인스턴스: InfoUI 패널 Content 안에 있어야 함 ──
        var debuffPopupInst = FindDebuffInfoPopup(infoUIGO);
        if (debuffPopupInst == null)
        {
            // 없으면 InfoUI Content 하위에 프리팹 인스턴스 추가
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                $"{PrefabRoot}/DebuffInfoPopup.prefab"
            );
            var content = FindDeepChild(infoUIGO.transform, "Content");
            if (prefab != null && content != null)
            {
                debuffPopupInst = (
                    PrefabUtility.InstantiatePrefab(prefab, content) as GameObject
                ).GetComponent<DebuffInfoPopupUI>();
                debuffPopupInst.gameObject.SetActive(false);
                Debug.Log("[DebuffSetup] DebuffInfoPopup instance added to InfoUI > ... > Content");
            }
            else
            {
                Debug.LogWarning(
                    "[DebuffSetup] InfoUI Content를 찾지 못했습니다. 수동으로 DebuffInfoPopup을 배치하세요."
                );
            }
        }

        // ── InfoPopupUI.debuffInfoPopup 연결 ──
        if (infoPopupUI != null && debuffPopupInst != null)
        {
            var soInfo = new SerializedObject(infoPopupUI);
            soInfo.FindProperty("debuffInfoPopup").objectReferenceValue = debuffPopupInst;
            soInfo.ApplyModifiedProperties();
            Debug.Log("[DebuffSetup] InfoPopupUI.debuffInfoPopup wired");
        }

        // ── DebuffIconBar: 없으면 생성, 있으면 필드만 업데이트 ──
        if (iconBarGO == null)
        {
            var modal = FindByName("Modal");
            if (modal == null)
            {
                Debug.LogError("[DebuffSetup] Modal not found");
                return;
            }

            iconBarGO = new GameObject("DebuffIconBar");
            iconBarGO.transform.SetParent(modal.transform, false);
            var barRT = iconBarGO.AddComponent<RectTransform>();
            barRT.anchorMin = new Vector2(0.5f, 0.5f);
            barRT.anchorMax = new Vector2(0.5f, 0.5f);
            barRT.pivot = new Vector2(0.5f, 0f);
            barRT.sizeDelta = new Vector2(500, 40);
            barRT.anchoredPosition = new Vector2(0, 140);
            var hlg = iconBarGO.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childControlWidth = false;
            hlg.childControlHeight = false;
            hlg.spacing = 6f;
            hlg.padding = new RectOffset(4, 4, 4, 4);
            iconBarGO.AddComponent<DebuffIconBarUI>();
        }

        var barComp = iconBarGO.GetComponent<DebuffIconBarUI>();
        // iconContainer를 자식 컨테이너로 분리해 SetActive 버그 방지
        var iconContainer = iconBarGO.transform.Find("Icons");
        if (iconContainer == null)
        {
            var containerGO = new GameObject("Icons");
            containerGO.transform.SetParent(iconBarGO.transform, false);
            var cRT = containerGO.AddComponent<RectTransform>();
            cRT.anchorMin = Vector2.zero;
            cRT.anchorMax = Vector2.one;
            cRT.offsetMin = cRT.offsetMax = Vector2.zero;
            var cHLG = containerGO.AddComponent<HorizontalLayoutGroup>();
            cHLG.childControlWidth = false;
            cHLG.childControlHeight = false;
            cHLG.spacing = 6f;
            iconContainer = containerGO.transform;
        }

        var soBar = new SerializedObject(barComp);
        soBar.FindProperty("bossPatternSystem").objectReferenceValue = bossPatternSystem;
        soBar.FindProperty("iconContainer").objectReferenceValue = iconContainer;
        soBar.FindProperty("debuffIconPrefab").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabRoot}/DebuffIcon.prefab");
        soBar.FindProperty("infoPopupUI").objectReferenceValue = infoPopupUI;
        soBar.ApplyModifiedProperties();
        Debug.Log("[DebuffSetup] DebuffIconBarUI wired");

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[DebuffSetup] Scene saved.");
    }

    // ── 유틸 ───────────────────────────────────────────────────────────────

    static DebuffInfoPopupUI FindDebuffInfoPopup(GameObject infoUIGO)
    {
        return infoUIGO.GetComponentInChildren<DebuffInfoPopupUI>(true);
    }

    static Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;
            var found = FindDeepChild(child, name);
            if (found != null)
                return found;
        }
        return null;
    }

    static GameObject FindByName(string name) =>
        Resources
            .FindObjectsOfTypeAll<GameObject>()
            .FirstOrDefault(g => g.scene.isLoaded && g.name == name);

    static GameObject MakeTMP(
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
        tmp.textWrappingMode = TextWrappingModes.Normal;
        return go;
    }
}
#endif
