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
        var seen = new HashSet<(ClassType, int)>();
        for (int i = 0; i < groups.Count; i++)
        {
            var g = groups[i];
            // 등록 및 중복체크 
            if (!seen.Add((g.DominantClass, g.Length)))
                continue;

            var character = gameManager.CharacterSet?.GetCharacter(g.DominantClass);
            string hex =
                character != null ? ColorUtility.ToHtmlStringRGB(character.classColor) : "FFFFFF";
            sb.AppendLine(
                $"<color=#{hex}>{g.DominantClass} x{g.Length} - {damages[i]}</color>"
            );
        }
        groupText.text = sb.ToString();
    }
}
