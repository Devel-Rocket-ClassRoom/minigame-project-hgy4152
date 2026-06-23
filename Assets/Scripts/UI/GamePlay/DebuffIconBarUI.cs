using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebuffIconBarUI : MonoBehaviour
{
    [SerializeField]
    BossPatternSystem bossPatternSystem;

    [SerializeField]
    StageManager stageManager;

    [SerializeField]
    Transform iconContainer;

    [SerializeField]
    GameObject debuffIconPrefab;

    [SerializeField]
    BossInfoUIPanel bossInfoUIPanel;

    [SerializeField]
    CharacterSet characterSet;

    SkadiCharacter _skadi;
    GameObject _frostIconGO;
    TextMeshProUGUI _frostStackText;
    int _lastFrost = int.MinValue; // 더티 플래그: 스택이 바뀐 프레임에만 UI 갱신

    void OnEnable()
    {
        bossPatternSystem.OnInjected += Refresh;
    }

    void OnDisable()
    {
        bossPatternSystem.OnInjected -= Refresh;
    }

    void ShowBossInfo()
    {
        if (bossInfoUIPanel == null)
            return;
        var entry = stageManager.Current;
        if (entry.bossData != null)
            bossInfoUIPanel.Show(entry.bossData);
        else if (entry.enemyData != null)
            bossInfoUIPanel.Show(entry.enemyData);
    }

    void Update()
    {
        if (_skadi == null || _frostIconGO == null)
            return;
        int frost = _skadi.FrostStacks;
        if (frost == _lastFrost)
            return;
        _lastFrost = frost;
        _frostIconGO.SetActive(frost > 0);
        if (frost > 0 && _frostStackText != null)
            _frostStackText.text = frost.ToString();
    }

    public void Refresh()
    {
        foreach (Transform child in iconContainer)
            Destroy(child.gameObject);
        _frostIconGO = null;
        _frostStackText = null;
        _lastFrost = int.MinValue;

        var mods = new System.Collections.Generic.List<Modifier>(
            bossPatternSystem.GetActiveModifiers()
        );

        foreach (var mod in mods)
        {
            var icon = Instantiate(debuffIconPrefab, iconContainer);
            if (mod.icon != null)
                icon.GetComponent<Image>().sprite = mod.icon;
            icon.GetComponent<Button>().onClick.AddListener(ShowBossInfo);
        }

        // 동상 아이콘: Skadi가 파티에 있을 때만
        _skadi = characterSet?.GetCharacter(ClassType.Archer) as SkadiCharacter;
        if (_skadi != null)
        {
            var def = characterSet.GetDef(_skadi);
            _frostIconGO = Instantiate(debuffIconPrefab, iconContainer);
            if (def?.passiveIcon != null)
                _frostIconGO.GetComponent<Image>().sprite = def.passiveIcon;
            _frostIconGO.GetComponent<Button>().onClick.AddListener(ShowBossInfo);
            _frostStackText = _frostIconGO.GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}
