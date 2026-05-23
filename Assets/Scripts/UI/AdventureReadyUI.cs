using System.Collections.Generic;
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
    public TextMeshProUGUI infoName;
    public TextMeshProUGUI infoPassiveDesc;
    public TextMeshProUGUI infoClass;

    private List<CharacterSlotUI> slots = new List<CharacterSlotUI>();
    private CharacterSlotUI selectedSlot;
    private List<Character> currentParty = new List<Character>();

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
            Debug.LogWarning("[AdventureReadyUI] CharacterTable을 찾을 수 없어 목록 생성을 건너뜁니다.");
            return;
        }

        // TODO: 소유 캐릭터 시스템 도입 시 여기서 필터링
        foreach (var def in table.All)
        {
            if (def == null || def.prefab == null)
                continue;

            var slotGo = Instantiate(slotPrefab, gridContent);
            var slotUI = slotGo.GetComponent<CharacterSlotUI>();

            // Dummy check for party (e.g. first 2 are in party) — Part24-3에서 정리 예정
            bool inParty = currentParty.Count < 2;
            if (inParty)
                currentParty.Add(def.prefab);

            slotUI.Setup(def.prefab, inParty);
            slotUI.OnSelected = HandleSlotSelected;
            slots.Add(slotUI);

            // Setup Button component if missing
            var btn = slotGo.GetComponent<Button>();
            if (btn == null)
                btn = slotGo.gameObject.AddComponent<Button>();
            btn.onClick.AddListener(slotUI.OnClick);
        }

        if (slots.Count > 0)
            HandleSlotSelected(slots[0]);

        UpdatePartyDisplay();
    }

    private void HandleSlotSelected(CharacterSlotUI slot)
    {
        selectedSlot = slot;
        foreach (var s in slots)
        {
            s.UpdateState(s == slot);
        }

        UpdateInfoPanel(slot.Character);
    }

    private void UpdateInfoPanel(Character character)
    {
        if (infoName != null)
            infoName.text = character.CharacterName;
        if (infoClass != null)
            infoClass.text = character.Type.ToString();
        if (infoPassiveDesc != null)
            infoPassiveDesc.text = character.PassiveDescription;

        // Hide stats, show passive instead
        if (infoPassiveDesc != null)
            infoPassiveDesc.gameObject.SetActive(true);
    }

    private void UpdatePartyDisplay()
    {
        for (int i = 0; i < partyPortraits.Length; i++)
        {
            if (i < currentParty.Count)
            {
                partyPortraits[i].sprite = currentParty[i].Icon;
                partyPortraits[i].gameObject.SetActive(true);
            }
            else
            {
                partyPortraits[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnPlayClicked() => GameStateMachine.Instance.TransitionTo(GameState.Adventure);

    public void OnBackClicked() => GameStateMachine.Instance.TransitionTo(GameState.Lobby);
}
