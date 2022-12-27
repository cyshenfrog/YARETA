using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

///
/// !!! Machine generated code !!!
///
/// A class which deriveds ScritableObject class so all its data
/// can be serialized onto an asset data file.
///
[System.Serializable]
public class UISheet : ScriptableObject
{
    [HideInInspector]
    [SerializeField]
    public string SheetName = "";

    [HideInInspector]
    [SerializeField]
    public string WorksheetName = "";

    // Note: initialize in OnEnable() not here.
    public UISheetData[] dataArray;

    private void OnEnable()
    {
        //#if UNITY_EDITOR
        //hideFlags = HideFlags.DontSave;
        //#endif
        // Important:
        //    It should be checked an initialization of any collection data before it is initialized.
        //    Without this check, the array collection which already has its data get to be null
        //    because OnEnable is called whenever Unity builds.
        //
        if (dataArray == null)
            dataArray = new UISheetData[0];
    }

    public string[] GetNameArray()
    {
        string[] s = new string[dataArray.Length];
        for (int i = 0; i < dataArray.Length; i++)
        {
            s[i] = dataArray[i].Ch;
        }
        return s;
    }

    public string GetUIText(UIDataEnum targetText)
    {
        switch (SaveDataManager.Language)
        {
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseTraditional:
            case SystemLanguage.ChineseSimplified:
                return dataArray[(int)targetText].Ch;

            case SystemLanguage.English:
            default:
                return dataArray[(int)targetText].En;
        }
    }
}