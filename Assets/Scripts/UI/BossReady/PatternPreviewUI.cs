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

        if (pattern == null) return;

        foreach (var passive in pattern.passive)
        {
            var card = Instantiate(cardPrefab, cardContainer);
            card.Setup("패시브", passive);
        }

        for (int i = 0; i < pattern.turnModifiers.Length; i++)
        {
            var card = Instantiate(cardPrefab, cardContainer);
            card.Setup($"Turn {i + 1}", pattern.turnModifiers[i]);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
