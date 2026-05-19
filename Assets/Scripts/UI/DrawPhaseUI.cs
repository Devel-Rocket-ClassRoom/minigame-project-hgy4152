using UnityEngine;
using UnityEngine.UI;

public class DrawPhaseUI : MonoBehaviour
{
    DrawPhaseTimer drawPhaseTimer;
    BlockManager blockManager;
    Slider timerSlider;
    Button handPlayButton;

    void Awake()
    {
        drawPhaseTimer = GetComponent<DrawPhaseTimer>();
        blockManager = GetComponent<BlockManager>();
        timerSlider = GetComponentInChildren<Slider>();
        handPlayButton = GetComponentInChildren<Button>();
        handPlayButton.onClick.AddListener(drawPhaseTimer.PlayHandNow);
    }

    void Update()
    {
        timerSlider.value = drawPhaseTimer.RemainingRatio;
        handPlayButton.interactable = blockManager.IsHandFull;
    }
}
