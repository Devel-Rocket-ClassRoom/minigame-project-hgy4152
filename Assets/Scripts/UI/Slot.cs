using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField]
    Image connector;

    public Block Block { get; private set; }

    public void PlaceBlock(Block block, System.Action onArrived = null)
    {
        Block = block;
        block.transform.SetParent(transform, false);
        block.FlyIn(Vector2.zero, 0.4f, onArrived);
    }

    public void ShiftBlock(Block block, System.Action onArrived = null)
    {
        Block = block;
        block.transform.SetParent(transform, true);
        block.Slide(Vector2.zero, 0.2f, onArrived);
    }

    public void Clear()
    {
        Block = null;
    }

    public void SetConnector(bool active, Color color)
    {
        if (connector == null)
            return;
        connector.gameObject.SetActive(active);
        connector.color = color;
    }
}
