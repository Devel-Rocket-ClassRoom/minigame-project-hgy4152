using System.Text;
using TMPro;
using UnityEngine;

public class BossEffectUI : MonoBehaviour
{
    [SerializeField]
    BossPatternSystem system;

    [SerializeField]
    TMP_Text label;

    [SerializeField]
    GameObject panel;

    void OnEnable()
    {
        system.OnInjected += Refresh;
    }

    void OnDisable()
    {
        system.OnInjected -= Refresh;
    }

    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    void Refresh()
    {
        var p = system.Current;
        if (p == null)
        {
            if (panel != null)
                panel.SetActive(false);
            return;
        }

        if (panel != null)
            panel.SetActive(true);

        var sb = new StringBuilder();
        foreach (var m in p.passive)
        {
            if (m == null)
                continue;
            sb.AppendLine(
                $"[{Localization.Get("ui_label_passive")}] {Localization.Get(m.modName)}"
            );
            if (!string.IsNullOrEmpty(m.description))
                sb.AppendLine(Localization.Get(m.description));
        }

        if (system.TurnIndex < p.turnModifiers.Length)
        {
            var tm = p.turnModifiers[system.TurnIndex];
            if (tm != null)
            {
                sb.AppendLine(
                    $"[{Localization.Get("ui_label_this_turn")}] {Localization.Get(tm.modName)}"
                );
                if (!string.IsNullOrEmpty(tm.description))
                    sb.AppendLine(Localization.Get(tm.description));
            }
        }

        label.text = sb.ToString().TrimEnd();
    }
}
