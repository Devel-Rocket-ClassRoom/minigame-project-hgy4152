using UnityEngine;
using UnityEngine.UI;

public class DrawPhaseUI : MonoBehaviour
{
    [SerializeField]
    DrawPhaseTimer drawPhaseTimer;

    [SerializeField]
    BlockManager blockManager;

    [SerializeField]
    Slider timerSlider;

    [SerializeField]
    Button handPlayButton;

    void Awake()
    {
        handPlayButton.onClick.AddListener(drawPhaseTimer.PlayHandNow);
    }

    void Update()
    {
        timerSlider.value = drawPhaseTimer.RemainingRatio;
        handPlayButton.interactable = blockManager.IsHandFull;
    }
}