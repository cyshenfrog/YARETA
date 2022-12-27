using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

[RequireComponent(typeof(Text))]
public class UI_LocalizationText : MonoBehaviour
{
    public bool SetAlignment;

    [ShowIf("SetAlignment")]
    public TextAnchor EnAlignment;

    [ShowIf("SetAlignment")]
    public TextAnchor ChAlignment;

    public bool UpdateFontOnly;

    [HideIf("UpdateFontOnly")]
    public UIDataEnum UIText;

    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
        UpdateText();
        SaveDataManager.OnLanguageChanged += UpdateText;
    }

    private void UpdateText()
    {
        if (!UpdateFontOnly)
            text.text = GameManager.Instance.UISheet.GetUIText(UIText);
        if (SetAlignment)
        {
            text.alignment = SaveDataManager.Language == SystemLanguage.English ? EnAlignment : ChAlignment;
        }
        text.font = GameManager.Instance.GetFont();
        text.transform.localScale = SaveDataManager.Language == SystemLanguage.English ? Vector3.one : (Vector3.one - Vector3.right * 0.1f);
    }
}