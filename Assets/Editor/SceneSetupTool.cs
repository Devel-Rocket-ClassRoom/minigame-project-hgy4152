#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public static class SceneSetupTool
{
    [MenuItem("ChainKnights/Setup/Create Title-Lobby-AdventureReady Scenes")]
    public static void CreateAllScenes()
    {
        CreateTitleScene();
        CreateLobbyScene();
        CreateAdventureReadyScene();
        AddScenesToBuildSettings();
        Debug.Log("[SceneSetupTool] 씬 3개 생성 및 Build Settings 등록 완료");
    }

    static void CreateTitleScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // GameStateMachine
        var gsm = new GameObject("GameStateMachine");
        gsm.AddComponent<GameStateMachine>();

        // Camera
        var camGO = new GameObject("Main Camera");
        var cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
        camGO.tag = "MainCamera";

        // Canvas
        var canvas = CreateCanvas("Canvas");
        var titleUI = canvas.gameObject.AddComponent<TitleUI>();

        // Title text
        var titleText = CreateText(canvas, "TitleText", "ChainKnights", 72, FontStyle.Bold);
        titleText.rectTransform.anchoredPosition = new Vector2(0, 80);

        // Hint text
        var hintText = CreateText(canvas, "HintText", "터치하여 시작", 32, FontStyle.Normal);
        hintText.rectTransform.anchoredPosition = new Vector2(0, -80);

        CreateEventSystem();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Title.unity");
    }

    static void CreateLobbyScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var camGO = new GameObject("Main Camera");
        var cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
        camGO.tag = "MainCamera";

        var canvas = CreateCanvas("Canvas");
        var lobbyUI = canvas.gameObject.AddComponent<LobbyUI>();

        var titleText = CreateText(canvas, "TitleText", "모드 선택", 56, FontStyle.Bold);
        titleText.rectTransform.anchoredPosition = new Vector2(0, 200);

        // Adventure button
        var advBtn = CreateButton(canvas, "AdventureButton", "모험 모드");
        advBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50);
        advBtn.onClick.AddListener(lobbyUI.OnAdventureClicked);

        // Boss button (disabled)
        var bossBtn = CreateButton(canvas, "BossModeButton", "보스 모드 (준비 중)");
        bossBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -80);
        bossBtn.interactable = false;

        CreateEventSystem();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Lobby.unity");
    }

    static void CreateAdventureReadyScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var camGO = new GameObject("Main Camera");
        var cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
        camGO.tag = "MainCamera";

        var canvas = CreateCanvas("Canvas");
        var readyUI = canvas.gameObject.AddComponent<AdventureReadyUI>();

        var titleText = CreateText(canvas, "TitleText", "모험 준비", 56, FontStyle.Bold);
        titleText.rectTransform.anchoredPosition = new Vector2(0, 200);

        var playBtn = CreateButton(canvas, "PlayButton", "플레이 시작");
        playBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50);
        playBtn.onClick.AddListener(readyUI.OnPlayClicked);

        var backBtn = CreateButton(canvas, "BackButton", "← 돌아가기");
        backBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -80);
        backBtn.onClick.AddListener(readyUI.OnBackClicked);

        CreateEventSystem();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/AdventureReady.unity");
    }

    static void AddScenesToBuildSettings()
    {
        var scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/Title.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Lobby.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/AdventureReady.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/GamePlay.unity", true),
        };
        EditorBuildSettings.scenes = scenes;
    }

    // ── helpers ──────────────────────────────────────────────────

    static Canvas CreateCanvas(string name)
    {
        var go = new GameObject(name);
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        go.AddComponent<CanvasScaler>();
        go.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    static TMP_Text CreateText(Canvas parent, string name, string text, int size, FontStyle style)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(600, 100);
        return tmp;
    }

    static Button CreateButton(Canvas parent, string name, string label)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var img = go.AddComponent<Image>();
        img.color = new Color(0.2f, 0.4f, 0.8f);
        var btn = go.AddComponent<Button>();
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(300, 80);

        var textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 28;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        var trt = textGO.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.sizeDelta = Vector2.zero;

        return btn;
    }

    static void CreateEventSystem()
    {
        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
    }
}
#endif
