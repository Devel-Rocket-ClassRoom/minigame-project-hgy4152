using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossReadyUI : MonoBehaviour {
    [Header("Selection Slots")]
    public Transform saveSlotParent;
    public Transform bossSlotParent;
    
    [Header("Preview Panels")]
    public PatternPreviewUI patternPreview;
    public TextMeshProUGUI deckSummaryText;
    public CanvasGroup fullscreenPreview;
    
    [Header("Buttons")]
    public Button startButton;
    public Button backButton;
}
