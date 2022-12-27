using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityQuickSheet;

///
/// !!! Machine generated code !!!
///
[CustomEditor(typeof(ScanSheet))]
public class ScanSheetEditor : BaseExcelEditor<ScanSheet>
{
    public override bool Load()
    {
        ScanSheet targetData = target as ScanSheet;

        string path = targetData.SheetName;
        if (!File.Exists(path))
            return false;

        string sheet = targetData.WorksheetName;

        ExcelQuery query = new ExcelQuery(path, sheet);
        if (query != null && query.IsValid())
        {
            targetData.dataArray = query.Deserialize<ScanSheetData>().ToArray();
            EditorUtility.SetDirty(targetData);
            AssetDatabase.SaveAssets();
            GenerateEnum.Go(EditorTool_ScriptableObject.FindInstance<ScanSheet>().GetNameArray(), "ScanDataEnum");
            return true;
        }
        else
            return false;
    }
}