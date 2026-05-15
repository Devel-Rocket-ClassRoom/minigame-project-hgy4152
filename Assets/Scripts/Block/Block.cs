using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Block : MonoBehaviour
{
    public BlockData data;
    public int chainGroupId = -1;

    SpriteRenderer _sr;
    SpriteRenderer _background;

    static Sprite _whiteSprite;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _background = CreateBackground();
    }

    public void Init(BlockData blockData)
    {
        data = blockData;
        if (blockData.icon != null)
            _sr.sprite = blockData.icon;
        _background.color = blockData.blockColor;
    }

    SpriteRenderer CreateBackground()
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
        var sr = bgGO.AddComponent<SpriteRenderer>();
        sr.sprite = _whiteSprite;
        sr.sortingOrder = _sr.sortingOrder - 1;
        return sr;
    }
}
