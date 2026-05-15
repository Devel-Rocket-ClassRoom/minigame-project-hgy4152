using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Block : MonoBehaviour
{
    public BlockData data;
    public int chainGroupId = -1;

    SpriteRenderer _sr;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    public void Init(BlockData blockData)
    {
        data = blockData;
        if (blockData != null && blockData.icon != null)
            _sr.sprite = blockData.icon;
    }
}
