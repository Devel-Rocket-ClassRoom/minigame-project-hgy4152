using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI groupText;

    [SerializeField]
    GameManager gameManager;

    public void Refresh(List<ChainGroup> groups)
    {
        var sb = new StringBuilder();
        var damages = gameManager.PreviewGroupDamages(groups);
        var seen = new HashSet<(Character, int)>();
        for (int i = 0; i < groups.Count; i++)
        {
            var g = groups[i];
            if (!seen.Add((g.DominantCharacter, g.Length)))
                continue;

            var character = g.DominantCharacter;
            string hex =
                character != null ? ColorUtility.ToHtmlStringRGB(character.classColor) : "FFFFFF";
            sb.AppendLine(
                $"<color=#{hex}>{g.DominantClass} x{g.Length} - {damages[i]}</color>"
            );
        }
        groupText.text = sb.ToString();
    }
}
