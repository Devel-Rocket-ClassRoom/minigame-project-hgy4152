using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 보스 플레이 전용: 같은 구간에서 핸드를 2회 이상 플레이하면
// 구간 시작 상태로 되돌리는 버튼 (커맨드 히스토리 UndoAll).
// 씬 수정 없이 GameManager가 보스 플레이 시작 시 런타임으로 생성한다.
public class HandRewindButtonUI : MonoBehaviour
{
    GameManager _gameManager;
    Button _button;

    public static HandRewindButtonUI Create(GameManager gameManager, Canvas canvas)
    {
        var go = new GameObject(
            "HandRewindButton",
            typeof(RectTransform),
            typeof(Image),
            typeof(Button)
        );
        var rect = (RectTransform)go.transform;
        rect.SetParent(canvas.transform, false);
        rect.anchorMin = rect.anchorMax = new Vector2(1f, 0.5f);
        rect.pivot = new Vector2(1f, 0.5f);
        rect.anchoredPosition = new Vector2(-20f, -120f);
        rect.sizeDelta = new Vector2(200f, 48f);

        var image = go.GetComponent<Image>();
        image.color = new Color(0.15f, 0.15f, 0.2f, 0.9f);

        var labelGO = new GameObject("Label", typeof(RectTransform));
        var labelRect = (RectTransform)labelGO.transform;
        labelRect.SetParent(rect, false);
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        var label = labelGO.AddComponent<TextMeshProUGUI>();
        label.text = "처음으로 되돌리기";
        label.fontSize = 22f;
        label.alignment = TextAlignmentOptions.Center;

        var ui = go.AddComponent<HandRewindButtonUI>();
        ui._gameManager = gameManager;
        ui._button = go.GetComponent<Button>();
        ui._button.targetGraphic = image;
        ui._button.onClick.AddListener(ui.OnClicked);
        ui.Subscribe();
        ui.Refresh();
        return ui;
    }

    void Subscribe()
    {
        _gameManager.OnHandPlayCountChanged += HandleHandCountChanged;
        _gameManager.OnBattlePhaseChanged += HandlePhaseChanged;
    }

    void OnDestroy()
    {
        if (_gameManager == null)
            return;
        _gameManager.OnHandPlayCountChanged -= HandleHandCountChanged;
        _gameManager.OnBattlePhaseChanged -= HandlePhaseChanged;
    }

    void HandleHandCountChanged(int current, int max) => Refresh();

    void HandlePhaseChanged(BattlePhase phase) => Refresh();

    void OnClicked()
    {
        _gameManager.RewindToSegmentStart();
        Refresh();
    }

    void Refresh() => gameObject.SetActive(_gameManager.CanRewindHand);
}
