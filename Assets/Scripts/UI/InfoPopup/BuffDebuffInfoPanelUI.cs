using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct BuffDebuffEntry
{
    public Sprite icon;
    public string name;
    public string desc;
}

public class BuffDebuffInfoPanelUI : MonoBehaviour
{
    [SerializeField]
    RectTransform panel;

    [SerializeField]
    Button backdropButton;

    [SerializeField]
    Transform slotContainer;

    [SerializeField]
    GameObject slotPrefab;

    [SerializeField]
    float slideDuration = 0.25f;

    // true = 오른편(파티측), false = 왼편(보스측)
    [SerializeField]
    bool slideFromRight = true;

    Vector2 _shownPos;
    Vector2 _hiddenPos;
    Coroutine _slide;

    void Awake()
    {
        _shownPos = panel.anchoredPosition;
        float offset = slideFromRight ? panel.sizeDelta.x : -panel.sizeDelta.x;
        _hiddenPos = _shownPos + new Vector2(offset, 0);
        panel.anchoredPosition = _hiddenPos;
        panel.gameObject.SetActive(false);

        if (backdropButton != null)
        {
            backdropButton.gameObject.SetActive(false);
            backdropButton.onClick.AddListener(Hide);
        }
    }

    public void Show(IEnumerable<BuffDebuffEntry> entries)
    {
        Populate(entries);
        panel.gameObject.SetActive(true);
        if (backdropButton != null)
            backdropButton.gameObject.SetActive(true);
        Slide(_hiddenPos, _shownPos);
    }

    public void Hide()
    {
        if (backdropButton != null)
            backdropButton.gameObject.SetActive(false);
        Slide(_shownPos, _hiddenPos, () => panel.gameObject.SetActive(false));
    }

    void Populate(IEnumerable<BuffDebuffEntry> entries)
    {
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        foreach (var e in entries)
        {
            var slot = Instantiate(slotPrefab, slotContainer);
            var img = slot.GetComponentInChildren<Image>();
            if (img != null && e.icon != null)
                img.sprite = e.icon;
            var texts = slot.GetComponentsInChildren<TMP_Text>();
            if (texts.Length >= 1)
                texts[0].text = e.name;
            if (texts.Length >= 2)
                texts[1].text = e.desc;
        }
    }

    void Slide(Vector2 from, Vector2 to, Action onComplete = null)
    {
        if (_slide != null)
            StopCoroutine(_slide);
        _slide = StartCoroutine(SlideRoutine(from, to, onComplete));
    }

    IEnumerator SlideRoutine(Vector2 from, Vector2 to, Action onComplete = null)
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
