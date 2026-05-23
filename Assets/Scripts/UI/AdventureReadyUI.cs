using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

public class AdventureReadyUI : MonoBehaviour
{
    [Header("Character List")]
    public Transform gridContent;
    public CharacterSlotUI slotPrefab;

    [Header("Top Party Slots")]
    public Image[] partyPortraits;

    [Header("Info Panel")]
    public Image infoIcon;
    public TextMeshProUGUI infoName;
    public TextMeshProUGUI infoPassiveDesc;
    public TextMeshProUGUI infoClass;

    private List<CharacterSlotUI> slots = new List<CharacterSlotUI>();
    private CharacterSlotUI selectedSlot;
    private List<CharacterDef> currentParty = new List<CharacterDef>();

    private void Start()
    {
        InitializeList();
    }

    private void InitializeList()
    {
        // Clear existing slots if any
        foreach (Transform child in gridContent)
        {
            Destroy(child.gameObject);
        }
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

        // TODO: 소유 캐릭터 시스템 도입 시 여기서 필터링
        foreach (var def in table.All)
        {
            if (def == null)
                continue;

            var slotGo = Instantiate(slotPrefab, gridContent);
            var slotUI = slotGo.GetComponent<CharacterSlotUI>();

            bool inParty = equippedSet != null && equippedSet.Contains(def.id);
            if (inParty)
                currentParty.Add(def);

            slotUI.Setup(def, inParty);
            slotUI.OnSelected = HandleSlotSelected;
            slots.Add(slotUI);

            // Setup Button component if missing
            var btn = slotGo.GetComponent<Button>();
            if (btn == null)
                btn = slotGo.gameObject.AddComponent<Button>();
            btn.onClick.AddListener(slotUI.OnClick);
        }

        var defaultSlot = FindFirstPartySlot() ?? (slots.Count > 0 ? slots[0] : null);
        if (defaultSlot != null)
            HandleSlotSelected(defaultSlot);

        UpdatePartyDisplay();
    }

    private CharacterSlotUI FindFirstPartySlot()
    {
        foreach (var s in slots)
            if (s.IsInParty)
                return s;
        return null;
    }

    private void HandleSlotSelected(CharacterSlotUI slot)
    {
        selectedSlot = slot;
        foreach (var s in slots)
        {
            s.UpdateState(s == slot);
        }

        UpdateInfoPanel(slot.Def);
    }

    private void UpdateInfoPanel(CharacterDef def)
    {
        if (infoIcon != null)
            infoIcon.sprite = def.icon;
        if (infoName != null)
            infoName.text = def.displayName;
        if (infoClass != null)
            infoClass.text = def.classType.ToString();
        if (infoPassiveDesc != null)
            infoPassiveDesc.text = def.description;
    }

    private void UpdatePartyDisplay()
    {
        for (int i = 0; i < partyPortraits.Length; i++)
        {
            if (i < currentParty.Count)
            {
                partyPortraits[i].sprite = currentParty[i].icon;
                partyPortraits[i].gameObject.SetActive(true);
            }
            else
            {
                partyPortraits[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnPlayClicked()
    {
        if (currentParty.Count > 0)
        {
            AdventurePartyContext.PendingCharacterIds = currentParty.Select(d => d.id).ToArray();
        }
        GameStateMachine.Instance.TransitionTo(GameState.Adventure);
    }

    public void OnBackClicked() => GameStateMachine.Instance.TransitionTo(GameState.Lobby);
}
