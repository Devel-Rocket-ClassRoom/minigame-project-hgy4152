using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossReadyUI : MonoBehaviour
{
    [Header("Save Slots")]
    public Transform saveSlotParent;
    public SaveSlotUI saveSlotPrefab;

    [Header("Boss Navigation")]
    public BossSlotUI bossDisplay;
    public Button prevBossButton;
    public Button nextBossButton;

    [Header("Pattern Preview")]
    public PatternPreviewUI patternPreview;

    [Header("Deck Summary")]
    public GameObject deckSummaryPanel;
    public TextMeshProUGUI deckCharacterNames;
    public TextMeshProUGUI deckJokerNames;

    [Header("Fullscreen Preview")]
    public CanvasGroup fullscreenPreview;
    public PatternPreviewUI fullscreenPatternPreview;
    public Button enterBossButton;

    [Header("Buttons")]
    public Button startButton;
    public Button backButton;

    private readonly List<SaveSlotUI> saveSlots = new();
    private readonly List<EnemyData> bossList = new();
    private readonly Dictionary<int, SaveSlotData> slotDataCache = new();

    private SaveManager saveManager;
    private SaveSlotUI selectedSaveSlot;
    private int bossIndex = -1;

    private void Start()
    {
        saveManager = FindObjectOfType<SaveManager>();

        if (startButton != null)
            startButton.onClick.AddListener(OnStartClicked);
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
        if (enterBossButton != null)
            enterBossButton.onClick.AddListener(OnEnterBossConfirmed);

        if (fullscreenPreview != null)
        {
            fullscreenPreview.alpha = 0f;
            fullscreenPreview.gameObject.SetActive(false);
        }

        patternPreview?.Hide();
        if (deckSummaryPanel != null)
            deckSummaryPanel.SetActive(false);

        InitSaveSlots();
        InitBossSlots();
        RefreshStartButton();
    }

    private void InitSaveSlots()
    {
        for (int i = 0; i < SaveManager.SlotCount; i++)
        {
            var slotGo = Instantiate(saveSlotPrefab, saveSlotParent);
            var slotUI = slotGo.GetComponent<SaveSlotUI>();

            if (saveManager != null && saveManager.TryLoad(i, out var data))
            {
                slotDataCache[i] = data;
                slotUI.Setup(i, data);
            }
            else
            {
                slotUI.SetupEmpty(i);
            }

            var btn = slotGo.GetComponent<Button>() ?? slotGo.gameObject.AddComponent<Button>();
            btn.onClick.AddListener(slotUI.OnClick);
            slotUI.OnSelected = HandleSaveSlotSelected;
            saveSlots.Add(slotUI);
        }
    }

    private void InitBossSlots()
    {
        var table = TableRegistry.Instance?.Enemy;
        if (table != null)
            foreach (var e in table.All)
                if (e != null && e.bossPattern != null)
                    bossList.Add(e);

        if (prevBossButton != null)
            prevBossButton.onClick.AddListener(OnPrevBoss);
        if (nextBossButton != null)
            nextBossButton.onClick.AddListener(OnNextBoss);

        if (bossList.Count > 0)
            ShowBoss(0);
    }

    private void ShowBoss(int index)
    {
        bossIndex = index;
        bossDisplay?.Setup(bossList[index]);
        patternPreview?.Show(bossList[index].bossPattern);
        RefreshStartButton();
    }

    private void OnPrevBoss()
    {
        if (bossList.Count == 0)
            return;
        ShowBoss((bossIndex - 1 + bossList.Count) % bossList.Count);
    }

    private void OnNextBoss()
    {
        if (bossList.Count == 0)
            return;
        ShowBoss((bossIndex + 1) % bossList.Count);
    }

    private void HandleSaveSlotSelected(SaveSlotUI slot)
    {
        selectedSaveSlot?.SetSelected(false);
        selectedSaveSlot = slot;
        selectedSaveSlot.SetSelected(true);

        slotDataCache.TryGetValue(slot.SlotIndex, out var data);
        UpdateDeckSummary(data);
        RefreshStartButton();
    }

    private void UpdateDeckSummary(SaveSlotData data)
    {
        if (deckSummaryPanel == null)
            return;

        if (data == null)
        {
            deckSummaryPanel.SetActive(false);
            return;
        }

        deckSummaryPanel.SetActive(true);

        if (deckCharacterNames != null)
        {
            var charTable = TableRegistry.Instance?.Character;
            var sb = new System.Text.StringBuilder();
            foreach (var id in data.characterIds)
            {
                if (string.IsNullOrEmpty(id))
                    continue;
                if (charTable != null && charTable.TryGet(id, out var def))
                    sb.AppendLine(Localization.Get(def.displayName));
                else
                    sb.AppendLine(id);
            }
            deckCharacterNames.text = sb.ToString().TrimEnd();
        }

        if (deckJokerNames != null)
        {
            var jokerTable = TableRegistry.Instance?.JokerCard;
            var sb = new System.Text.StringBuilder();
            foreach (var id in data.jokerIds)
            {
                if (string.IsNullOrEmpty(id))
                    continue;
                if (jokerTable != null && jokerTable.TryGet(id, out var card))
                    sb.AppendLine(card.cardName);
                else
                    sb.AppendLine(id);
            }
            deckJokerNames.text = sb.ToString().TrimEnd();
        }
    }

    private void RefreshStartButton()
    {
        if (startButton == null)
            return;
        startButton.interactable = selectedSaveSlot != null && bossIndex >= 0;
    }

    private void OnStartClicked()
    {
        if (selectedSaveSlot == null || bossIndex < 0)
            return;
        StartCoroutine(ShowFullscreenAndWait());
    }

    private IEnumerator ShowFullscreenAndWait()
    {
        fullscreenPatternPreview?.Show(bossList[bossIndex].bossPattern);

        fullscreenPreview.gameObject.SetActive(true);
        float dur = 0.3f;
        for (float t = 0; t < dur; t += Time.deltaTime)
        {
            fullscreenPreview.alpha = t / dur;
            yield return null;
        }
        fullscreenPreview.alpha = 1f;
    }

    private void OnEnterBossConfirmed()
    {
        BossPartyContext.SaveSlotIndex = selectedSaveSlot.SlotIndex;
        BossPartyContext.BossId = bossList[bossIndex].id;
        GameStateMachine.Instance.TransitionTo(GameState.Adventure);
    }

    private void OnBackClicked() => GameStateMachine.Instance.TransitionTo(GameState.Lobby);
}
