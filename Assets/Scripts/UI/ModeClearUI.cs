using TMPro;
using UnityEngine;
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
    Button exitButton;

    [SerializeField]
    Image[] characterIcons = new Image[3];

    [SerializeField]
    JokerCardSlotUI[] jokerSlots = new JokerCardSlotUI[5];

    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
        if (restartButton != null)
            restartButton.onClick.AddListener(() =>
            {
                if (GameStateMachine.Instance != null)
                    GameStateMachine.Instance.TransitionTo(GameState.AdventureReady);
                else
                    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            });
        if (exitButton != null)
            exitButton.onClick.AddListener(() =>
            {
                if (GameStateMachine.Instance != null)
                    GameStateMachine.Instance.TransitionTo(GameState.Lobby);
                else
                    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            });
    }

    public void Show(GameManager gameManager, Color color, string title = "모드 클리어")
    {
        if (titleText != null)
        {
            titleText.text = title;
            titleText.color = color;
        }

        var charSet = gameManager.CharacterSet;
        var deployed = charSet?.GetDeployedClassTypes();
        for (int i = 0; i < characterIcons.Length; i++)
        {
            var character =
                (deployed != null && i < deployed.Length) ? charSet.GetCharacter(deployed[i]) : null;
            SetSprite(characterIcons[i], character?.Icon);
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
