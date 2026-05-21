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
            sb.AppendLine($"[패시브] {m.modName}");
            if (!string.IsNullOrEmpty(m.description))
                sb.AppendLine(m.description);
        }

        if (system.TurnIndex < p.turnModifiers.Length)
        {
            var tm = p.turnModifiers[system.TurnIndex];
            if (tm != null)
            {
                sb.AppendLine($"[이번 턴] {tm.modName}");
                if (!string.IsNullOrEmpty(tm.description))
                    sb.AppendLine(tm.description);
            }
        }

        label.text = sb.ToString().TrimEnd();
    }
}
