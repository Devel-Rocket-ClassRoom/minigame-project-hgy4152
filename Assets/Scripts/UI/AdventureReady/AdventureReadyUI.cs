using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdventureReadyUI : MonoBehaviour
{
    [Header("Character List")]
    public Transform gridContent;
    public CharacterSlotUI slotPrefab;

    [Header("Top Party Slots")]
    public Image[] partyPortraits;
    public TextMeshProUGUI[] partyNames;

    [Header("Info Panel")]
    public TextMeshProUGUI infoName;
    public TextMeshProUGUI infoPassiveDesc;
    public TextMeshProUGUI infoClass;

    [Header("Buttons")]
    public Button startButton;
    public Button cancelButton;
    public Button backgroundButton;

    [Header("Warning")]
    public CanvasGroup partyWarningGroup;

    private List<CharacterSlotUI> slots = new List<CharacterSlotUI>();
    private CharacterSlotUI selectedSlot;
    private List<CharacterDef> currentParty = new List<CharacterDef>();

    private const int MaxPartySize = 3;

    private void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(OnPlayClicked);
        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnBackClicked);
        if (backgroundButton != null)
            backgroundButton.onClick.AddListener(Deselect);
        InitializeList();
    }

    private void InitializeList()
    {
        foreach (Transform child in gridContent)
            Destroy(child.gameObject);
        slots.Clear();

        var table = TableRegistry.Instance?.Character;
        if (table == null)
        {
            Debug.LogWarning(
                "[AdventureReadyUI] CharacterTable을 찾을 수 없어 목록 생성을 건너뜁니다."
            );
            return;
        }

        var equippedIds = AdventurePartyContext.PendingCharacterIds;
        var equippedSet = equippedIds != null ? new HashSet<string>(equippedIds) : null;

        foreach (var def in table.All)
        {
            if (def == null)
                continue;
            if (!UnlockManager.IsCharacterUnlocked(def.id))
                continue;

            var slotGo = Instantiate(slotPrefab, gridContent);
            var slotUI = slotGo.GetComponent<CharacterSlotUI>();

            bool inParty = equippedSet != null && equippedSet.Contains(def.id);
            if (inParty)
                currentParty.Add(def);

            slotUI.Setup(def, inParty);
            slotUI.OnSelected = HandleSlotSelected;
            slotUI.OnDeployClicked = HandleDeploy;
            slotUI.OnRemoveClicked = HandleRemove;
            slots.Add(slotUI);

            var deployBtn = slotUI.deployButton?.GetComponent<Button>();
            if (deployBtn != null)
                deployBtn.onClick.AddListener(slotUI.OnDeployClick);
            var removeBtn = slotUI.removeButton?.GetComponent<Button>();
            if (removeBtn != null)
                removeBtn.onClick.AddListener(slotUI.OnRemoveClick);

            var btn = slotGo.GetComponent<Button>();
            if (btn == null)
                btn = slotGo.gameObject.AddComponent<Button>();
            btn.onClick.AddListener(slotUI.OnClick);
        }

        UpdatePartyDisplay();
    }

    private void HandleSlotSelected(CharacterSlotUI slot)
    {
        selectedSlot = slot;
        foreach (var s in slots)
            s.UpdateVisuals(s == slot);

        UpdateInfoPanel(slot.Def);
    }

    private void Deselect()
    {
        selectedSlot = null;
        foreach (var s in slots)
            s.UpdateVisuals(false);
    }

    private void HandleDeploy(CharacterSlotUI slot)
    {
        if (currentParty.Count >= MaxPartySize)
            return;
        if (currentParty.Contains(slot.Def))
            return;

        currentParty.Add(slot.Def);
        slot.SetInParty(true);
        slot.UpdateVisuals(slot == selectedSlot);
        UpdatePartyDisplay();
    }

    private void HandleRemove(CharacterSlotUI slot)
    {
        currentParty.Remove(slot.Def);
        slot.SetInParty(false);
        slot.UpdateVisuals(slot == selectedSlot);
        UpdatePartyDisplay();
    }

    private void UpdateInfoPanel(CharacterDef def)
    {
        if (infoName != null)
            infoName.text = Localization.Get(def.displayName);
        if (infoClass != null)
            infoClass.text = def.classType.ToString();
        if (infoPassiveDesc != null)
            infoPassiveDesc.text = Localization.Get(def.description);
    }

    private void UpdatePartyDisplay()
    {
        for (int i = 0; i < partyPortraits.Length; i++)
        {
            if (i < currentParty.Count)
            {
                partyPortraits[i].sprite = currentParty[i].prefab.Icon;
                partyPortraits[i].preserveAspect = true;
                partyPortraits[i].gameObject.SetActive(true);
                if (partyNames != null && i < partyNames.Length && partyNames[i] != null)
                {
                    partyNames[i].text = Localization.Get(currentParty[i].displayName);
                    partyNames[i].gameObject.SetActive(true);
                }
            }
            else
            {
                partyPortraits[i].gameObject.SetActive(false);
                if (partyNames != null && i < partyNames.Length && partyNames[i] != null)
                    partyNames[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnPlayClicked()
    {
        if (currentParty.Count < MaxPartySize)
        {
            StartCoroutine(ShowPartyWarning());
            return;
        }

        AdventurePartyContext.PendingCharacterIds = currentParty.Select(d => d.id).ToArray();
        GameStateMachine.Instance.TransitionTo(GameState.Adventure);
    }

    public void OnBackClicked() => GameStateMachine.Instance.TransitionTo(GameState.Lobby);

    private IEnumerator ShowPartyWarning()
    {
        if (partyWarningGroup == null)
            yield break;

        partyWarningGroup.gameObject.SetActive(true);

        float duration = 0.3f;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            partyWarningGroup.alpha = t / duration;
            yield return null;
        }
        partyWarningGroup.alpha = 1f;

        yield return new WaitForSeconds(1f);

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            partyWarningGroup.alpha = 1f - t / duration;
            yield return null;
        }
        partyWarningGroup.alpha = 0f;
        partyWarningGroup.gameObject.SetActive(false);
    }
}
