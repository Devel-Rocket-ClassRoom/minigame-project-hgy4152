using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InfoPopupUI : MonoBehaviour
{
    [SerializeField]
    RectTransform panel;

    [SerializeField]
    Button backdropButton;

    [Header("Content")]
    [FormerlySerializedAs("nameText")]
    [SerializeField]
    TMP_Text titleNameText;

    [SerializeField]
    TMP_Text descText;

    [SerializeField]
    GameObject subPanel1;

    [FormerlySerializedAs("passiveTitle")]
    [SerializeField]
    TMP_Text subTitle1Text;

    [FormerlySerializedAs("passiveText")]
    [SerializeField]
    TMP_Text subDesc1Text;

    [SerializeField]
    GameObject subPanel2;

    [FormerlySerializedAs("blockTitle")]
    [SerializeField]
    TMP_Text subTitle2Text;

    [FormerlySerializedAs("blockText")]
    [SerializeField]
    TMP_Text subDesc2Text;

    [SerializeField]
    DebuffInfoPopupUI debuffInfoPopup;

    [SerializeField]
    Image infoIconImage;

    [SerializeField]
    GameObject lockInfoPanel;

    [SerializeField]
    TMP_Text lockDescText;

    [SerializeField]
    float slideDuration = 0.25f;

    Vector2 _shownPos;
    Vector2 _hiddenPos;
    Coroutine _slide;

    void Awake()
    {
        panel.gameObject.SetActive(false);

        if (backdropButton != null)
        {
            backdropButton.gameObject.SetActive(false);
            backdropButton.onClick.AddListener(Hide);
        }
    }

    void Start()
    {
        _shownPos = panel.anchoredPosition;
        _hiddenPos = _shownPos + new Vector2(panel.sizeDelta.x, 0);
        panel.anchoredPosition = _hiddenPos;
    }

    public void ShowCharacter(CharacterDef def)
    {
        ShowWith(() =>
        {
            if (debuffInfoPopup != null)
                debuffInfoPopup.gameObject.SetActive(false);
            if (lockInfoPanel != null)
                lockInfoPanel.SetActive(false);
            if (lockDescText != null)
                lockDescText.gameObject.SetActive(false);

            if (infoIconImage != null)
            {
                infoIconImage.sprite = def.prefab.Icon;
                infoIconImage.preserveAspect = true;
            }
            SetText(titleNameText, Localization.Get(def.displayName));
            SetText(descText, Localization.Get(def.charDescription));

            SetSubPanel(
                subPanel1,
                subTitle1Text,
                subDesc1Text,
                true,
                $"{Localization.Get("ui_skill_type_passive")} - {Localization.Get(def.passiveName)}",
                Localization.Get(def.description)
            );

            bool hasBlock = def.blockData != null;
            SetSubPanel(
                subPanel2,
                subTitle2Text,
                subDesc2Text,
                hasBlock,
                hasBlock
                    ? $"{Localization.Get("ui_skill_type_block_skill")} - {Localization.Get(def.blockData.displayName)}"
                    : null,
                hasBlock ? Localization.Get(def.blockData.description) : null
            );
        });
    }

    public void ShowJoker(JokerCard card)
    {
        ShowWith(() =>
        {
            if (debuffInfoPopup != null)
                debuffInfoPopup.gameObject.SetActive(false);
            if (lockInfoPanel != null)
                lockInfoPanel.SetActive(false);
            if (lockDescText != null)
                lockDescText.gameObject.SetActive(false);

            if (infoIconImage != null)
            {
                infoIconImage.sprite = card.icon;
                infoIconImage.preserveAspect = true;
            }
            SetText(titleNameText, Localization.Get(card.cardName));
            SetText(descText, Localization.Get(card.description));

            SetSubPanel(
                subPanel1,
                subTitle1Text,
                subDesc1Text,
                true,
                Localization.Get("ui_info_effect"),
                Localization.Get(card.description)
            );

            SetSubPanel(subPanel2, subTitle2Text, subDesc2Text, false);
        });
    }

    public void ShowEnemy(EnemyData data) =>
        ShowEnemyInternal(data.icon, data.enemyName, data.description, data.bossPattern);

    public void ShowEnemy(BossData data) =>
        ShowEnemyInternal(data.icon, data.bossName, data.description, data.bossPattern);

    public void ShowLocked(List<UnlockCondition> conditions, Sprite icon = null)
    {
        ShowWith(() =>
        {
            if (debuffInfoPopup != null)
                debuffInfoPopup.gameObject.SetActive(false);
            if (titleNameText != null)
                titleNameText.gameObject.SetActive(false);
            if (descText != null)
                descText.gameObject.SetActive(false);
            SetSubPanel(subPanel1, subTitle1Text, subDesc1Text, false);
            SetSubPanel(subPanel2, subTitle2Text, subDesc2Text, false);

            if (infoIconImage != null)
            {
                infoIconImage.sprite = icon;
                infoIconImage.preserveAspect = true;
            }
            if (lockInfoPanel != null)
                lockInfoPanel.SetActive(true);
            if (lockDescText != null)
            {
                lockDescText.gameObject.SetActive(true);
                lockDescText.text = FormatConditions(conditions);
            }
        });
    }

    public void ShowDebuffs(IEnumerable<Modifier> modifiers)
    {
        ShowWith(() =>
        {
            if (lockInfoPanel != null)
                lockInfoPanel.SetActive(false);
            if (lockDescText != null)
                lockDescText.gameObject.SetActive(false);
            if (titleNameText != null)
                titleNameText.gameObject.SetActive(false);
            if (descText != null)
                descText.gameObject.SetActive(false);
            SetSubPanel(subPanel1, subTitle1Text, subDesc1Text, false);
            SetSubPanel(subPanel2, subTitle2Text, subDesc2Text, false);
            if (debuffInfoPopup != null)
            {
                debuffInfoPopup.gameObject.SetActive(true);
                debuffInfoPopup.Populate(modifiers);
            }
        });
    }

    void ShowEnemyInternal(Sprite icon, string nameKey, string descKey, BossPattern bp)
    {
        ShowWith(() =>
        {
            if (debuffInfoPopup != null)
                debuffInfoPopup.gameObject.SetActive(false);
            if (lockInfoPanel != null)
                lockInfoPanel.SetActive(false);
            if (lockDescText != null)
                lockDescText.gameObject.SetActive(false);

            if (infoIconImage != null)
            {
                infoIconImage.sprite = icon;
                infoIconImage.preserveAspect = true;
            }
            SetText(titleNameText, Localization.Get(nameKey));
            SetText(descText, Localization.Get(descKey));

            bool hasPassive = bp?.passive != null && bp.passive.Count > 0;
            if (hasPassive)
            {
                var sb = new StringBuilder();
                foreach (var m in bp.passive)
                {
                    if (sb.Length > 0)
                        sb.Append('\n');
                    sb.Append(Localization.Get(m.description));
                }
                SetSubPanel(
                    subPanel1,
                    subTitle1Text,
                    subDesc1Text,
                    true,
                    $"{Localization.Get("ui_skill_type_passive")} - {Localization.Get(bp.passive[0].modName)}",
                    sb.ToString()
                );
            }
            else
                SetSubPanel(subPanel1, subTitle1Text, subDesc1Text, false);

            bool anyTurn = false;
            if (bp?.phaseModifiers != null)
            {
                var sb = new StringBuilder();
                for (int i = 0; i < bp.phaseModifiers.Length; i++)
                {
                    var m = bp.phaseModifiers[i];
                    if (m == null)
                        continue;
                    if (sb.Length > 0)
                        sb.Append('\n');
                    sb.Append(
                        $"{i + 1}{Localization.Get("ui_info_turn_suffix")} - {Localization.Get(m.modName)} : {Localization.Get(m.description)}"
                    );
                    anyTurn = true;
                }
                if (anyTurn)
                    SetSubPanel(
                        subPanel2,
                        subTitle2Text,
                        subDesc2Text,
                        true,
                        Localization.Get("ui_info_turn_skill"),
                        sb.ToString()
                    );
            }
            if (!anyTurn)
                SetSubPanel(subPanel2, subTitle2Text, subDesc2Text, false);
        });
    }

    void ShowWith(Action fillContent)
    {
        if (!panel.gameObject.activeSelf)
        {
            fillContent();
            Open();
            return;
        }

        if (backdropButton == null)
        {
            Slide(
                _shownPos,
                _hiddenPos,
                () =>
                {
                    fillContent();
                    Slide(_hiddenPos, _shownPos);
                }
            );
        }
        else
        {
            fillContent();
        }
    }

    static string FormatConditions(List<UnlockCondition> conditions)
    {
        var sb = new StringBuilder();
        foreach (var c in conditions)
        {
            if (sb.Length > 0)
                sb.Append('\n');
            string fmt = Localization.Get($"ui_unlock_cond_{c.type.ToString().ToLower()}");
            sb.Append(
                c.type == UnlockConditionType.ClearWithClass
                    ? string.Format(
                        fmt,
                        Localization.Get($"ui_class_{c.classType.ToString().ToLower()}"),
                        c.count
                    )
                    : string.Format(fmt, c.count)
            );
        }
        return sb.ToString();
    }

    static void SetText(TMP_Text tmp, string text)
    {
        if (tmp == null)
            return;
        tmp.gameObject.SetActive(true);
        tmp.text = text;
    }

    static void SetSubPanel(
        GameObject root,
        TMP_Text titleTmp,
        TMP_Text descTmp,
        bool active,
        string title = null,
        string desc = null
    )
    {
        if (root != null)
            root.SetActive(active);
        if (!active)
            return;
        if (titleTmp != null && title != null)
            titleTmp.text = title;
        if (descTmp != null && desc != null)
            descTmp.text = desc;
    }

    public void Hide()
    {
        if (backdropButton != null)
            backdropButton.gameObject.SetActive(false);
        Slide(_shownPos, _hiddenPos, () => panel.gameObject.SetActive(false));
    }

    public void HideImmediate()
    {
        if (_slide != null)
            StopCoroutine(_slide);
        if (backdropButton != null)
            backdropButton.gameObject.SetActive(false);
        panel.anchoredPosition = _hiddenPos;
        panel.gameObject.SetActive(false);
    }

    void Open()
    {
        panel.gameObject.SetActive(true);
        if (backdropButton != null)
            backdropButton.gameObject.SetActive(true);
        Slide(_hiddenPos, _shownPos);
    }

    void Slide(Vector2 from, Vector2 to, Action onComplete = null)
    {
        if (_slide != null)
            StopCoroutine(_slide);
        _slide = StartCoroutine(SlideRoutine(from, to, onComplete));
    }

    IEnumerator SlideRoutine(Vector2 from, Vector2 to, Action onComplete = null)
    {
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / slideDuration);
            panel.anchoredPosition = Vector2.LerpUnclamped(from, to, t);
            yield return null;
        }
        panel.anchoredPosition = to;
        onComplete?.Invoke();
    }
}
