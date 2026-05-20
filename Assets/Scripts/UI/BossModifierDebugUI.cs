using System.Text;
using TMPro;
using UnityEngine;

public class BossModifierDebugUI : MonoBehaviour
{
    [SerializeField]
    BossPatternSystem system;

    [SerializeField]
    TMP_Text label;

    void OnEnable()
    {
        system.OnInjected += Refresh;
    }

    void OnDisable()
    {
        system.OnInjected -= Refresh;
    }

    void Refresh()
    {
        var p = system.Current;
        if (p == null)
        {
            label.text = "";
            return;
        }

        var sb = new StringBuilder();
        sb.Append($"Turn {system.TurnIndex + 1}: ");
        foreach (var m in p.passive)
            if (m != null)
                sb.Append($"[P]{m.modName} ");
        if (system.TurnIndex < p.turnModifiers.Length && p.turnModifiers[system.TurnIndex] != null)
            sb.Append($"[T]{p.turnModifiers[system.TurnIndex].modName}");
        label.text = sb.ToString();
    }
}
