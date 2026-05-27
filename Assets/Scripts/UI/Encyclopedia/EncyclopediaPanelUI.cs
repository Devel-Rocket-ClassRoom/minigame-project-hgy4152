using TMPro;
using UnityEngine;

public class EncyclopediaPanelUI : MonoBehaviour
{
    [SerializeField]
    TMP_Text headerText;

    [SerializeField]
    Transform grid;

    public Transform Grid => grid;

    public void SetHeader(string label)
    {
        if (headerText != null)
            headerText.text = label;
    }
}
