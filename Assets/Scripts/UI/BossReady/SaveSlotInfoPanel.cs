using UnityEngine;
using UnityEngine.UI;

public class SaveSlotInfoPanel : MonoBehaviour
{
    [SerializeField]
    Button backdropButton;

    [SerializeField]
    Image[] characterIcons = new Image[3];

    [SerializeField]
    Image[] jokerIcons = new Image[5];

    void Awake()
    {
        if (backdropButton != null)
            backdropButton.onClick.AddListener(Hide);
        gameObject.SetActive(false);
    }

    public void Show(SaveSlotData data)
    {
        var reg = TableRegistry.Instance;

        for (int i = 0; i < characterIcons.Length; i++)
        {
            Sprite sp = null;
            if (
                reg != null
                && i < data.characterIds.Length
                && !string.IsNullOrEmpty(data.characterIds[i])
            )
                sp = reg.Character.Get(data.characterIds[i])?.prefab.Icon;
            SetIcon(characterIcons[i], sp);
        }

        for (int i = 0; i < jokerIcons.Length; i++)
        {
            Sprite sp = null;
            if (reg != null && i < data.jokerIds.Length && !string.IsNullOrEmpty(data.jokerIds[i]))
                sp = reg.JokerCard.Get(data.jokerIds[i])?.icon;
            SetIcon(jokerIcons[i], sp);
        }

        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);

    static void SetIcon(Image img, Sprite sprite)
    {
        if (img == null)
            return;
        img.enabled = sprite != null;
        if (sprite != null)
            img.sprite = sprite;
    }
}
