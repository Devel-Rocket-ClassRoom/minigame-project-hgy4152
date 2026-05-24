using TMPro;
using UnityEngine;

public class PatternCardUI : MonoBehaviour
{
    public TextMeshProUGUI typeLabel;
    public TextMeshProUGUI modNameLabel;
    public TextMeshProUGUI descLabel;

    public void Setup(string type, Modifier mod)
    {
        if (typeLabel != null)
            typeLabel.text = type;
        if (modNameLabel != null)
            modNameLabel.text = mod != null ? mod.modName : "—";
        if (descLabel != null)
            descLabel.text = mod != null ? mod.description : "";
    }
}
