using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebuffInfoPopupUI : MonoBehaviour
{
    [SerializeField]
    RectTransform panel;

    [SerializeField]
    Button backdropButton;

    [SerializeField]
    Transform entryContainer;

    [SerializeField]
    GameObject modifierEntryPrefab;

    [SerializeField]
    float slideDuration = 0.25f;

    Vector2 _shownPos;
    Vector2 _hiddenPos;
    Coroutine _slide;

    void Awake()
    {
        _shownPos = panel.anchoredPosition;
        _hiddenPos = _shownPos - new Vector2(panel.sizeDelta.x, 0);
        panel.anchoredPosition = _hiddenPos;
        panel.gameObject.SetActive(false);

        if (backdropButton != null)
        {
            backdropButton.gameObject.SetActive(false);
            backdropButton.onClick.AddListener(Hide);
        }
    }

    public void Show(IEnumerable<Modifier> modifiers)
    {
        foreach (Transform child in entryContainer)
            Destroy(child.gameObject);

        foreach (var mod in modifiers)
        {
            var entry = Instantiate(modifierEntryPrefab, entryContainer);
            var texts = entry.GetComponentsInChildren<TMP_Text>();
            if (texts.Length >= 1)
                texts[0].text = Localization.Get(mod.modName);
            if (texts.Length >= 2)
                texts[1].text = Localization.Get(mod.description);
        }

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
