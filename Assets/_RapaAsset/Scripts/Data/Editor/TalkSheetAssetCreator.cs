using UnityEngine;
using UnityEditor;
using System.IO;
using UnityQuickSheet;

///
/// !!! Machine generated code !!!
/// 
public partial class GoogleDataAssetUtility
{
    [MenuItem("Assets/Create/Google/TalkSheet")]
    public static void CreateTalkSheetAssetFile()
    {
        TalkSheet asset = CustomAssetUtility.CreateAsset<TalkSheet>();
        asset.SheetName = "ETASheet";
        asset.WorksheetName = "TalkSheet";
        EditorUtility.SetDirty(asset);        
    }
    
}