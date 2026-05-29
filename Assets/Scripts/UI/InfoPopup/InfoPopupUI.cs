using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopupUI : MonoBehaviour
{
    [SerializeField]
    RectTransform panel;

    [SerializeField]
    Button backdropButton;

    [SerializeField]
    TMP_Text nameText;

    [SerializeField]
    TMP_Text passiveText;

    [SerializeField]
    TMP_Text passiveTitle;

    [SerializeField]
    TMP_Text blockText;

    [SerializeField]
    TMP_Text blockTitle;

    [SerializeField]
    DebuffInfoPopupUI debuffInfoPopup;

    [SerializeField]
    float slideDuration = 0.25f;

    Vector2 _shownPos;
    Vector2 _hiddenPos;
    Coroutine _slide;

    void Awake()
    {
        _shownPos = panel.anchoredPosition;
        _hiddenPos = _shownPos + new Vector2(panel.sizeDelta.x, 0);
        panel.anchoredPosition = _hiddenPos;
        panel.gameObject.SetActive(false);

        if (backdropButton != null)
        {
            backdropButton.gameObject.SetActive(false);
            backdropButton.onClick.AddListener(Hide);
        }
    }

    public void ShowCharacter(CharacterDef def)
    {
        SetDebuffSectionVisible(false);
        if (nameText != null)
            nameText.text = Localization.Get(def.DisplayName);
        if (passiveText != null)
            passiveText.text = Localization.Get(((IDisplayable)def).Description);
        if (blockText != null)
        {
            var hasBlock = def.blockData != null;
            blockText.gameObject.SetActive(hasBlock);
            if (hasBlock)
                blockText.text = Localization.Get(def.blockData.description);
        }
        Open();
    }

    public void ShowJoker(JokerCard card)
    {
        SetDebuffSectionVisible(false);
        if (nameText != null)
            nameText.text = Localization.Get(card.cardName);
        if (passiveText != null)
            passiveText.text = Localization.Get(card.description);
        if (blockText != null)
            blockText.gameObject.SetActive(false);
        Open();
    }

    public void ShowDebuffs(IEnumerable<Modifier> modifiers)
    {
        SetDebuffSectionVisible(true);
        if (debuffInfoPopup != null)
            debuffInfoPopup.Populate(modifiers);
        Open();
    }

    void SetDebuffSectionVisible(bool show)
    {
        if (debuffInfoPopup != null)
            debuffInfoPopup.gameObject.SetActive(show);
        if (nameText != null)
            nameText.gameObject.SetActive(!show);
        if (passiveText != null)
            passiveText.gameObject.SetActive(!show);
        if (passiveTitle != null)
            passiveTitle.gameObject.SetActive(!show);
        if (blockText != null)
            blockText.gameObject.SetActive(!show);
        if (blockTitle != null)
            blockTitle.gameObject.SetActive(!show);
    }

    public void Hide()
    {
        if (backdropButton != null)
            backdropButton.gameObject.SetActive(false);
        Slide(_shownPos, _hiddenPos, () => panel.gameObject.SetActive(false));
    }

    void Open()
    {
        panel.gameObject.SetActive(true);
        if (backdropButton != null)
            backdropButton.gameObject.SetActive(true);
        Slide(_hiddenPos, _shownPos);
    }

    void Slide(Vector2 from, Vector2 to, System.Action onComplete = null)
    {
        if (_slide != null)
            StopCoroutine(_slide);
        _slide = StartCoroutine(SlideRoutine(from, to, onComplete));
    }

    IEnumerator SlideRoutine(Vector2 from, Vector2 to, System.Action onComplete = null)
    {
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / slideDuration);
            panel.anchoredPosition = Vector2.LerpUnclamped(from, to, t);
            yield return null;
        }
        panel.anchoredPosition = to;
        onComplete?.Invoke();
    }
}
