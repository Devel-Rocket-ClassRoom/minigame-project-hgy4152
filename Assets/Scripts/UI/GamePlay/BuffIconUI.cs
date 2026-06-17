using TMPro;
using UnityEngine;

public class BuffIconUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI stackText;

    Character _character;
    int _lastStack = int.MinValue; // 더티 플래그: 값이 바뀐 프레임에만 UI 갱신

    public void Bind(Character character)
    {
        _character = character;
        _lastStack = int.MinValue;
        UpdateDisplay();
    }

    void Update() => UpdateDisplay();

    void UpdateDisplay()
    {
        if (stackText == null)
            return;
        int stack = _character != null ? _character.StackCount : -1;
        if (stack == _lastStack)
            return;
        _lastStack = stack;
        stackText.gameObject.SetActive(stack >= 0);
        if (stack >= 0)
            stackText.text = stack.ToString();
    }
}
