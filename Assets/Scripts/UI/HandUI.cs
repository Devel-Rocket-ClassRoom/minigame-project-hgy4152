using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI groupText;

    public void Refresh(List<ChainGroup> groups)
    {
        var sb = new StringBuilder();
        foreach (var g in groups)
            sb.AppendLine($"[{g.Blocks[0].chainGroupId}] {g.DominantClass} x{g.Length}");
        groupText.text = sb.ToString();
    }
}
