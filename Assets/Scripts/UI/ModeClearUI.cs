using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModeClearUI : MonoBehaviour
{
    [SerializeField]
    GameObject panel;

    [SerializeField]
    TMP_Text titleText;

    [SerializeField]
    Button restartButton;

    [SerializeField]
    Image[] characterIcons = new Image[3];

    [SerializeField]
    JokerCardSlotUI[] jokerSlots = new JokerCardSlotUI[5];

    static readonly ClassType[] DisplayOrder =
    {
        ClassType.Warrior,
        ClassType.Archer,
        ClassType.Priest,
    };

    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
        if (restartButton != null)
            restartButton.onClick.AddListener(() =>
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex)
            );
    }

    public void Show(GameManager gameManager, Color color, string title = "모험 완료")
    {
        if (titleText != null)
        {
            titleText.text = title;
            titleText.color = color;
        }

        var charSet = gameManager.CharacterSet;
        for (int i = 0; i < DisplayOrder.Length; i++)
        {
            var character = charSet?.GetCharacter(DisplayOrder[i]);
            SetSprite(i < characterIcons.Length ? characterIcons[i] : null, character?.Icon);
        }

        var hand = gameManager.JokerManager?.ActiveHand;
        for (int i = 0; i < jokerSlots.Length; i++)
        {
            if (jokerSlots[i] == null)
                continue;
            var card = (hand != null && i < hand.Length) ? hand[i] : null;
            jokerSlots[i].Refresh(card);
        }

        if (panel != null)
            panel.SetActive(true);
    }

    static void SetSprite(Image img, Sprite sprite)
    {
        if (img == null)
            return;
        img.enabled = sprite != null;
        if (sprite != null)
            img.sprite = sprite;
    }
}
