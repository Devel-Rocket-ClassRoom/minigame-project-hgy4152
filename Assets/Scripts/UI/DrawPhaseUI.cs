using UnityEngine;
using UnityEngine.UI;

public class DrawPhaseUI : MonoBehaviour
{
    DrawPhaseTimer drawPhaseTimer;
    BlockManager blockManager;

    [SerializeField]
    Slider timerSlider;

    [SerializeField]
    Button handPlayButton;

    void Awake()
    {
        drawPhaseTimer = GetComponent<DrawPhaseTimer>();
        blockManager = GetComponent<BlockManager>();
        handPlayButton.onClick.AddListener(drawPhaseTimer.PlayHandNow);
    }

    void Update()
    {
        timerSlider.value = drawPhaseTimer.RemainingRatio;
        handPlayButton.interactable = blockManager.IsHandFull;
    }
}
