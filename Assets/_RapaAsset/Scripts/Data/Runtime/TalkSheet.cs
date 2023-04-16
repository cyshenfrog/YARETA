using UnityEngine;

///
/// !!! Machine generated code !!!
///
/// A class which deriveds ScritableObject class so all its data
/// can be serialized onto an asset data file.
///
[System.Serializable]
public class TalkSheet : ScriptableObject
{
    [HideInInspector]
    [SerializeField]
    public string SheetName = "";

    [HideInInspector]
    [SerializeField]
    public string WorksheetName = "";

    // Note: initialize in OnEnable() not here.
    public TalkSheetData[] dataArray;

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
            dataArray = new TalkSheetData[0];
    }

    public string[] GetTalks(int ID)
    {
        switch (SaveDataManager.Language)
        {
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseTraditional:
            case SystemLanguage.ChineseSimplified:
                return dataArray[ID].Ch.Split(';');
            case SystemLanguage.English:
            default:
                return dataArray[ID].En.Split(';');
        }
    }

    public string[] GetNameArray()
    {
        string[] s = new string[dataArray.Length];
        for (int i = 0; i < dataArray.Length; i++)
        {
            s[i] = dataArray[i].Talkname;
        }
        return s;
    }
}