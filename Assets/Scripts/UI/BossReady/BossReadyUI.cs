using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossReadyUI : MonoBehaviour
{
    [Header("Save Slot Carousel")]
    public SaveSlotUI saveSlotDisplay;
    public SaveSlotSwipeArea saveSlotSwipeArea;

    [Header("Boss Navigation")]
    public BossSlotUI bossDisplay;
    public Button prevBossButton;
    public Button nextBossButton;

    [Header("Pattern Preview")]
    public PatternPreviewUI patternPreview;

    [Header("Buttons")]
    public Button startButton;
    public Button backButton;

    private readonly List<(int slotIndex, SaveSlotData data)> saveSlotList = new();
    private readonly List<BossData> bossList = new();

    [SerializeField]
    private SaveManager saveManager;

    [SerializeField]
    SaveSlotInfoPanel saveSlotInfoPanel;

    [SerializeField]
    GameObject emptySlotMessage;

    private int saveSlotIndex = -1;
    private int bossIndex = -1;

    private void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(OnStartClicked);
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);

        patternPreview?.Hide();

        InitSaveSlots();
        InitBossSlots();
        RefreshStartButton();
    }

    private void InitSaveSlots()
    {
        if (saveManager != null)
        {
            for (int i = 0; i < SaveManager.SlotCount; i++)
            {
                if (saveManager.TryLoad(i, out var data))
                    saveSlotList.Add((i, data));
            }
        }

        if (saveSlotSwipeArea != null)
        {
            saveSlotSwipeArea.OnSwipeLeft = OnNextSaveSlot;
            saveSlotSwipeArea.OnSwipeRight = OnPrevSaveSlot;
        }

        bool hasSlots = saveSlotList.Count > 0;
        if (emptySlotMessage != null)
            emptySlotMessage.SetActive(!hasSlots);

        if (hasSlots)
            ShowSaveSlot(0);
    }

    private void InitBossSlots()
    {
        var table = TableRegistry.Instance?.Boss;
        if (table != null)
            foreach (var b in table.All)
                if (b != null)
                    bossList.Add(b);

        if (prevBossButton != null)
            prevBossButton.onClick.AddListener(OnPrevBoss);
        if (nextBossButton != null)
            nextBossButton.onClick.AddListener(OnNextBoss);

        if (bossList.Count > 0)
            ShowBoss(0);
    }

    private void ShowSaveSlot(int index)
    {
        if (saveSlotList.Count == 0 || saveSlotDisplay == null)
            return;
        saveSlotIndex = index;
        var entry = saveSlotList[index];
        saveSlotDisplay.OnInfoRequested -= OnSlotInfoRequested;
        saveSlotDisplay.Setup(entry.slotIndex, entry.data);
        saveSlotDisplay.OnInfoRequested += OnSlotInfoRequested;
        RefreshStartButton();
    }

    private void OnPrevSaveSlot()
    {
        if (saveSlotList.Count == 0)
            return;
        ShowSaveSlot((saveSlotIndex - 1 + saveSlotList.Count) % saveSlotList.Count);
    }

    private void OnNextSaveSlot()
    {
        if (saveSlotList.Count == 0)
            return;
        ShowSaveSlot((saveSlotIndex + 1) % saveSlotList.Count);
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

    private void RefreshStartButton()
    {
        if (startButton == null)
            return;
        startButton.interactable = saveSlotIndex >= 0 && bossIndex >= 0;
    }

    private void OnStartClicked()
    {
        if (saveSlotIndex < 0 || bossIndex < 0)
            return;
        BossPartyContext.SaveSlotIndex = saveSlotList[saveSlotIndex].slotIndex;
        BossPartyContext.BossId = bossList[bossIndex].id;
        GameStateMachine.Instance.TransitionTo(GameState.Adventure);
    }

    private void OnSlotInfoRequested(SaveSlotData data) => saveSlotInfoPanel?.Show(data);

    private void OnBackClicked() => GameStateMachine.Instance.TransitionTo(GameState.Lobby);
}
