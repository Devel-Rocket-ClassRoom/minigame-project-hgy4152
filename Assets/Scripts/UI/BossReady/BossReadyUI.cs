using System.Collections.Generic;
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

        patternPreview?.Hide();

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
        RefreshStartButton();
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
        BossPartyContext.SaveSlotIndex = selectedSaveSlot.SlotIndex;
        BossPartyContext.BossId = bossList[bossIndex].id;
        GameStateMachine.Instance.TransitionTo(GameState.Adventure);
    }

    private void OnBackClicked() => GameStateMachine.Instance.TransitionTo(GameState.Lobby);
}
