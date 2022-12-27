using UnityEngine;
using UnityEditor;
using System.IO;
using UnityQuickSheet;

///
/// !!! Machine generated code !!!
/// 
public partial class GoogleDataAssetUtility
{
    [MenuItem("Assets/Create/Google/ScanSheet")]
    public static void CreateScanSheetAssetFile()
    {
        ScanSheet asset = CustomAssetUtility.CreateAsset<ScanSheet>();
        asset.SheetName = "ETASheet";
        asset.WorksheetName = "ScanSheet";
        EditorUtility.SetDirty(asset);        
    }
    
}