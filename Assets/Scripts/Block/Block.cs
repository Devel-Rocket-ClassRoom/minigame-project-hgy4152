using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    public BlockData data;
    public int chainGroupId = -1;

    Image _sprite;
    Image _background;

    static Sprite _whiteSprite;

    void Awake()
    {
        _background = CreateBackground();
        _sprite = CreateSpriteImage();

        // Ensure the button uses the background for visual feedback
        var button = GetComponent<UnityEngine.UI.Button>();
        if (button != null)
        {
            button.targetGraphic = _background;
            button.onClick.AddListener(OnClicked);
        }
    }

    void OnClicked()
    {
        Debug.Log($"[Block] Clicked: {data?.id} ({data?.ownerClass})");
    }

    public void Init(BlockData blockData)
    {
        data = blockData;
        if (blockData.icon != null)
            _sprite.sprite = blockData.icon;
        _background.color = blockData.blockColor;
    }

    Image CreateBackground()
    {
        if (_whiteSprite == null)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            _whiteSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 1f);
        }

        var bgGO = new GameObject("Background");
        bgGO.transform.SetParent(transform, false);
        bgGO.transform.localScale = Vector3.one * 1.1f;
        var img = bgGO.AddComponent<Image>();
        img.sprite = _whiteSprite;
        return img;
    }

    Image CreateSpriteImage()
    {
        var spriteGO = new GameObject("SpriteImage");
        spriteGO.transform.SetParent(transform, false);
        return spriteGO.AddComponent<Image>();
    }
}
