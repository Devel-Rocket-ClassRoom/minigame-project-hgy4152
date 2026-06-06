using TMPro;
using UnityEngine;

public class BuffIconUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI stackText;

    Character _character;

    public void Bind(Character character)
    {
        _character = character;
        UpdateDisplay();
    }

    void Update() => UpdateDisplay();

    void UpdateDisplay()
    {
        if (stackText == null)
            return;
        int stack = _character != null ? _character.StackCount : -1;
        stackText.gameObject.SetActive(stack >= 0);
        if (stack >= 0)
            stackText.text = stack.ToString();
    }
}
