using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPortraitUI : MonoBehaviour
{
    [SerializeField]
    Image portrait;

    [SerializeField]
    Button button;

    public CharacterDef Def { get; private set; }
    public RectTransform ButtonRect =>
        button != null ? button.transform as RectTransform : transform as RectTransform;

    public event Action<CharacterPortraitUI> OnClicked;

    void Awake()
    {
        if (button != null)
            button.onClick.AddListener(() => OnClicked?.Invoke(this));
    }

    public void Bind(CharacterDef def, Sprite icon)
    {
        Def = def;
        if (portrait != null)
            portrait.sprite = icon;
        gameObject.SetActive(def != null);
    }
}
