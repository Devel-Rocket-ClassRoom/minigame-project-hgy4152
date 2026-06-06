using UnityEngine;

public class PatternPreviewUI : MonoBehaviour
{
    public Transform cardContainer;
    public PatternCardUI cardPrefab;

    public void Show(BossPattern pattern)
    {
        gameObject.SetActive(true);

        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);

        if (pattern == null)
            return;

        foreach (var passive in pattern.passive)
        {
            var card = Instantiate(cardPrefab, cardContainer);
            card.Setup(Localization.Get("ui_label_passive"), passive);
        }

        for (int i = 0; i < pattern.phaseModifiers.Length; i++)
        {
            var card = Instantiate(cardPrefab, cardContainer);
            card.Setup($"Phase {i + 1}", pattern.phaseModifiers[i]);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
