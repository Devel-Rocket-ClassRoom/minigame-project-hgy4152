using UnityEngine;
using UnityEngine.UI;

public class DebuffIconBarUI : MonoBehaviour
{
    [SerializeField]
    BossPatternSystem bossPatternSystem;

    [SerializeField]
    Transform iconContainer;

    [SerializeField]
    GameObject debuffIconPrefab;

    [SerializeField]
    InfoPopupUI infoPopupUI;

    void OnEnable()
    {
        bossPatternSystem.OnInjected += Refresh;
    }

    void OnDisable()
    {
        bossPatternSystem.OnInjected -= Refresh;
    }

    void Refresh()
    {
        foreach (Transform child in iconContainer)
            Destroy(child.gameObject);

        var mods = new System.Collections.Generic.List<Modifier>(
            bossPatternSystem.GetActiveModifiers()
        );

        // 아이콘 컨테이너만 토글 (자신의 GO는 항상 활성 유지해 이벤트 구독 유지)
        iconContainer.gameObject.SetActive(mods.Count > 0);

        foreach (var mod in mods)
        {
            var icon = Instantiate(debuffIconPrefab, iconContainer);
            icon.GetComponent<Button>()
                .onClick.AddListener(() =>
                    infoPopupUI.ShowDebuffs(bossPatternSystem.GetActiveModifiers())
                );
        }
    }
}
