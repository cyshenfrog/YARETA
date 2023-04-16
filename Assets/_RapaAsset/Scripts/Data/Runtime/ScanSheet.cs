using UnityEngine;

///
/// !!! Machine generated code !!!
///
/// A class which deriveds ScritableObject class so all its data
/// can be serialized onto an asset data file.
///
[System.Serializable]
public class ScanSheet : ScriptableObject
{
    [HideInInspector]
    [SerializeField]
    public string SheetName = "";

    [HideInInspector]
    [SerializeField]
    public string WorksheetName = "";

    // Note: initialize in OnEnable() not here.
    public ScanSheetData[] dataArray;

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
            dataArray = new ScanSheetData[0];
    }

    public string[] GetNameArray()
    {
        string[] s = new string[dataArray.Length];
        for (int i = 0; i < dataArray.Length; i++)
        {
            s[i] = dataArray[i].Enumname;
        }
        return s;
    }

    public string GetName(int ID, ClearLevel nearLevel, SystemLanguage language)
    {
        switch (nearLevel)
        {
            case ClearLevel.清楚:
                return GetFullName(ID, language);

            case ClearLevel.模糊:
                return GetBlurName(ID, language);

            case ClearLevel.在畫三小:
            default:
                return GetFullName(23, language);
        }
    }

    public string GetFullName(int ID, SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseTraditional:
            case SystemLanguage.ChineseSimplified:
                return dataArray[ID].Fullname;

            case SystemLanguage.English:
            default:
                return dataArray[ID].Fullnameen;
        }
    }

    public string GetBlurName(int ID, SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseTraditional:
            case SystemLanguage.ChineseSimplified:
                return dataArray[ID].Name;

            case SystemLanguage.English:
            default:
                return dataArray[ID].Nameen;
        }
    }

    public string GetInfo(int ID, SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseTraditional:
            case SystemLanguage.ChineseSimplified:
                return dataArray[ID].Info;

            case SystemLanguage.English:
            default:
                return dataArray[ID].Infoen;
        }
    }
}