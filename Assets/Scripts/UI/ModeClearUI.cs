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
        if (exitButton != null)
            exitButton.onClick.AddListener(() =>
            {
                if (GameStateMachine.Instance != null)
                    GameStateMachine.Instance.TransitionTo(GameState.Lobby);
                else
                    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            });
    }

    public void Show(GameManager gameManager, Color color, string titleKey = "ui_mode_clear")
    {
        if (titleText != null)
        {
            titleText.text = Localization.Get(titleKey);
            titleText.color = color;
        }

        var charSet = gameManager.CharacterSet;
        var instances = charSet?.GetInstances();
        for (int i = 0; i < characterIcons.Length; i++)
        {
            var character = (instances != null && i < instances.Length) ? instances[i] : null;
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

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            var target = gameManager.IsBossPlay ? GameState.BossReady : GameState.AdventureReady;
            restartButton.onClick.AddListener(() =>
            {
                if (GameStateMachine.Instance != null)
                    GameStateMachine.Instance.TransitionTo(target);
                else
                    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            });
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
        {
            img.sprite = sprite;
            img.preserveAspect = true;
        }
    }
}
