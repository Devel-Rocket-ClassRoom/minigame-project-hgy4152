#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// ChainKnights/Setup/Create Boss Turn Routine UI 메뉴로 실행.
/// StageIntroUI 오브젝트에 BossRoutinePanel을 추가하고 필드를 연결합니다.
public static class BossTurnRoutineSetup
{
    [MenuItem("ChainKnights/Setup/Create Boss Turn Routine UI")]
    static void Run()
    {
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Font/JejuGothic SDF.asset");
        if (font == null)
        {
            Debug.LogError("[BossTurnRoutineSetup] Font not found");
            return;
        }

        var scene = EditorSceneManager.OpenScene(
            "Assets/Scenes/GamePlay.unity",
            OpenSceneMode.Single
        );

        var stageIntroGO = FindByName("StageIntroUI");
        if (stageIntroGO == null)
        {
            Debug.LogError("[BossTurnRoutineSetup] StageIntro 오브젝트를 찾을 수 없습니다.");
            return;
        }

        var stageIntroUI = stageIntroGO.GetComponent<StageIntroUI>();
        if (stageIntroUI == null)
        {
            Debug.LogError("[BossTurnRoutineSetup] StageIntroUI 컴포넌트가 없습니다.");
            return;
        }

        // 이미 존재하면 재와이어링만
        var existing = stageIntroGO.transform.Find("BossRoutinePanel");
        GameObject panelGO;
        if (existing != null)
        {
            panelGO = existing.gameObject;
            Debug.Log("[BossTurnRoutineSetup] BossRoutinePanel already exists, re-wiring.");
        }
        else
        {
            panelGO = BuildPanel(stageIntroGO.transform, font);
        }

        WireFields(stageIntroUI, panelGO);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[BossTurnRoutineSetup] Done.");
    }

    static GameObject BuildPanel(Transform parent, TMP_FontAsset font)
    {
        // 루트 패널
        var panel = new GameObject("BossRoutinePanel");
        panel.transform.SetParent(parent, false);
        var panelRT = panel.AddComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0.5f, 0.5f);
        panelRT.anchorMax = new Vector2(0.5f, 0.5f);
        panelRT.sizeDelta = new Vector2(700, 160);
        panelRT.anchoredPosition = Vector2.zero;

        var panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0.05f, 0.05f, 0.1f, 0.88f);

        var vlg = panel.AddComponent<VerticalLayoutGroup>();
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 8f;
        vlg.padding = new RectOffset(20, 20, 20, 20);

        // 이름 텍스트
        MakeTMP(panel.transform, "NameText", font, 22, FontStyles.Bold, new Color(1f, 0.75f, 0.3f));

        // 설명 텍스트
        MakeTMP(
            panel.transform,
            "DescText",
            font,
            16,
            FontStyles.Normal,
            new Color(0.9f, 0.9f, 0.9f)
        );

        panel.SetActive(false);
        return panel;
    }

    static void WireFields(StageIntroUI comp, GameObject panel)
    {
        var so = new SerializedObject(comp);
        so.FindProperty("bossRoutinePanel").objectReferenceValue = panel;
        so.FindProperty("bossRoutineNameText").objectReferenceValue = panel
            .transform.Find("NameText")
            ?.GetComponent<TMP_Text>();
        so.FindProperty("bossRoutineDescText").objectReferenceValue = panel
            .transform.Find("DescText")
            ?.GetComponent<TMP_Text>();
        so.ApplyModifiedProperties();
        Debug.Log("[BossTurnRoutineSetup] Fields wired.");
    }

    static GameObject FindByName(string name) =>
        System.Linq.Enumerable.FirstOrDefault(
            Resources.FindObjectsOfTypeAll<GameObject>(),
            g => g.scene.isLoaded && g.name == name
        );

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
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = true;
    }
}
#endif
